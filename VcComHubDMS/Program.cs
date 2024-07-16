using System;

namespace VcComHubDMS
{
    internal class Program
    {
        public static void Main()
        {
            Console.Title = "VcComHub Dedicated master server v1.0 by https://github.com/yungrixxxi";
            Console.WriteLine("Starting VcComHub Dedicated master server..");
            WebServer.RunTask();
        }
    }
}
