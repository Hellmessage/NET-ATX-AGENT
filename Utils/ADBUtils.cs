using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace HAtxLib.Utils {
	public class ADBUtils {
		private readonly Socket _socket;
		private readonly string _serial;
		private const string DefaultEncoding = "ISO-8859-1";
		private readonly static string OKAY = "OKAY";
		private readonly static string FAIL = "FAIL";
		public static int ReceiveBufferSize { get; set; } = 40960;
		public static Encoding Encoding { get; } = Encoding.GetEncoding(DefaultEncoding);

		private static bool ServerRunning = false;
		private readonly static object _lock = new object();
		public static void StartServer() {
			lock(_lock) {
				if (!ServerRunning) {
					Cmd("adb start-server");
					ServerRunning = true;
				}
			}
		}

		public ADBUtils(string serial) {
			_serial = serial;
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_socket.Connect("127.0.0.1", 5037);
		}

		public int ForwardPort(int remote) {
			var list = ForwardList();
			foreach (var item in list) {
				if (item.Remote == remote) {
					return item.Local;
				}
			}
			return -1;
		}

		public List<ForwardItem> ForwardList() {
			var list = new List<ForwardItem>();
			var result = Command("list-forward");
			if (string.IsNullOrWhiteSpace(result)) {
				return list;
			}
			var sp = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in sp) {
				var ls = line.Split(' ');
				if (ls.Length != 3) {
					continue;
				}
				var item = new ForwardItem() {
					Serial = ls[0],
					Local = int.TryParse(ls[1].Split(':')[1], out int local) ? local : -1,
					Remote = int.TryParse(ls[2].Split(':')[1], out int remote) ? remote : -1,
				};
				list.Add(item);
			}
			return list;
		}

		//public string Command(params object[] argv) {
		//	string command = string.Join(":", argv);
		//	string resultStr = string.Format("{0}{1}", command.Length.ToString("X4"), command);

		//}

		public string Command(string command) {
			command = $"host-serial:{_serial}:{command}";
			string resultStr = string.Format("{0}{1}", command.Length.ToString("X4"), command);
			byte[] result = Encoding.GetBytes(resultStr);
			_socket.Send(result);
			CheckOkay();
			string reply = ReadToEnd();
			return reply;
		}

		private void CheckOkay() {
			byte[] buf = new byte[4];
			_socket.Receive(buf, 0, 4, SocketFlags.None);
			string state = Encoding.GetString(buf);
			if (state == OKAY) {
				return;
			} else if (state == FAIL) {
				throw new ADBException($"FAIL: {ReadToEnd()}");
			} else {
				throw new ADBException("未知错误");
			}
		}

		private string ReadToEnd() {
			byte[] reply = new byte[4];
			var message = Read(reply);
			if (message == 0) {
				return null;
			}
			string lenHex = Encoding.GetString(reply);
			int len = int.Parse(lenHex, NumberStyles.HexNumber);
			reply = new byte[len];
			Read(reply);
			string value = Encoding.GetString(reply);
			return value;
		}

		private int Read(byte[] data) {
			return Read(data, data.Length);
		}

		private int Read(byte[] data, int length) {
			int expLen = length != -1 ? length : data.Length;
			int count = -1;
			int totalRead = 0;
			while (count != 0 && totalRead < expLen) {
				try {
					int left = expLen - totalRead;
					int len = left < ReceiveBufferSize ? left : ReceiveBufferSize;
					byte[] buffer = new byte[len];
					count = _socket.Receive(buffer, len, SocketFlags.None);
					if (count < 0) {
						throw new ADBException("EOF");
					} else if (count == 0) {
						Console.WriteLine("DONE with Read");
					} else {
						Array.Copy(buffer, 0, data, totalRead, count);
						totalRead += count;
					}
				} catch (SocketException sex) {
					throw new ADBException(string.Format("No Data to read: {0}", sex.Message));
				}
			}
			return totalRead;
		}

		public void Push(string file, string path) {
			ADBCmd($"push {file} {path}");
		}

		public string Shell(params object[] argv) {
			string command = string.Join(" ", argv);
			return ADBCmd($"shell {command}");
		}

		private string ADBCmd(string cmd) {
			return Cmd($"adb -s {_serial} {cmd}");
		}

		public static string Cmd(string cmd) {
			ProcessStartInfo info = new ProcessStartInfo() {
				FileName = "cmd.exe",
				Arguments = $"/c \"{cmd}\"",
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true,
			};
			using (Process process = Process.Start(info)) {
				string output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				if (!string.IsNullOrWhiteSpace(output)) {
					return output;
				}
				return $"{output}";
			}
		}

		public class ForwardItem {
			public string Serial;
			public int Local;
			public int Remote;
		}
	}


	


	public class ADBException : Exception {
		public ADBException(string message) : base(message) { }
	}
}
