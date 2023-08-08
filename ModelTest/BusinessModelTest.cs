using Sunup.ScriptExecutor;
using Sunup.Contract;
using System;
using Sunup.PlatformModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using Sunup.DeviceModel;
using System.Timers;

namespace ModelTest
{
   public class PlatformModelTest : IDataChange, IAppContainerDataChange
    {
        public void OnDataChange()
        {
            //throw new NotImplementedException();
        }

        public void OnDataChange(List<VTQ> changedObject)
        {
            foreach (var item in changedObject)
            {
                Console.WriteLine(item.Id + ":" + item.Value);
            }
        }

        public void Run()
        {
            
            var dataSource = new Sunup.DataSource.DataSource("DataSource1");
            var device1 = new Device("Device1");
           

            device1.SetDataSource(dataSource);
            device1.InParser = new Sunup.ScriptLibrary.Script(
                @" 

                var jsonSet = JSON.parse(DataSource.DataSet);
                var temprature= jsonSet.temprature;
                var pressure= jsonSet.pressure + 10;
                Device.SetField('temprature', temprature);
                Device.SetField('pressure', pressure);

                "
                );



            var businessContainer = new NodeContainer("Demo1", this, new DevicesProxy());
            businessContainer.DevicesProxy.AddDevice(device1);
            businessContainer.AddNode("Root",  new DataValue(0));
            businessContainer.AddNode("Root.设备模型1",  new DataValue(0));
            
            businessContainer.AddNode("Root.设备模型1.设备模型2", new DataValue(0),
                " DevicesProxy.GetData(Root.设备模型1.设备模型2.设备模型3.Value,'temprature') + 100", 
                @" 
                    Root.设备模型1.Value = DevicesProxy.GetData('Device1','pressure');
  
                ", this);

            businessContainer.AddNode("Root.设备模型1.设备模型2.设备模型3",  new DataValue("Device1"));

            businessContainer.Build();

            dataSource.DataSourceReference.DataSet = "{\"temprature\" : 12,\"pressure\": 20}";

            businessContainer.Run();
            //dataSource.Run();


            // change  data source
            //device1.RemoveDataSource();

            //var dataSource2 = new Sunup.DataSource.DataSource("MQTT2");
            //device1.SetDataSource(dataSource2);
            //dataSource2.DataSourceReference.DataSet = 13;
            //dataSource2.Run();

            //// change device
            //businessContainer.DevicesProxy.RemoveDevice("Device1");
            //var device2 = new Device("Device2");

            //businessContainer.DevicesProxy.AddDevice(device2);
            //device2.SetDataSource(dataSource);
            //device2.Script = new Sunup.ScriptLibrary.Script(
            //    @" 
            //    var val = DataSource.DataSet +10;
            //    Device.SetField('temprature', val);"
            //    );

            //dataSource.DataSourceReference.DataSet = 14;

            //dataSource.Run();
            //int n = 0;
            //n = n + 10;
            //device1.SetField("temprature", n + 20);
            //device1.SetField("pressure", n + 30);
            //device1.Run();

            //n = n + 10;
            //device1.SetField("temprature", n + 20);
            //device1.SetField("pressure", n + 30);
            //device1.Run();

            //var interval = 10000;
            //var timer = new Timer(interval);
            //int n = 0;
            //timer.Elapsed += (sender, e) =>
            //{
            //    n = n + 10;
            //    device1.SetField("temprature",n+20);
            //    device1.SetField("pressure",n+30);
            //    device1.Run();
            //};

            //timer.Start();

            //devicePorxy.AddDevice(new Sunup.DeviceModel.Device("Device1"));
            //Dictionary<string, IReference> nodes = new Dictionary<string, IReference>();
            //var nodeA = new BusinessNode("Root", new DataValue(0));
            //var nodeB = new BusinessNode("Root__设备模型1", new DataValue(10));
            //var nodeC = new BusinessNode("Root__设备模型1__设备模型2", new DataValue(10));

            //nodes.Add(nodeA.ReferenceName, nodeA);
            //nodes.Add(nodeB.ReferenceName, nodeB);
            //nodes.Add(nodeC.ReferenceName, nodeC);


            //string transformedScript = null;
            //var script = "     Root . 设备模型1  . Value + 100;";
            ////var script = "     Devices.GetData('Device1','temp') + 100;";
            //ScriptHelper scriptHelper = new ScriptHelper(script);
            //var referenceList = scriptHelper.TransformScript(nodes, out transformedScript);
            //referenceList.Add(devicePorxy);

            //nodeC.Expression = new Sunup.ScriptLibrary.Expression(transformedScript, new DataValue(0));

            //referenceList.ForEach(x => {
            //    x.AddNotification(nodeC.Expression);
            //});

            //nodeC.Expression.ReferenceList = referenceList;

            //script = " Devices.GetData('Device1','temp')";
            //scriptHelper = new ScriptHelper(script);
            //referenceList = scriptHelper.TransformScript(nodes, out transformedScript);
            //referenceList.Add(devicePorxy);
            //nodeC.Script = new Sunup.ScriptLibrary.Script(script);
            //nodeC.Script.ReferenceList = referenceList;


            //nodeC.AddNotification(this);

            //nodeC.Expression.Run();

        }
    }
}
