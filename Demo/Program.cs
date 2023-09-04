using HAtxLib;
using HAtxLib.Extend;
using HAtxLib.UIAutomator;
using HAtxLib.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Demo {
	internal class Program {
		static void Main(string[] args) {

			HAtx atx = new HAtx("98897a394f41334c4c");
			atx.SetDebug();
			atx.Test();
			//Console.WriteLine(atx.DumpHierarchy());
			Console.WriteLine(atx.AtxAgentUrl);
			var data = HRuntime.Run("测试 SOCKET", () => {
				HSocket socket = HSocket.Create(atx.AtxAgentUrl);
				JObject json = JObject.Parse("{\"jsonrpc\":\"2.0\",\"id\":\"fefb22f3a51249b587de33696bd1f317\",\"method\":\"waitForExists\",\"params\":[{\"childOrSibling\":[],\"childOrSiblingSelector\":[],\"resourceId\":\"com.android.systemui:id/home\",\"mask\":2097152},0]}");
				return socket.Get("/services/uiautomator");
			});
			Console.WriteLine(data);
			Console.WriteLine($"EEEE {atx.ElementExists(By.ResourceId("com.android.systemui:id/home"), 1000)}");
			Console.WriteLine($"EEEE {atx.ElementExists(By.Text("Chrome"), 1000)}");

			Thread.Sleep(1000);
			//watch.Stop();
			//Console.WriteLine($"{watch.Elapsed}");

			Console.ReadLine();
			//return;





			//Console.WriteLine($"IsAlive: {atx.IsAlive()}");

			//atx.SetOrientation(HAtx.Orientation.Natural);
			//atx.FreezeRotation();

			//atx.JsonRpc("waitForExists", By.ResourceId("com.android.systemui:id/home", By.Text("聊天功能")), 10);

			

			//while (true) {
			//    Console.ReadLine();
			//    Console.WriteLine(atx.DeviceInfo().CurrentPackageName);
			//}
		}
	}
}
