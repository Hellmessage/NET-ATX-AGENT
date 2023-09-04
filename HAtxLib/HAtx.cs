using HAtxLib.ADB;
using HAtxLib.Catch;
using HAtxLib.UIAutomator;
using HAtxLib.UIAutomator.Model;
using HAtxLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace HAtxLib {

	public class HAtx {
		private readonly string _serial;
		private readonly ADBClient _client;
		private int _port = -1;
		private string _url = null;
		private bool _debug = false;

		public int ElementMaxWaitTime { get; set; } = 2000;
		// 检测中延时
		public int ElementClickExistDelay { get; set; } = 60;
		// 点击是延迟
		public int ElementClickDelay { get; set; } = 200;

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

		public HAtx(string serial) {
			_serial = serial;
			_client = new ADBClient(_serial);
			Initer initer = new Initer(_client);
			while (true) {
				try {
					initer.Install();
					break;
				} catch (Exception) {
					continue;
				}
			}
			Console.WriteLine($"HAtx<{_serial}> Connect: {Connect()}");
			HRuntime.Run("运行 UIAUTOMATOR", () => Console.WriteLine($"HAtx<{_serial}> RunUiautomator: {RunUiautomator()}"));
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

		#region 手机显示信息页面
		/// <summary>
		/// 手机显示信息页面
		/// </summary>
		public void ShowInfo() {
			_client.Shell("am", "start", "-W", "-n", "com.github.uiautomator/.IdentifyActivity", "-e", "theme", "black");
		}
		#endregion

		#region DUMP屏幕
		/// <summary>
		/// DUMP屏幕
		/// </summary>
		public string DumpHierarchy() {
			return HRuntime.Run("屏幕DUMP", () => {
				using (HSocket socket = HSocket.Create(_url)) {
					var result = socket.HttpGet("/dump/hierarchy");
					if (result == null || result.Code != 200) {
						return null;
					}
					JObject json = JObject.Parse(result.Content);
					return json.Value<string>("result");
				}
			});
		}

		/// <summary>
		/// DUMP屏幕(DumpWindowHierarchy)
		/// </summary>
		public string DumpWindowHierarchy(bool compressed = false) {
			var result = JsonRpc("dumpWindowHierarchy", compressed, null);
			if (result == null || result.Data == null) {
				return null;
			}
			if (result.Data is string xml) {
				return xml;
			}
			return null;
		}

		#endregion

		#region 设备信息
		/// <summary>
		/// 设备信息
		/// </summary>
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
		#endregion

		#region 是否在线
		/// <summary>
		/// 判断是否在线
		/// </summary>
		public bool IsAlive() {
			int size = 10;
			while (size-- > 0) {
				var device = DeviceInfo();
				if (device == null) {
					Thread.Sleep(500);
					continue;
				}
				return true;
			}
			return false;
		}
		#endregion

		#region 设置屏幕方向
		/// <summary>
		/// 设置屏幕方向
		/// </summary>
		public void SetOrientation(Orientation orientation) {
			JsonRpc("setOrientation", OrientationDict[orientation][1]);
		}
		#endregion

		#region 锁定屏幕
		/// <summary>
		/// 锁定屏幕方向 
		/// True 自动 False 锁定
		/// </summary>
		public void FreezeRotation(bool freezed = true) {
			JsonRpc("freezeRotation", freezed);
		}
		#endregion

		#region 判断元素是否存在
		/// <summary>
		/// 检测元素是否存在
		/// </summary>
		/// <exception cref="ATXException"></exception>
		public bool ElementExists(By by, int wait = -1) {
			if (wait == -1) {
				wait = ElementMaxWaitTime;
			}
			if (by.Mask == ByMask.Xpath) {
				string xml = DumpWindowHierarchy();
				bool exist = by.XpathToAndroid(xml);
				if (!exist) {
					Thread.Sleep(ElementClickExistDelay);
				}
				return exist;
			}
			var result = JsonRpc("waitForExists", by, wait) ?? throw new ATXException("ElementExists Fail");
			if (result.Error != null) {
				throw new ATXElementException("ElementExists Fail", result.Error);
			}
			return result.Data is bool v && v;
		}
		#endregion

		#region 元素点击

		public bool Click(By by, int wait = -1) {
			if (wait == -1) {
				wait = ElementMaxWaitTime;
			}
			while (wait > 0) {
				bool exist = HRuntime.Time(() => {
					return ElementExists(by, 60);
				}, out int time);
				wait -= time;
				if (exist) {
					Thread.Sleep(ElementClickDelay);
					if (by.Mask == ByMask.Xpath) {
						by.XpathToAndroid(DumpWindowHierarchy());
						return Click(by.Pos.X, by.Pos.Y);
					}
				}
			}
			return false;
		}

		public bool Click(params int[] pos) {
			var result = JsonRpc("click", pos);
			if (result == null) {
				return false;
			}
			if (result.Error != null) {
				throw new ATXElementException("Click fail", result.Error);
			}
			return result.Data is bool s && s;
		}

		#endregion

		#region 按下按钮
		/// <summary>
		/// 按下按钮
		/// </summary>
		public void Press(string key) {
			JsonRpc("pressKey", key);
		}

		/// <summary>
		/// 按下按钮
		/// </summary>
		public void Press(PressKey key) {
			JsonRpc("pressKey", PressKeyDict[key]);
		}
		#endregion

		#region JSONRPC基函数
		/// <summary>
		/// JSONRPC基函数
		/// </summary>
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
						var result = socket.HttpPost("/jsonrpc/0", json);
						if (_debug) {
							Console.WriteLine($"JsonRpc <<< {result?.Content}");
						}
						if (result == null) {
							return null;
						}
						if (string.IsNullOrWhiteSpace(result.Content)) {
							return null;
						}
						return JsonConvert.DeserializeObject<JsonRpcResponse>(result.Content);
					}
				} catch (Exception) {
					return null;
				}
			});
		}
		#endregion

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
			bool service = UIService.Running();
			if (IsAlive() && service) {
				return true;
			}
			if (service) {
				Console.WriteLine($"Uiautomator Service Stop: {UIService.Stop()}");
			}
			Thread.Sleep(1000);
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
				Thread.Sleep(500);
				if (IsAtxAgentOutdated()) {
					GithubDown(ATX_AGENT_DOWN_URL, ATX_AGENT_CAHCE_FILE);
					string file = Path.GetDirectoryName(ATX_AGENT_CAHCE_FILE) + $"\\{_abi}\\atx-agent";
					if (!File.Exists(file)) {
						HZip.UnzipTgz(ATX_AGENT_CAHCE_FILE, Path.GetDirectoryName(file));
					}
					Console.WriteLine($"PUSH {file}: {_client.Push(file, ATX_AGENT_PATH)}");
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
				using (HSocket socket = HSocket.Create("127.0.0.1", port)) {
					var result = socket.HttpGet("/version");
					if (result == null || result.Code != 200) {
						return false;
					}
					return true;
				}
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
				} catch (Exception) {
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
						Console.WriteLine($"PUSH {tmp}: {_client.Push(file, tmp, 420)}");
						Console.WriteLine($"INSTALL {tmp}: {_client.Shell("pm", "install", "-r", "-t", tmp)}");
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
				string result = _client.Shell("ls", "-a", "/data/local/tmp");
				var list = new List<string>(result.Split(' '));
				if (!list.Contains("minicap.so")) {
					string so_url = $"{base_url}{_abi}/lib/android-{_sdk}/minicap.so";
					string so_file = $"{CACHE_PATH}minicap/{_abi}/minicap.so";
					GithubDown(so_url, so_file);
					Console.WriteLine($"PUSH {ANDROID_LOCAL_TMP_PATH}minicap.so: {_client.Push(so_file, $"{ANDROID_LOCAL_TMP_PATH}minicap.so")}");
				}
				if (!list.Contains("minicap")) {
					string minicap_url = $"{base_url}{_abi}/bin/minicap";
					string minicap_file = $"{CACHE_PATH}minicap/{_abi}/minicap";
					GithubDown(minicap_url, minicap_file);
					Console.WriteLine($"PUSH {ANDROID_LOCAL_TMP_PATH}minicap: {_client.Push(minicap_file, $"{ANDROID_LOCAL_TMP_PATH}minicap")}");
				}
			}
			#endregion

			#region minitouch
			public void SetupMinitouch() {
				string result = _client.Shell("ls", "-a", "/data/local/tmp");
				var list = new List<string>(result.Split(' '));
				if (!list.Contains("minitouch")) {
					string base_url = $"{GITHUB_BASEURL}/stf-binaries/raw/0.3.0/node_modules/@devicefarmer/minitouch-prebuilt/prebuilt/{_abi}/bin/minitouch";
					string minitouch_file = $"{CACHE_PATH}minitouch/{_abi}/minitouch";
					GithubDown(base_url, minitouch_file);
					Console.WriteLine($"PUSH {ANDROID_LOCAL_TMP_PATH}minitouch: {_client.Push(minitouch_file, $"{ANDROID_LOCAL_TMP_PATH}minitouch")}");
				}
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
				_client.KillProcessByName("atx-agent");
				_client.Shell(ATX_AGENT_PATH, "server", "--stop");
				Thread.Sleep(1000);
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

			#region 下载
			private void GithubDown(string url, string file) {
				DownLock.EnterWriteLock();
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
				} catch (Exception) {
					if (File.Exists(file)) {
						File.Delete(file);
					}
					Console.WriteLine($"DOWN {file}: 失败");
				} finally {
					DownLock.ExitWriteLock();
				}
			}
			#endregion
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

		#region PressKey(按键)
		private readonly static Dictionary<PressKey, string> PressKeyDict = new Dictionary<PressKey, string>() {
			{ PressKey.Home, "home" },
			{ PressKey.Back, "back" },
			{ PressKey.Left, "left" },
			{ PressKey.Right, "right" },
			{ PressKey.Up, "up" },
			{ PressKey.Down, "down" },
			{ PressKey.Center, "center" },
			{ PressKey.Menu, "menu" },
			{ PressKey.Search, "search" },
			{ PressKey.Enter, "enter" },
			{ PressKey.Delete, "delete" },
			{ PressKey.Recent, "recent" },
			{ PressKey.VolumeUp, "volume_up" },
			{ PressKey.VolumeDown, "volume_down" },
			{ PressKey.VolumeMute, "volume_mute" },
			{ PressKey.Camera, "camera" },
			{ PressKey.Power, "power" },
		};

		public enum PressKey {
			Home,
			Back,
			Left,
			Right,
			Up,
			Down,
			Center,
			Menu,
			Search,
			Enter,
			Delete,
			Recent,
			VolumeUp,
			VolumeDown,
			VolumeMute,
			Camera,
			Power
		}
		#endregion
	}
}
