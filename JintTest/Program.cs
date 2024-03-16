// See https://aka.ms/new-console-template for more information
using Jint;
using JintTest;
using System;

Console.WriteLine("Hello, World!");


var p = new Root
{
};
p["设备模型1"] = new Node
{
    Name = "设备模型1",
    Value = "13"
};

p["设备模型1"]["设备模型2"] = new Node
{
    Name = "设备模型1",
    Value = "133"
};

var engine = new Engine()
    .SetValue("$Sunup", p)
    .Execute("$Sunup.设备模型1.设备模型2.Value = '13333'");

var script = Engine.PrepareScript("$Sunup.设备模型1.设备模型2.Value = '13333'");
//engine.Evaluate(script);
Console.ReadLine();
