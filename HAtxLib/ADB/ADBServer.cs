using HAtxLib.Utils;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace HAtxLib.ADB {
	public class ADBServer {
		public readonly static string ADB_PATH = $"{AppDomain.CurrentDomain.BaseDirectory}/{Properties.Resources.CACHE_PATH}adb/adb.exe";
		private readonly static string ADB_DOWN_URL = "https://dl.google.com/android/repository/platform-tools-latest-windows.zip";
		
		public static void StartServer() {
			InitADB();
			HProcess.Run($"{ADB_PATH} start-server");
		}

		public static void Restart() {
			HProcess.Run($"{ADB_PATH} kill-server");
			HProcess.Run($"{ADB_PATH} start-server");
		}

		private static void InitADB() {
			string path = Path.GetDirectoryName(ADB_PATH);
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			if (!File.Exists(ADB_PATH)) {
				string zip = $"{path}/adb.zip";
				string tp = $"{path}/platform-tools";
				if (Directory.Exists(tp)) {
					Directory.Delete(tp, true);
				}
				GithubDown(ADB_DOWN_URL, zip);
				ZipFile.ExtractToDirectory(zip, path, Encoding.UTF8);
				foreach (string file in Directory.GetFiles(tp)) {
					File.Move(file, $"{path}/{Path.GetFileName(file)}");
				}
				Directory.Delete(tp);
			}
		}

		#region 下载
		private static void GithubDown(string url, string file) {
			try {
				string path = Path.GetDirectoryName(file);
				if (!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
				if (File.Exists(file)) {
					return;
				}
				HRuntime.Run("下载任务", () => {
					using (var client = new WebClient()) {
						client.Headers.Add("user-agent", "Hell");
						client.DownloadFile(url, file);
					}
				});
				Console.WriteLine($"DOWN {file}: 完成");
			} catch (Exception e) {
				if (File.Exists(file)) {
					File.Delete(file);
				}
				Console.WriteLine($"DOWN {file}: 失败");
				throw e;
			}
		}
		#endregion
	}
}
