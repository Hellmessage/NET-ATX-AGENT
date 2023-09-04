using HAtxLib.Extend;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace HAtxLib.Utils {
	public class HSocket : IDisposable {
		private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private readonly string _host;
		private readonly int _port;

		public static HSocket Create(string url) {
			return new HSocket(url);
		}

		public static HSocket Create(string host, int port) {
			return new HSocket(host, port);
		}

		public static HSocket Create(int port) {
			return new HSocket(port);
		}

		public HSocket(string host, int port) {
			_port = port;
			_host = host;
			_socket.Connect(_host, _port);
		}

		public HSocket(string url) {
			Uri uri = new Uri(url);
			_port = uri.Port;
			_host = uri.Host;
			_socket.Connect(_host, _port);
		}

		public HSocket(int port) : this("127.0.0.1", port){}


		private readonly static string[] HttpIgnoreKeys = new string[] { "user-agent", "host", "content-length" };

		public HttpResult HttpRequest(string method, string path, Dictionary<string, object> header = null, string data = null) {
			var keys = new List<string>();
			StringBuilder sb = new StringBuilder($"{method} {path} HTTP/1.1\r\n");
			if (header != null) {
				foreach (var kv in header) {
					string key = kv.Key.ToLower();
					if (HttpIgnoreKeys.Contains(key)) {
						continue;
					}
					if (keys.Contains(key)) {
						continue;
					}
					keys.Add(key);
					sb.Append($"{kv.Key}: {kv.Value}\r\n");
				}
			}
			sb.Append($"User-Agent: Hell\r\n");
			sb.Append($"Host: {_host}:{_port}\r\n");
			if (!keys.Contains("connection")) {
				sb.Append($"Connection: close\r\n");
			}
			if (!keys.Contains("accept")) {
				sb.Append($"Accept: */*\r\n");
			}
			if (data != null) {
				sb.Append($"Content-Length: {Encoding.UTF8.GetBytes(data).Length}\r\n\r\n");
				sb.Append(data);
			} else {
				sb.Append("\r\n");
			}
			string raw = sb.ToString();
			byte[] bytes = Encoding.UTF8.GetBytes(raw);
			_socket.Send(bytes);
			byte[] headerBuffer = ReadEndWith("\r\n\r\n");
			raw = Encoding.UTF8.GetString(headerBuffer).ToLower();
			var result = HttpResult.Create(raw);
			result.SetContent((result.ContentLength > 0 ? Encoding.UTF8.GetString(ReadLength(result.ContentLength)) : result.ContentLength == -1 ? ReadToClose() : "").Strip());
			return result;
		}

		public HttpResult HttpGet(string path) {
			return HttpRequest("GET", path);
		}

		public HttpResult HttpDelete(string path) {
			return HttpRequest("DELETE", path);
		}

		public HttpResult HttpPost(string path, JObject json) {
			string postdata = "";
			if (json != null) {
				postdata = json.ToString(Newtonsoft.Json.Formatting.None);
			}
			return HttpRequest("POST", path, new Dictionary<string, object>() { { "Content-Type", "application/json; charset=UTF-8" } }, postdata.Strip());
		}

		private string ReadToClose() {
			using (MemoryStream stream = new MemoryStream()) {
				byte[] buffer = new byte[4096];
				int len = _socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
				while (len > 0) {
					stream.Write(buffer, 0, len);
					if (len != buffer.Length) {
						break;
					}
					len = _socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
				}
				buffer = stream.ToArray();
				return Encoding.UTF8.GetString(buffer);
			}
		}

		private byte[] ReadEndWith(string str) {
			byte[] be = Encoding.UTF8.GetBytes(str);
			List<byte> bytes = new List<byte>();
			byte[] buf = new byte[1];
			int len = _socket.Receive(buf);
			while (len > 0) {
				bytes.Add(buf[0]);
				if (bytes.Count > be.Length) {
					bool exist = true;
					for (int i = 1; i < be.Length; i++) {
						if (bytes[bytes.Count - i] != be[be.Length - i]) {
							exist = false;
							break;
						}
					}
					if (exist) {
						return bytes.ToArray();
					}
				}
				len = _socket.Receive(buf);
			}
			return bytes.ToArray();
		}

		private byte[] ReadLength(int length) {
			using (MemoryStream stream = new MemoryStream()) {
				byte[] buffer = new byte[length];
				int len = _socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
				while (len > 0) {
					stream.Write(buffer, 0, len);
					length -= len;
					if (length == 0) {
						break;
					}
					buffer = new byte[length];
					len = _socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
				}
				return stream.ToArray();
			}
		}

		public void Dispose() {
			_socket.Close();
			_socket?.Dispose();
		}
	}

	public class HttpResult {
		public int Code { get; private set; } = -1;
		public string Header { get; private set; } = null;
		public string Content { get; private set; } = null;
		public int ContentLength { get; private set; } = -1;

		public readonly Dictionary<string, string> Headers = new Dictionary<string, string>();

		private HttpResult() { }

		internal static HttpResult Create(string raw) {
			var result = new HttpResult() {
				Header = raw
			};
			string[] sp = raw.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in sp) {
				if (!line.Contains(": ")) {
					string[] ls = line.Split(' ');
					if (ls.Length > 0) {
						result.Code = int.Parse(ls[1]);
					}
				} else {
					string[] ls = line.Split(new string[] { ": " }, StringSplitOptions.None);
					if (ls.Length > 1) {
						result.Headers.Add(ls[0], ls[1]);
						if (ls[0] == "content-length") {
							result.ContentLength = int.Parse(ls[1]);
						}
					}
				}
			}
			return result;
		}

		internal void SetContent(string content) {
			Content = content;
		}
	}
}
