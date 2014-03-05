using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoUpdaterDotNET;
using System.IO;

namespace UpdateBootstrap
{
    class Program
    {
        static void Main(string[] args)
        {
            //File.AppendAllText("error.txt", "In " + args.Length);

            if (args.Length == 5)
            {
                var url = args[0];
                var appName = args[1];
                var appLoc = args[2];
                var appCompany = args[3];
                var version = args[4];
                //File.AppendAllText("error.txt", "In " + url + " " + appName + " " + appLoc);
                AutoUpdater.Start(url, appName, appLoc, appCompany, version);
                
            }
            //else
            //{
            //    File.AppendAllText("error.txt", "Args Len : " + args.Length);
            //}
        }
    }
}
