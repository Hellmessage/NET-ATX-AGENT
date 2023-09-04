using HAtxLib.ADB;
using HAtxLib.Catch;
using HAtxLib.UIAutomator;
using HAtxLib.UIAutomator.Model;
using HAtxLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.UI.WebControls.WebParts;

namespace HAtxLib {

	public class HAtx {
		private readonly string _serial;
		private readonly ADBClient _client;
		private int _port = -1;
		private string _url = null;
		private bool _debug = false;

		public string AtxAgentUrl {
			get {
				if (string.IsNullOrWhiteSpace(_url)) {
					if (!Connect()) {
						throw new ATXException("Connect Error");
					}
				}
				return _url;
			}
		}

		public string AtxAgentWs {
            get {
                if (_port == -1) {
                    if (!Connect()) {
                        throw new ATXException("Connect Error");
                    }
                }
                return $"ws://127.0.0.1:{_port}";
            }
        }

		private UIAutomatorService UIService {
			get {
				return new UIAutomatorService(this);
			}
		}

		public void Test() {

		}

		public HAtx(string serial) {
			_serial = serial;
			_client = new ADBClient(_serial);
			Initer initer = new Initer(_client);
			initer.Install();
			Console.WriteLine($"HAtx<{_serial}> Connect: {Connect()}");
			HRuntime.Run("运行 UIAUTOMATOR", () => Console.WriteLine($"HAtx<{_serial}> RunUiautomator: {RunUiautomator()}"));
			//_client.Shell("am", "start", "-W", "-n", "com.github.uiautomator/.IdentifyActivity", "-e", "theme", "black");
		}

		public void SetDebug() {
			_debug = true;
		}

		private bool Connect() {
			int port = _client.ForwardPort(7912);
			if (port == -1) {
				return false;
			}
			_port = port;
			_url = $"http://127.0.0.1:{port}";
			return true;
		}

		public string DumpHierarchy() {
			HttpWebResponse response = HApi.Get($"{_url}/dump/hierarchy");
			if (response == null) {
				return null;
			}
			Console.WriteLine(response.StatusCode);
			if (response.StatusCode == HttpStatusCode.OK) {
				string data = response.DecodeDefault();
				Console.WriteLine("DUMP: " + data);
				JObject json = JObject.Parse(data);
				return json.Value<string>("result");
			}
			return null;
		}

		public UADeviceInfo DeviceInfo() {
			var json = JsonRpc("deviceInfo");
			if (json == null) {
				return null;
			}
			if (json.Error != null) {
				Console.WriteLine($"DeviceInfo: {json.Error.ToString(Formatting.None)}");
				return null;
			}
			
			JObject data = (JObject)json.Data;
			return JsonConvert.DeserializeObject<UADeviceInfo>(data.ToString());
		}

		public bool IsAlive() {
			var device = DeviceInfo();
			if (device == null) {
				return false;
			}
			return true;
		}

		public void SetOrientation(Orientation orientation) {
			var argv = OrientationDict[orientation];
			JsonRpc("setOrientation", argv[1]);
		}

		public void FreezeRotation(bool freezed = true) {
			JsonRpc("freezeRotation", freezed);
		}

		public bool ElementExists(By by, int wait = 2000) {
			var result = JsonRpc("waitForExists", by, wait);
			if (result == null) {
				throw new ATXException("ElementExists Fail");
			}
			if (result.Error != null) {
				throw new ATXException($"ElementExists Fail {result.Error.ToString(Formatting.None)}");
			}
			if (result.Data is bool) {
				return (bool)result.Data;
			} else {
				return false;
			}
		}

		public JsonRpcResponse JsonRpc(string method, params object[] argv) {
			return HRuntime.Run($"JSONRPC<{method}>", () => {
				string url = $"{_url}/jsonrpc/0";
				JArray array = new JArray();
				foreach (var obj in argv) {
					if (obj is By) {
						array.Add((obj as By).ToJson());
						continue;
					}
					array.Add(obj);
				}
				string id = Guid.NewGuid().ToString().Replace("-", "");
				JObject json = new JObject {
					{ "jsonrpc", "2.0" },
					{ "id", id },
					{ "method", method },
					{ "params", array }
				};
				if (_debug) {
					Console.WriteLine($"JsonRpc >>> {json.ToString(Formatting.None)}");
				}
				try {
					using (var socket = HSocket.Create(_url)) {
						string data = socket.Post("/jsonrpc/0", json);
						if (_debug) {
							Console.WriteLine($"JsonRpc <<< {data}");
						}
						if (string.IsNullOrWhiteSpace(data)) {
							return null;
						}
						JsonRpcResponse result = JsonConvert.DeserializeObject<JsonRpcResponse>(data);
						return result;
					}
				} catch (Exception) {
					return null;
				}
			});

		}

		#region 启动UIAutomator

		private void GrantAppPermissions() {
			var argv = new string[] {
				"pm",
				"grant",
				"com.github.uiautomator",
				"android.permission.SYSTEM_ALERT_WINDOW",
				"android.permission.ACCESS_FINE_LOCATION",
				"android.permission.READ_PHONE_STATE"
			};
			_client.Shell(argv);
		}

		private bool RunUiautomator(int timeout = 20) {
			//if (IsAlive()) {
			//	return true;
			//}
			if (UIService.Running()) {
				return true;
			}
			Console.WriteLine($"Uiautomator Service Stop: {UIService.Stop()}");
			GrantAppPermissions();
			var argv = new string[] {
				"am",
				"start",
				"-a",
				"android.intent.action.MAIN",
				"-c",
				"android.intent.category.LAUNCHER",
				"-n",
				"com.github.uiautomator/.ToastActivity",
			};
			Console.WriteLine($"RunUiautomator: {_client.Shell(argv)}");
			Console.WriteLine($"Uiautomator Service Start: {UIService.Start()}");
			Thread.Sleep(500);
			Console.WriteLine($"Uiautomator Running: {UIService.Running()}");
			while (timeout-- > 0) {
				if (!UIService.Running()) {
					continue;
				}
				if (IsAlive()) {
					return true;
				}
				Thread.Sleep(1000);
			}
			UIService.Stop();
			string result = _client.Shell("am instrument -w -r -e debug false -e class com.github.uiautomator.stub.Stub com.github.uiautomator.test/android.support.test.runner.AndroidJUnitRunner");
			if (result.Contains("does not have a signature matching the target")) {
				Initer initer = new Initer(_client);
				initer.SetupAtxApp();
			}
			return false;
		}


		#endregion

		#region 初始化安装类

		internal class Initer {
			private readonly ADBClient _client;
			private readonly string _abi;
			private readonly string _sdk;

			private readonly static string ATX_APP_VERSION = "2.3.3";
			private readonly static string ATX_AGENT_VERSION = "0.10.0";

			private readonly static ReaderWriterLockSlim DownLock = new ReaderWriterLockSlim();
			private readonly static string CACHE_PATH = $"{AppDomain.CurrentDomain.BaseDirectory}/cache/";
			private readonly static string ATX_LISTEN_ADDR = "127.0.0.1:7912";
			private readonly static string GITHUB_BASEURL = "https://github.com/openatx";
			private readonly static string GITHUB_DOWN_APK_PATH = "/android-uiautomator-server/releases/download/";
			private readonly static string GITHUB_DOWN_AGENT_PATH = "/atx-agent/releases/download/";
			private readonly static string ANDROID_LOCAL_TMP_PATH = "/data/local/tmp/";
			private readonly static string ATX_AGENT_PATH = "/data/local/tmp/atx-agent";
			private readonly static string[] ATX_APKS = new string[2] { "app-uiautomator", "app-uiautomator-test" };
			private readonly static Dictionary<string, string> ATX_AGENT_FILE_DICT = new Dictionary<string, string>() {
				{ "armeabi-v7a", "atx-agent_{0}_linux_armv7.tar.gz" },
				{ "arm64-v8a", "atx-agent_{0}_linux_arm64.tar.gz" },
				{ "armeabi", "atx-agent_{0}_linux_armv6.tar.gz" },
				{ "x86", "atx-agent_{0}_linux_386.tar.gz" },
				{ "x86_64", "atx-agent_{0}_linux_386.tar.gz" },
			};
			private string ATX_AGENT_DOWN_URL {
				get {
					if (_abi == null) {
						throw new ATXIniterException("CPU not exists");
					}
					if (!ATX_AGENT_FILE_DICT.ContainsKey(_abi)) {
						throw new ATXIniterException("CPU not support");
					}
					string file = ATX_AGENT_FILE_DICT[_abi];
					return $"{GITHUB_BASEURL}{GITHUB_DOWN_AGENT_PATH}{ATX_AGENT_VERSION}/{string.Format(file, ATX_AGENT_VERSION)}";
				}
			}
			private string ATX_AGENT_CAHCE_FILE {
				get {
					if (_abi == null) {
						throw new ATXIniterException("CPU not exists");
					}
					if (!ATX_AGENT_FILE_DICT.ContainsKey(_abi)) {
						throw new ATXIniterException("CPU not support");
					}
					string file = string.Format(ATX_AGENT_FILE_DICT[_abi], ATX_AGENT_VERSION);
					return $"{CACHE_PATH}atx_agent/{ATX_AGENT_VERSION}/{file}";
				}
			}

			public Initer(ADBClient client) {
				_client = client;
				_abi = _client.GetProp("ro.product.cpu.abi");
				_sdk = _client.GetProp("ro.build.version.sdk");
			}

			#region atx-agent
			public void SetupAtxAgent() {
				_client.KillProcessByName("atx-agent");
				_client.Shell(ATX_AGENT_PATH, "server", "--stop");
				if (IsAtxAgentOutdated()) {
					GithubDown(ATX_AGENT_DOWN_URL, ATX_AGENT_CAHCE_FILE);
					string file = Path.GetDirectoryName(ATX_AGENT_CAHCE_FILE) + $"\\{_abi}\\atx-agent";
					if (!File.Exists(file)) {
						HZip.UnzipTgz(ATX_AGENT_CAHCE_FILE, Path.GetDirectoryName(file));
					}
					_client.Push(file, ATX_AGENT_PATH);
				}
				_client.Shell(ATX_AGENT_PATH, "server", "--nouia", "-d", "--addr", ATX_LISTEN_ADDR);
				int size = 10;
				while (!CheckAtxAgentVersion()) {
					size--;
					if (size <= 0) {
						throw new ATXIniterException("Init atx-agent fail");
					}
					Thread.Sleep(500);
				}
				Console.WriteLine($"SetupAtxAgent: True");
			}

			public bool CheckAtxAgentVersion() {
				int port = _client.ForwardPort(7912);
				if (port == -1) {
					return false;
				}
				string url = $"http://127.0.0.1:{port}/version";
				HttpWebResponse response = HApi.Get(url);
				if (response == null) {
					return false;
				}
				string data = response.DecodeDefault();
				if (string.IsNullOrWhiteSpace(data)) {
					return false;
				}
				return true;
			}


			private bool IsAtxAgentOutdated() {
				try {
					string version = _client.Shell(ATX_AGENT_PATH, "version");
                    Console.WriteLine($"AtxAgent version: {version}");
                    if (version == "dev") {
						return false;
					}
					var nv = ATX_AGENT_VERSION.Split('.');
					var ov = version.Split('.');
					if (nv[1] != ov[1]) {
						return true;
					}
					return int.Parse(ov[2]) < int.Parse(nv[2]);
				} catch (Exception ex) {
					Console.WriteLine($"123 {ex}");
					return true;
				}
			}
			#endregion

			#region atx-app
			public void SetupAtxApp() {
				if (IsAtxAppOutdated()) {
					_client.Shell("pm", "uninstall", "com.github.uiautomator");
					_client.Shell("pm", "uninstall", "com.github.uiautomator.test");
					foreach (string app in ATX_APKS) {
						string tmp = $"{ANDROID_LOCAL_TMP_PATH}{app}.apk";
						_client.Shell("rm", tmp);
						string url = $"{GITHUB_BASEURL}{GITHUB_DOWN_APK_PATH}{ATX_APP_VERSION}/{app}.apk";
						string file = $"{CACHE_PATH}apk/{ATX_APP_VERSION}/{app}.apk";
						GithubDown(url, file);
						Console.WriteLine($"{tmp}: {_client.Push(file, tmp, 420/*644*/)}");
						Console.WriteLine($"{tmp} Install: {_client.Shell("pm", "install", "-r", "-t", tmp)}");
					}
				}

			}

			public bool IsAtxAppOutdated() {
				var apk_debug = _client.AppInfo("com.github.uiautomator");
				var apk_debug_test = _client.AppInfo("com.github.uiautomator.test");
				if (apk_debug == null || apk_debug_test == null) {
					return true;
				}
				if (apk_debug.VersionName != ATX_APP_VERSION) {
					return true;
				}
				if (apk_debug.Signature != apk_debug_test.Signature) {
					return true;
				}
				return false;
			}
			#endregion

			#region minicap
			public void SetupMinicap() {
				if (_abi == "x86") {
					Console.WriteLine("abi:x86 not supported well, skip install minicap");
					return;
				}
				if (int.Parse(_sdk) > 30) {
					Console.WriteLine("Android R (sdk:30) has no minicap resource");
					return;
				}
				string base_url = $"{GITHUB_BASEURL}/stf-binaries/raw/0.3.0/node_modules/@devicefarmer/minicap-prebuilt/prebuilt/";
				string so_url = $"{base_url}{_abi}/lib/android-{_sdk}/minicap.so";
				string minicap_url = $"{base_url}{_abi}/bin/minicap";
				string so_file = $"{CACHE_PATH}minicap/{_abi}/minicap.so";
				string minicap_file = $"{CACHE_PATH}minicap/{_abi}/minicap";
				GithubDown(so_url, so_file);
				GithubDown(minicap_url, minicap_file);
				Console.WriteLine($"{ANDROID_LOCAL_TMP_PATH}minicap: {_client.Push(minicap_file, $"{ANDROID_LOCAL_TMP_PATH}minicap")}");
				Console.WriteLine($"{ANDROID_LOCAL_TMP_PATH}minicap.so: {_client.Push(so_file, $"{ANDROID_LOCAL_TMP_PATH}minicap.so")}");
			}
			#endregion

			#region minitouch
			public void SetupMinitouch() {
				string base_url = $"{GITHUB_BASEURL}/stf-binaries/raw/0.3.0/node_modules/@devicefarmer/minitouch-prebuilt/prebuilt/{_abi}/bin/minitouch";
				string minitouch_file = $"{CACHE_PATH}minitouch/{_abi}/minitouch";
				GithubDown(base_url, minitouch_file);
				Console.WriteLine($"{ANDROID_LOCAL_TMP_PATH}minitouch: {_client.Push(minitouch_file, $"{ANDROID_LOCAL_TMP_PATH}minitouch")}");
			}
			#endregion

			#region 安装/卸载/重装
			public void Install() {
				SetupMinitouch();
				SetupMinicap();
				SetupAtxApp();
				SetupAtxAgent();
			}

			public void Reinstall(bool clear = false) {
				if (clear) {
					if (Directory.Exists(CACHE_PATH)) {
						try {
							Directory.Delete(CACHE_PATH, true);
						} catch (Exception ex) {
							Console.WriteLine($"Clear cache path exception: {ex}");
						}
					}
				}
				Uninstall();
				Install();
			}

			public void Uninstall() {
				_client.Shell(ATX_AGENT_PATH, "server", "--stop");
				_client.Shell("rm", ATX_AGENT_PATH);
				_client.Shell("rm", $"{ANDROID_LOCAL_TMP_PATH}minicap");
				_client.Shell("rm", $"{ANDROID_LOCAL_TMP_PATH}minicap.so");
				_client.Shell("rm", $"{ANDROID_LOCAL_TMP_PATH}minitouch");
				foreach (string app in ATX_APKS) {
					_client.Shell("rm", $"{ANDROID_LOCAL_TMP_PATH}{app}.apk");
				}
				_client.Shell("pm", "uninstall", "com.github.uiautomator");
				_client.Shell("pm", "uninstall", "com.github.uiautomator.test");
			}
			#endregion

			private void GithubDown(string url, string file) {
				DownLock.EnterWriteLock();
				string path = Path.GetDirectoryName(file);
				if (!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
				if (File.Exists(file)) {
					Thread.Sleep(1000);
					DownLock.ExitWriteLock();
					return;
				}
				using (var client = new WebClient()) {
					client.Headers.Add("user-agent", "Hell");
					client.DownloadFile(url, file);
				}
				DownLock.ExitWriteLock();
			}
		}

		#endregion

		#region 屏幕方向
		private readonly static Dictionary<Orientation, object[]> OrientationDict = new Dictionary<Orientation, object[]>() {
			{ Orientation.Natural, new object[] { 0, "natural", "n", 0 } },
			{ Orientation.Left, new object[] { 1, "left", "l", 90 } },
			{ Orientation.Upsidedown, new object[] { 2, "upsidedown", "u", 180 } },
			{ Orientation.Right, new object[] { 3, "right", "r", 270 } }
		};

		public enum Orientation {
			Natural,
			Left,
			Upsidedown,
			Right
		}
		#endregion
	}
}
