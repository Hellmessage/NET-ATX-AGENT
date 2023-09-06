using HAtxLib.ADB.Entity;
using HAtxLib.Extend;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HAtxLib.ADB {
    public class ADBClient {
		private readonly string _serial;
		public ADBClient(string serial) {
			_serial = serial;
		}
		
		public int ForwardPort(int remote) {
            using (ADBSocket adb = ADBSocket.Create(_serial)) {
                int port = adb.ForwardPort(remote);
				return port;
            }
        }

		public string Shell(params object[] argv) {
             return ADBSocket.Shell(_serial, argv);
        }

		public AndroidAppInfo AppInfo(string package) {
			string result = Shell("pm", "path", package);
			if (string.IsNullOrWhiteSpace(result) || !result.StartsWith("package:")) {
				return null;
			}
            string[] lines = result.Split('\n');
			AndroidAppInfo info = new AndroidAppInfo() {
				PackageName = package,
			};
			string apk_path = lines[0].Split(':')[1].Strip();
			info.Path = apk_path;
			List<string> sub_apk_path = info.SubApkPaths;
			if (lines.Length > 1) {
				for (int i = 0; i < lines.Length; i++) {
					sub_apk_path.Add(lines[i].Split(':')[1].Strip());
				}
			}
            info.Dumpsys = Shell("dumpsys", "package", package);
			if (string.IsNullOrWhiteSpace(info.Dumpsys)) {
				return null;
			}
            Match match = new Regex("versionName=(?<name>[^\\s]+)").Match(info.Dumpsys);
			if (match.Success) {
				info.VersionName = match.Groups["name"].Value;
			}
            match = new Regex("versionCode=(?<code>\\d+)").Match(info.Dumpsys);
            if (match.Success) {
                info.VersionCode = match.Groups["code"].Value;
            }
            match = new Regex("PackageSignatures\\{.*?\\[(?<signatures>.*?)\\]").Match(info.Dumpsys);
            if (match.Success) {
                info.Signature = match.Groups["signatures"].Value;
            }
            match = new Regex("firstInstallTime=(?<time>[-\\d]+\\s[:\\d]+)").Match(info.Dumpsys);
            if (match.Success) {
                info.FirstInstallTime = match.Groups["time"].Value;
            }
            match = new Regex("lastUpdateTime=(?<time>[-\\d]+\\s[:\\d]+)").Match(info.Dumpsys);
            if (match.Success) {
                info.LastUpdateTime = match.Groups["time"].Value;
            }
            match = new Regex("pkgFlags=\\[(?<flags>\\s.*?\\s*)\\]").Match(info.Dumpsys);
            if (match.Success) {
                info.Flags = match.Groups["flags"].Value.Strip();
            }
			return info;
		}

		public List<AndroidProcessItem> GetProcessList() {
			List<AndroidProcessItem> list = new List<AndroidProcessItem>();
			string result = ADBSocket.Shell(_serial, "ps;", "ps", "-A");
			if (string.IsNullOrWhiteSpace(result)) {
				return list;
			}
			var pids = new List<string>();
			string[] sp = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in sp) {
				string[] info = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				if (info.Length < 3) {
					continue;
				}
				if (info[0] == "USER") {
					continue;
				}
				if (pids.Contains(info[1])) {
					continue;
				}
				pids.Add(info[1]);
				list.Add(new AndroidProcessItem() {
					User = info[0],
					Pid = int.TryParse(info[1], out int pid) ? pid : -1,
					Name = info[info.Length - 1]
				});
			}
			return list;
		}

		public void KillProcessByName(string name) {
			var list = GetProcessList();
			foreach (var item in list) {
				if (item.Name.Equals(name) && item.User == "shell") {
					ADBSocket.Shell(_serial, "kill", "-9", item.Pid);
				}
			}
		}

		public string GetProp(string porp) {
			return ADBSocket.Shell(_serial, "getprop", porp);
        }

		public bool Push(string file, string path, int mode = 493) {
            //MODE -> 755
            return ADBSocket.Push(_serial, file, path, mode);
		}

		public bool IsScreenOn() {
			return Shell("dumpsys", "power").Contains("mHoldingDisplaySuspendBlocker=true");
		}

		public List<string> AppList(string filter = "") {
			List<string> list = new List<string>();
			var result = Shell("pm", "list", "packages", filter);
			if (!string.IsNullOrWhiteSpace(result)) {
				string[] sp = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string app in sp) {
					if (app.StartsWith("package:")) {
						string line = app.Split(':')[1];
						list.Add(line);
					}
				}
			}
			return list;
		}

		public List<string> AppRunningList() {
			List<string> apps = AppList();
			List<AndroidProcessItem> process = GetProcessList();
			List<string> list = new List<string>();
			foreach (string app in apps) {
				foreach (var item in process) {
					if (item.Name.Contains(app)) {
						list.Add(app);
						break;
					}
				}
			}
			return list;
		}

	}
}
