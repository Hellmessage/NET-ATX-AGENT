// See https://aka.ms/new-console-template for more information
using HAtxLib;

HAtx atx = new HAtx("5557344542583398");
//Console.WriteLine(atx.DumpHierarchy());
Console.WriteLine(atx.AtxAgentUrl);
Console.WriteLine($"IsAlive: {atx.IsAlive()}");
Console.WriteLine(atx.DeviceInfo().CurrentPackageName);


Console.ReadLine();