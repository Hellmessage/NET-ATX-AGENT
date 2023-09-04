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

			HAtx atx = new HAtx("304155554f363098");
			Console.WriteLine(atx.AtxAgentUrl);
			atx.SetDebug();
			//atx.DumpHierarchy();
			//atx.DumpWindowHierarchy();
			
			atx.FreezeRotation(false);

			//atx.ElementExists(By.ResourceId("com.android.systemui:id/home"), 1000);
			Console.WriteLine(atx.Click(By.Xpath("//*[@resource-id=\"com.sec.android.daemonapp:id/widget_empty_icon\"]")));
			//var data = HRuntime.Run("测试 SOCKET", () => {
			//	HSocket socket = HSocket.Create(atx.AtxAgentUrl);
			//	JObject json = JObject.Parse("{\"jsonrpc\":\"2.0\",\"id\":\"fefb22f3a51249b587de33696bd1f317\",\"method\":\"waitForExists\",\"params\":[{\"childOrSibling\":[],\"childOrSiblingSelector\":[],\"resourceId\":\"com.android.systemui:id/home\",\"mask\":2097152},0]}");
			//	return socket.Get("/services/uiautomator");
			//});
			//Console.WriteLine(data);

			//Console.WriteLine($"EEEE {atx.ElementExists(By.Text("Chrome"), 1000)}");

			Thread.Sleep(1000);
			Console.ReadLine();
			
		}
	}
}
