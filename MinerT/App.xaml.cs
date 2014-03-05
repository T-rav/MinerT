using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AutoUpdaterDotNET;
using Hardcodet.Wpf.TaskbarNotification;
using MinerT.kungfuactiongrip.com;

namespace MinerT
{
    /// <summary>
    /// Simple application. Check the XAML for comments.
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private DispatcherTimer _timer;
        private bool _initVerCheck;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon) FindResource("NotifyIcon");

            // add exception handling ;)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // check on start-up
            if (!_initVerCheck)
            {
                CheckForUpdates();
                _initVerCheck = true;
            }

            // every four hours look for updates ;)
            _timer = new DispatcherTimer { Interval = new TimeSpan(4, 0, 0) };
            _timer.Tick += ((sender, e1) => CheckForUpdates());
            _timer.Start();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
            _timer.Stop();
        }

        private void CheckForUpdates()
        {
            //AutoUpdater.Start(Constants.UpdateUrl);
            //Thread.Sleep(1500);
            //OnExit(null);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           var um = new UserMgt();
           um.LaunchBugWindow(e.ExceptionObject as Exception);
        }

    }
}
