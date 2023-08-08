using System;
using System.Text;

namespace ModelTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var modelTest = new PlatformModelTest();
            //modelTest.Run();
            //Console.OutputEncoding = System.Text.Encoding.UTF8;
            //Encoding.Default.WebName
            //Console.WriteLine(Console.OutputEncoding.WebName);
            var modelTest = new PlatformModelTest5();
            modelTest.Run();
            Console.ReadLine();
        }
    }
}
