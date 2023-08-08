// <copyright file="Program2.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOGateway
{
    using System;
    using System.IO;
    using System.IO.Ports;
    using System.Runtime.InteropServices;
    using System.Timers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Modbus.Device;
    using Sunup.Diagnostics;

    /// <summary>
    /// Program.
    /// </summary>
    internal class Program2
    {
        private static MQTTOptions options;
        private static string publishTopic;
        private static string clientId;
        private static string urls;
        private static Timer executionTimer;
        private static MQTTClient client;
        private static SerialPort serialPort;
        private static IModbusMaster master;
        private static byte slaveAddress;
        private static ushort startAddress;
        private static ushort numberOfPoints;
        private static ushort loopInterval;

        private static void Main1(string[] args)
        {
            InitalizeConfig();
            CreateHostBuilder(args).Build().Run();
            Logger.LogInfo($"[IOGateway] Initalize Config.");

            client = new MQTTClient(clientId, options, null);
            ////client.Run();
            Logger.LogInfo($"[IOGateway] Run Mqtt client.");

            master = ModbusSerialMaster.CreateRtu(serialPort);

            Logger.LogInfo($"[IOGateway] Create Modbus Rtu.");

            ////StartLoopingTimer();

            Logger.LogInfo($"[IOGateway] Start looping.");

            Logger.LogInfo($"[IOGateway] Press \"Enter\" to exit.");
            Console.ReadLine();
        }

        private static void Execute()
        {
            if (serialPort.IsOpen == false)
            {
                serialPort.Open();
            }

            Console.WriteLine("Looping ReadHoldingRegisters");
            ////var start = DateTime.Now;
            var registerBuffer = master.ReadHoldingRegisters(slaveAddress, startAddress, numberOfPoints);

            Console.WriteLine("Looping ReadHoldingRegisters done.");
            string json = "\"id\":\"" + clientId + "\"";
            int count = registerBuffer.Length;
            for (var i = 0; i < count; i++)
            {
                json += ",\"" + i + "\":" + registerBuffer[i];
            }

            Console.WriteLine("Looping ReadHoldingRegisters" + "{" + json + "}");

            ////var end = DateTime.Now;
            client.Publish(publishTopic, "{" + json + "}");
            ////Console.WriteLine("Time spent : " + (end - start).TotalSeconds);
        }

        private static void InitalizeConfig()
        {
            var path = Directory.GetCurrentDirectory();
            Console.WriteLine("Current path : " + path);
            ////var factory = LoggerFactory.Create(builder =>
            ////{
            ////    builder.ClearProviders();
            ////    builder.AddLog4Net(Path.Combine(path, "log4net.config"));
            ////});

            ////var logger = factory.CreateLogger("Sunup");
            ////Logger.MSLogger = logger;

            var builder = new ConfigurationBuilder()
             .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            urls = configuration.GetValue<string>("appSettings:ListenOn");
            options = new MQTTOptions();
            options.Server = configuration.GetValue<string>("MQTT:Server");
            options.Port = configuration.GetValue<int>("MQTT:Port");
            options.User = configuration.GetValue<string>("MQTT:User");
            options.Password = configuration.GetValue<string>("MQTT:Password");
            options.Quality = configuration.GetValue<int>("MQTT:Quality");
            options.Retained = configuration.GetValue<bool>("MQTT:Retained");
            options.Mode = configuration.GetValue<string>("MQTT:Mode");

            clientId = configuration.GetValue<string>("MQTT:ClientId");
            publishTopic = configuration.GetValue<string>("MQTT:PublishTopic");

            var portName = configuration.GetValue<string>("SerialPort:Port");
            Console.WriteLine("SerialPort:Port : " + portName);
            var baudRate = configuration.GetValue<int>("SerialPort:BaudRate");
            Console.WriteLine("SerialPort:BaudRate : " + baudRate);
            var parity = configuration.GetValue<int>("SerialPort:Parity");
            Console.WriteLine("SerialPort:Parity : " + parity);
            var dataBits = configuration.GetValue<int>("SerialPort:DataBits");
            Console.WriteLine("SerialPort:DataBits : " + dataBits);
            var stopBits = configuration.GetValue<int>("SerialPort:StopBits");
            Console.WriteLine("SerialPort:StopBits : " + stopBits);
            serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);

            slaveAddress = configuration.GetValue<byte>("Modbus:SlaveAddress");
            startAddress = configuration.GetValue<ushort>("Modbus:StartAddress");
            numberOfPoints = configuration.GetValue<ushort>("Modbus:NumberOfPoints");
            loopInterval = configuration.GetValue<ushort>("Modbus:LoopInterval");
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            if (!string.IsNullOrEmpty(urls))
            {
                var urlsArr = urls.Split(",");
                if (urlsArr.Length > 0)
                {
                    webBuilder.UseUrls(urlsArr);
                            ////Logger.LogInfo($"[IO Server Host]Server will listen on {Urls}.");
                 }
                else
                {
                    webBuilder.UseUrls("http://*:5000");
                            ////Logger.LogInfo($"[IO Server Host]Server will listen on default url http://*:5000.");
                }
            }
            else
            {
                webBuilder.UseUrls("http://*:5000");
                        ////Logger.LogInfo($"[IO Server Host]Server will listen on default url http://*:5000.");
             }

            webBuilder.UseStartup<Startup>();
        })
        .ConfigureLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddLog4Net();
                    //// Write eventlog
                    ////builder.AddEventLog(eventLogSettings =>
                    ////{
                    ////    eventLogSettings.SourceName = "MyLogs";
                    ////});
                });

        private static void StartLoopingTimer()
        {
            if (executionTimer == null)
            {
                var interval = loopInterval;
                executionTimer = new Timer(interval);
                executionTimer.Elapsed += (sender, e) =>
                {
                    executionTimer.Enabled = false;
                    try
                    {
                        Execute();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Looping exception : " + ex.StackTrace);
                        Logger.LogError("[IOGateway] StartExecutionTimer >> Calling Execute() Exception ", ex);
                    }

                    executionTimer.Enabled = true;
                };

                executionTimer.Start();
            }
        }

        private static void StopLoopingTimer()
        {
            if (executionTimer != null)
            {
                executionTimer.Stop();
                executionTimer.Dispose();
                executionTimer = null;
            }
        }

        //////private static string GetRuntimeDirectory(string path)
        //////{
        //////    ////ForLinux
        //////    if (IsLinuxRunTime())
        //////    {
        //////        return GetLinuxDirectory(path);
        //////    }
        //////    ////ForWindows
        //////    if (IsWindowRunTime())
        //////    {
        //////        return GetWindowDirectory(path);
        //////    }

        //////    return path;
        //////}

        //////////OSPlatform.Windows监测运行环境
        //////private static bool IsWindowRunTime()
        //////{
        //////    return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        //////}

        //////////OSPlatform.Linux运行环境
        //////private static bool IsLinuxRunTime()
        //////{
        //////    return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        //////}

        //////private static string GetLinuxDirectory(string path)
        //////{
        //////    string pathTemp = Path.Combine(path);
        //////    return pathTemp.Replace("\\", "/");
        //////}

        //////private static string GetWindowDirectory(string path)
        //////{
        //////    string pathTemp = Path.Combine(path);
        //////    return pathTemp.Replace("/", "\\");
        //////}
    }
}
