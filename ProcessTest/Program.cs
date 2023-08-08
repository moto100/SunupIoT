using System;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var argss = @" Urls:http://localhost:8001 BasePath:E:\Sunup\bin\Debug\netcoreapp3.1\deployedApps\ee8f2d40-cf31-df18-8709-21eae4f7386c";
            var path = @"E:\Sunup\bin\Debug\netcoreapp3.1\aa\Sunup.IOServerHost.exe";
            StartProcess(path, argss);
            Console.ReadKey();
        }

        /// <summary>
        /// StartClient.
        /// </summary>
        /// <param name="args">args.</param>
        /// <returns>true.</returns>
        public static bool StartProcess(string path, string args)
        {
            try
            {
                var appPath = path;
                var procInfo = new ProcessStartInfo()
                {
                    Arguments = args,
                    FileName = appPath,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                };
                Process proc = Process.Start(procInfo);
                proc.OutputDataReceived += (sender, data) =>
                {
                    Console.WriteLine("[Control Panel] After start process, OutputDataReceived " + data.Data);
                };

                proc.StartInfo.RedirectStandardError = true;

                proc.ErrorDataReceived += (sender, data) =>
                {
                    Console.WriteLine("[Control Panel] After start process, ErrorDataReceived " + data.Data);
                };

                if (proc == null)
                {
                    Console.WriteLine("[Control Panel] Failed to run app.\r\n app args is " + args);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Control Panel] Failed to run app.\r\n app args is " + args, ex);
                return false;
            }
        }
    }
}
