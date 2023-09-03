using HAtxLib;
using HAtxLib.UIAutomator;
using HAtxLib.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace Demo {
    internal class Program {
        static void Main(string[] args) {
			HAtx atx = new HAtx("5557344542583398");
			atx.SetDebug();
			atx.Test();

            var data = HRuntime.Run("测试 SOCKET", () => {
				HSocket socket = HSocket.Create(atx.AtxAgentUrl);
				JObject json = JObject.Parse("{\"jsonrpc\":\"2.0\",\"id\":\"fefb22f3a51249b587de33696bd1f317\",\"method\":\"waitForExists\",\"params\":[{\"childOrSibling\":[],\"childOrSiblingSelector\":[],\"resourceId\":\"com.android.systemui:id/home\",\"mask\":2097152},0]}");
                return socket.Post("/jsonrpc/0", json);
			});

            Console.WriteLine(data);
			
			//watch.Stop();
			//Console.WriteLine($"{watch.Elapsed}");

			//Console.ReadLine();
			//return;

			Console.WriteLine(atx.AtxAgentUrl);

            Console.WriteLine(atx.DumpHierarchy());

            //Console.WriteLine($"IsAlive: {atx.IsAlive()}");

            //atx.SetOrientation(HAtx.Orientation.Natural);
            //atx.FreezeRotation();

            //atx.JsonRpc("waitForExists", By.ResourceId("com.android.systemui:id/home", By.Text("聊天功能")), 10);

            //Console.WriteLine($"EEEE {atx.ElementExists(By.ResourceId("com.android.systemui:id/home"), 0)}");

            //while (true) {
            //    Console.ReadLine();
            //    Console.WriteLine(atx.DeviceInfo().CurrentPackageName);
            //}
        }


        
    }
}
