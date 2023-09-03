using HAtxLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Demo {
    internal class Program {
        static void Main(string[] args) {
            ServicePointManager.DefaultConnectionLimit = 1024;
            HAtx atx = new HAtx("5557344542583398");
            //Console.WriteLine(atx.DumpHierarchy());
            Console.WriteLine(atx.AtxAgentUrl);
            Console.WriteLine($"IsAlive: {atx.IsAlive()}");
            
            while (true) {
                Console.ReadLine();
                Console.WriteLine(atx.DeviceInfo().CurrentPackageName);
            }
        }
    }
}
