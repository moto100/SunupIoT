using Microsoft.Data.SqlClient;
using System;
using System.Reflection;
using System.Security.Cryptography;

namespace DBTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Assembly assembly = Assembly.LoadFile("D:\\Sunup\\DBTest\\bin\\Debug\\netcoreapp3.1\\runtimes\\win\\lib\\netcoreapp2.1\\System.Data.SqlClient.dll");
            Assembly assembly2 = Assembly.LoadFile("D:\\Sunup\\bin\\Debug\\net7.0\\Microsoft.Data.SqlClient.dll");
            var connection1 = new SqlConnection("server=DESKTOP-UJ7IABI; database=Sunup; uid=SunupAdmin; pwd=SunupAdmin; TrustServerCertificate=True;")
                ;
            connection1.Open();
            //var connection = new SqlConnection("data source=DESKTOP-UJ7IABI;initial catalog=Sunup;integrated security=false;persist security info=True;User ID=SunupAdmin;Password=SunupAdmin");
            //connection.Open();
        }
    }
}
