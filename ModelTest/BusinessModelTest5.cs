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
using Sunup.App;
using Sunup.Diagnostics;

namespace ModelTest
{
   public class PlatformModelTest5 : IAppContainerDataChange
    {

        public void OnDataChange(List<VTQ> changedObject)
        {
            foreach (var item in changedObject)
            {
                Console.WriteLine("Print Node Data: "+item.Id +":"+ item.Value );
            }
        }

        public void Run()
        {
            Logger.EnableLogTrace = true;
            AppContainer business = new AppContainer("businessdemo.json");
            business.AddContainerDataChange(this);
            business.Run();
        }

    }
}
