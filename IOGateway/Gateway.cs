// <copyright file="Gateway.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOGateway
{
    using System;
    using System.IO.Ports;
    using System.Text;
    using System.Timers;
    using Microsoft.Extensions.Configuration;
    using Modbus.Device;
    using Modbus.IO;
    using Sunup.Diagnostics;

    /// <summary>
    /// Program.
    /// </summary>
    internal class Gateway
    {
        ////private static MQTTOptions options;
        private static string publishTopic;
        private static string clientId;
        private static Timer executionTimer;
        private static MQTTClient client;
        private static SerialPort serialPort;
        private static IModbusMaster master;
        private static byte slaveAddress;
        private static ushort startAddress;
        private static ushort numberOfPoints;
        private static ushort loopInterval;

        /// <summary>
        ///  Start.
        /// </summary>
        /// <param name="configuration">args.</param>
        public static void Start(IConfiguration configuration)
        {
            try
            {
                Initalize(configuration);
            }
            catch (Exception ex)
            {
                Logger.LogError("[IOGateway] Initalize >> Exception happen : ", ex);
            }

            Logger.LogInfo($"[IOGateway]Start looping.");
            StartLoopingTimer();
        }

        private static void Execute()
        {
            if (serialPort.IsOpen == false)
            {
                serialPort.Open();
            }

            Logger.LogTrace($"[IOGateway Host]Read holding registers.");
            var start = DateTime.Now;
            var registerBuffer = master.ReadHoldingRegisters(slaveAddress, startAddress, numberOfPoints);
            Logger.LogTrace($"[IOGateway Host]Read holding registers done.");
            StringBuilder json = new StringBuilder("\"id\":\"" + clientId + "\"");
            int count = registerBuffer.Length;
            for (var i = 0; i < count; i++)
            {
                json.Append(",\"" + (i + startAddress) + "\":" + registerBuffer[i]);
            }

            var publishedStr = "{" + json.ToString() + "}";
            ////Console.WriteLine("Looping ReadHoldingRegisters" + "{" + json.ToString() + "}");
            Logger.LogTrace($"[IOGateway Host]Ready to publish:" + publishedStr);
            var end = DateTime.Now;
            client.Publish(publishTopic, publishedStr);
            Console.WriteLine("Time spent : " + (end - start).TotalSeconds);
        }

        private static void Initalize(IConfiguration configuration)
        {
            Logger.LogInfo($"[IOGateway] Create Serial port.");
            var portName = configuration.GetValue<string>("SerialPort:Port");
            Logger.LogInfo($"[IOGateway]SerialPort.Port : " + portName);
            var baudRate = configuration.GetValue<int>("SerialPort:BaudRate");
            Logger.LogInfo($"[IOGateway]SerialPort.BaudRate : " + baudRate);
            var parity = configuration.GetValue<int>("SerialPort:Parity");
            Logger.LogInfo($"[IOGateway]SerialPort.Parity : " + parity);
            var dataBits = configuration.GetValue<int>("SerialPort:DataBits");
            Logger.LogInfo($"[IOGateway]SerialPort.DataBits : " + dataBits);
            var stopBits = configuration.GetValue<int>("SerialPort:StopBits");
            Logger.LogInfo($"[IOGateway]SerialPort.StopBits : " + stopBits);

            var portArray = SerialPort.GetPortNames();
            foreach (var port in portArray)
            {
               Logger.LogInfo($"[IOGateway]Port : " + port);
            }

            serialPort = new SerialPort(portName, baudRate, (Parity)parity, dataBits, (StopBits)stopBits);

            Logger.LogInfo($"[IOGateway]Create Modbus.");
            slaveAddress = configuration.GetValue<byte>("Modbus:SlaveAddress");
            Logger.LogInfo($"[IOGateway]Modbus.SalveAddress : " + slaveAddress);
            startAddress = configuration.GetValue<ushort>("Modbus:AddressOfStartPoint");
            Logger.LogInfo($"[IOGateway]Modbus.StartAddress : " + startAddress);
            numberOfPoints = configuration.GetValue<ushort>("Modbus:NumberOfPoints");
            Logger.LogInfo($"[IOGateway]Modbus.NumberOfPoints : " + numberOfPoints);
            loopInterval = configuration.GetValue<ushort>("Modbus:LoopInterval");
            Logger.LogInfo($"[IOGateway]Modbus.LoopInterval : " + loopInterval);
            var checksums = configuration.GetValue<bool>("Modbus:Checksums");
            Logger.LogInfo($"[IOGateway]Modbus.Checksums : " + checksums);
            var readTimeout = configuration.GetValue<ushort>("Modbus:ReadTimeout");
            Logger.LogInfo($"[IOGateway]Modbus.ReadTimeout : " + readTimeout);
            master = ModbusSerialMaster.CreateRtu(serialPort);
            master.Transport.ReadTimeout = readTimeout;
            var transport = master.Transport as ModbusSerialTransport;
            if (transport != null)
            {
                transport.CheckFrame = checksums;
            }

            Logger.LogInfo($"[IOGateway] Create Modbus.");

            var options = new MQTTOptions();
            options.Server = configuration.GetValue<string>("MQTT:Server");
            options.Port = configuration.GetValue<int>("MQTT:Port");
            options.User = configuration.GetValue<string>("MQTT:User");
            options.Password = configuration.GetValue<string>("MQTT:Password");
            options.Quality = configuration.GetValue<int>("MQTT:Quality");
            options.Retained = configuration.GetValue<bool>("MQTT:Retained");
            options.Mode = configuration.GetValue<string>("MQTT:Mode");

            clientId = configuration.GetValue<string>("MQTT:ClientId");
            publishTopic = configuration.GetValue<string>("MQTT:PublishTopic");

            client = new MQTTClient(clientId, options, null);
            client.Run();
            Logger.LogInfo($"[IOGateway] Run Mqtt client.");
        }

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
    }
}
