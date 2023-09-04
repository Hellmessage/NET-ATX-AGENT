using HAtxLib.Extend;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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

		public string Get(string path) {
			string raw = $"GET {path} HTTP/1.1\r\n" +
				"User-Agent: Hell\r\n" +
				"Accept: */*\r\n" +
				$"Host: {_host}:{_port}\r\n" +
				"Connection: close\r\n\r\n";
			byte[] bytes = Encoding.UTF8.GetBytes(raw);
			_socket.Send(bytes);
			byte[] header = ReadEndWith("\r\n\r\n");
			raw = Encoding.UTF8.GetString(header).ToLower();
			var headers = raw.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			int length = -1;
			foreach (string line in headers) {
				if (line.StartsWith("content-length: ")) {
					length = int.Parse(line.Split(':')[1].Replace(" ", ""));
				}
			}
			return (length > 0 ? Encoding.UTF8.GetString(ReadLength(length)) : ReadToClose()).Strip();
		}

		public string Delete(string path) {
			string raw = $"DELETE {path} HTTP/1.1\r\n" +
				"User-Agent: Hell\r\n" +
				"Accept: */*\r\n" +
				$"Host: {_host}:{_port}\r\n" +
				"Connection: close\r\n\r\n";
			byte[] bytes = Encoding.UTF8.GetBytes(raw);
			_socket.Send(bytes);
			byte[] header = ReadEndWith("\r\n\r\n");
			raw = Encoding.UTF8.GetString(header).ToLower();
			var headers = raw.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			int length = -1;
			foreach (string line in headers) {
				if (line.StartsWith("content-length: ")) {
					length = int.Parse(line.Split(':')[1].Replace(" ", ""));
				}
			}
            return (length > 0 ? Encoding.UTF8.GetString(ReadLength(length)) : ReadToClose()).Strip();
        }

		public string Post(string path, JObject json) {
			string postdata = "";
			if (json != null) {
				postdata = json.ToString(Newtonsoft.Json.Formatting.None);
			}
			string raw = $"POST {path} HTTP/1.1\r\n" +
				"User-Agent: Hell\r\n" +
				"Content-Type: application/json; charset=UTF-8\r\n" +
				"Accept: */*\r\n" +
				$"Host: {_host}:{_port}\r\n" +
				"Connection: close\r\n" +
				$"Content-Length: {Encoding.UTF8.GetBytes(postdata).Length}\r\n\r\n" +
				$"{postdata}";
			byte[] bytes = Encoding.UTF8.GetBytes(raw);
			_socket.Send(bytes);
			byte[] header = ReadEndWith("\r\n\r\n");
			raw = Encoding.UTF8.GetString(header).ToLower();
			var headers = raw.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			int length = -1;
			foreach (string line in headers) {
				if (line.StartsWith("content-length: ")) {
					length = int.Parse(line.Split(':')[1].Replace(" ", ""));
				}
			}
            return (length > 0 ? Encoding.UTF8.GetString(ReadLength(length)) : ReadToClose()).Strip();
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
}
