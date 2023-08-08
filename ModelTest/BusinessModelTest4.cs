using Sunup.ScriptExecutor;
using Sunup.Contract;
using System;
using Sunup.PlatformModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using Sunup.DeviceModel;
using System.Timers;
using Sunup.DataSource.MQTT;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sunup.DataSource;

namespace ModelTest
{
   public class PlatformModelTest4 : IDataChange,IAppContainerDataChange
    {
        public void OnDataChange()
        {
            //throw new NotImplementedException();
        }

        public void OnDataChange(List<VTQ> changedObject)
        {
            foreach (var item in changedObject)
            {
                Console.WriteLine(item.Id +":"+ item.Value );
            }
        }

        private void BuildNodes(string prefix, JToken token, NodeContainer businessContainer)
        {
            foreach (var node in token)
            {
                var name = node.Value<string>("Name");
                var prefixWithName = prefix + "." + name;
                var dataValueTypeInt = node.Value<int>("DataValueType");

                DataType dataValueType  = (DataType)dataValueTypeInt;
                object dataValue = null;

                switch (dataValueType)
                {
                    case DataType.Integer:
                        dataValue = node.Value<int>("DataValue");
                        break;
                    case DataType.String:
                        dataValue = node.Value<string>("DataValue");
                        break;
                    case DataType.Bool:
                        dataValue = node.Value<bool>("DataValue");
                        break;
                }
                var expression = node.Value<string>("Expression");
                var script = node.Value<string>("Script");

                businessContainer.AddNode(prefixWithName, new DataValue(dataValue, dataValueType), expression, script);

                var nodes = node["Nodes"];
                if (nodes != null)
                {
                    BuildNodes(prefixWithName, nodes, businessContainer);
                }
            }
        }
        private Device CreateMQTTDevice(JToken deviceJson)
        {
            var deviceName = deviceJson.Value<string>("Name");
            var optionsJson = deviceJson["Options"];
            var itemsJson = deviceJson["Items"];
            var deviceParseJson = deviceJson.Value<string>("DeviceParse") ;

            MQTTOptions options = new MQTTOptions();
            options.Server = optionsJson.Value<string>("Server");
            options.Port = optionsJson.Value<int>("Port"); 
            options.User = optionsJson.Value<string>("User");
            options.Password = optionsJson.Value<string>("Password");
            options.Quality = optionsJson.Value<int>("Quality");
            options.Retained = optionsJson.Value<bool>("Retained");

            List<string> items = new List<string>();
            if (itemsJson != null)
            {
                foreach (var item in itemsJson)
                {
                    items.Add(item.Value<string>());
                }
            }
            MQTTClient dataSource = new MQTTClient(deviceName + "_Source", options, items.ToArray());
            var device = new Device(deviceName);

            device.SetDataSource(dataSource);
            if (deviceParseJson != null)
            {
                device.InParser = new Sunup.ScriptLibrary.Script(deviceParseJson);
            }
            return device;
        }

        public void Run()
        {
            var businessContainer = new NodeContainer("Demo1", this, new DevicesProxy());

            var nodeContainer = ReadConfiguration();

            var nodes = nodeContainer["Nodes"];

            if (nodes != null)
            {
                BuildNodes("Root", nodes, businessContainer);
            }

            var devicesJson = nodeContainer["Devices"];

            if (devicesJson != null)
            {
                Device device =null;
                foreach (var deviceJson in devicesJson)
                {
                   var deviceType = deviceJson.Value<int>("DeviceType");
                   DataSourceType dataSourceType = (DataSourceType)deviceType;
                    switch (dataSourceType)
                    {
                        case DataSourceType.MQTT:
                          device = CreateMQTTDevice(deviceJson);
                            break;
                    }

                    if (device != null)
                    {
                        businessContainer.DevicesProxy.AddDevice(device);
                    }
                }
            }

            businessContainer.Build();
            businessContainer.Run();

            //MQTTOptions options = new MQTTOptions();
            //options.Server = "";
            //options.Port = 6601;
            //options.User = "testuser";
            //options.Password = "nichai";
            //options.Quality = 2;
            //options.Retained = false;
            //MQTTSource dataSource = new MQTTSource("MqttTest", options, new string[] { "/asset1/temprature" });
            //var device1 = new Device("Device1");


            //device1.SetDataSource(dataSource);
            //device1.Script = new Sunup.ScriptLibrary.Script(
            //    @" 

            //    var jsonSet = JSON.parse(DataSource.DataSet);
            //    var tempratureString= jsonSet['/asset1/temprature'];
            //    var temprature = parseInt(tempratureString);
            //    //var pressure= jsonSet.pressure + 10;
            //    Device.SetField('temprature', temprature);
            //    //Device.SetField('pressure', pressure);

            //    "
            //    );



            //BusinessContainer businessContainer = new BusinessContainer("Demo1", this);
            //businessContainer.DevicesProxy.AddDevice(device1);
            //businessContainer.AddNode("Root",  new DataValue(0));
            //businessContainer.AddNode("Root.设备模型1",  new DataValue(0));

            //businessContainer.AddNode("Root.设备模型1.设备模型2",  new DataValue(0),
            //    " DevicesProxy.GetData(Root.设备模型1.设备模型2.设备模型3.Value,'temprature') + 100",
            //    @" 
            //        Root.设备模型1.Value = DevicesProxy.GetData('Device1','temprature');

            //    ");

            //businessContainer.AddNode("Root.设备模型1.设备模型2.设备模型3", new DataValue("Device1"));

            //businessContainer.Build();

            ////dataSource.DataSourceReference.DataSet = "{\"temprature\" : 12,\"pressure\": 20}";

            //businessContainer.Run();
            ////dataSource.Run();

        }


        public JObject ReadConfiguration()
        {
            var filePath = $"business.json";

            if (File.Exists(filePath))
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    var json = r.ReadToEnd();
                    JObject jo = JsonConvert.DeserializeObject<JObject>(json);
                    return jo;
                }
            }
            return null;

        }
    }
}
