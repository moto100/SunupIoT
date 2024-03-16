using Sunup.DataSource.MQTT;
using System;

namespace MqttTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            MQTTOptions options = new MQTTOptions();
            options.Server = "";
            options.Port = 6601;
            options.User = "testuser";
            options.Password = "nichai";
            options.Quality = 2;
            options.Retained = false;
            MQTTClient mQTTSource = new MQTTClient("MqttTest", options, new string[]{"/AAA/BBB" });

            mQTTSource.Start();

            System.Threading.Thread.Sleep(1000);
            mQTTSource.Publish("/AAA/CCC", "cheshi1");
            System.Threading.Thread.Sleep(1000);
            mQTTSource.Publish("/AAA/CCC", "cheshi2");
            System.Threading.Thread.Sleep(1000);
            mQTTSource.Publish("/AAA/CCC", "cheshi3");
            System.Threading.Thread.Sleep(1000);
            mQTTSource.Publish("/AAA/CCC", "cheshi4");
            System.Threading.Thread.Sleep(1000);
            mQTTSource.Publish("/AAA/CCC", "cheshi5");
            System.Threading.Thread.Sleep(1000);
            mQTTSource.Publish("/AAA/CCC", "cheshi6");
            System.Threading.Thread.Sleep(1000);

           // mQTTSource.Stop();
            Console.ReadLine();
        }
    }
}
