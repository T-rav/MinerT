using System.Windows;
using AutoUpdaterDotNET;
using System.Threading;

namespace UpdateBootstraper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 5)
            {
                var url = e.Args[0];
                var appName = e.Args[1];
                var appLoc = e.Args[2];
                var appCompany = e.Args[3];
                var version = e.Args[4];

                AutoUpdater.Start(url, appName, appLoc, appCompany, version);

                // wait for it to finish ;)
                while (!AutoUpdater.IsDone)
                {
                    Thread.Sleep(10);
                }

                Application.Current.Shutdown();
            }
        }
    }
}
