using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using MinerT.kungfuactiongrip.com;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using MinerT.Properties;

namespace MinerT
{
    /// <summary>
    /// Simple application. Check the XAML for comments.
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private DispatcherTimer _timer;
        private DispatcherTimer _scheduler;
        private bool _initVerCheck;
        private readonly List<System.Timers.Timer> _scheduleTimers = new List<System.Timers.Timer>();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon) TryFindResource("NotifyIcon");

            // add exception handling ;)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // check on start-up
            if (!_initVerCheck)
            {
                CheckForUpdates();
                _initVerCheck = true;
            }

            // now check for and save schedule ;)
            if (!FileHelper.DoesFileExist(Constants.ScheduleFile))
            {
                FileHelper.WriteToFile(Constants.ScheduleFile, MinerTResources.defaultSchedule);
            }

            // every four hours look for updates ;)
            _timer = new DispatcherTimer { Interval = new TimeSpan(4, 0, 0) };
            _timer.Tick += ((sender, e1) => CheckForUpdates());
            _timer.Start();

            // spin scheduler thread every five minute ;)
            _scheduler = new DispatcherTimer{ Interval = new TimeSpan(0,5,0)};
            _scheduler.Tick += ((sender, args) => ProcessSchedule());
            _scheduler.Start();

            // init on start up ;)
            ProcessSchedule();

        }

        private void ProcessSchedule()
        {
            try
            {
                var scheduleData = FileHelper.ReadFromFile(Constants.ScheduleFile);
                if (!string.IsNullOrEmpty(scheduleData))
                {
                    var schedule = JsonConvert.DeserializeObject<WeekSchedule>(scheduleData);
                    if (schedule != null)
                    {
                        // get current day
                        var now = DateTime.Now;
                        // convert to index
                        var dayOfWeek = (int) now.DayOfWeek;
                        // convert start and end to days
                        var scheduleDay = schedule.GetDayOfWeek(dayOfWeek);
                        if (scheduleDay != null)
                        {
                            bool startError;
                            bool endError;
                            var startTs = ParseScheduleValue(scheduleDay.StartMining, out startError);
                            var endTs = ParseScheduleValue(scheduleDay.EndMining, out endError);

                            if (!startError && !endError)
                            {
                                // see if current ts falls between
                                if (now.Ticks >= startTs.Ticks && now.Ticks <= endTs.Ticks)
                                {
                                    if (!Constants.IsMining)
                                    {
                                        Logger.LogToServer("NA", "Schedule Activated For [ " + scheduleDay.StartMining + " - " + scheduleDay.EndMining + " ]");

                                        // every 2 seconds process ;)
                                        var t = new System.Timers.Timer(30000) {Enabled = true};
                                        // self-destruc
                                        t.Elapsed += (sender, args) =>
                                        {
                                            var currentTs = DateTime.Now;
                                            // time to stop
                                            if (currentTs.Ticks >= endTs.Ticks)
                                            {
                                                try
                                                {
                                                    Logger.LogToServer("NA", "Stopped Scheduled Mining");
                                                    MinerProcess.Instance().StopMining();
                                                    // remove from the schedule queue ;)
                                                    _scheduleTimers.Remove(t);

                                                    t.Stop();
                                                    t.Dispose();
                                                }
                                                catch(Exception e1)
                                                {
                                                    Logger.LogToServer("NA", "Schedule Teardown Error [ " + e1.Message + " ]");
                                                }
                                            }else if (!Constants.IsMining && !Constants.ScheduleMissingUser){
                                                // Start Mining Process ;)
                                                try
                                                {
                                                    
                                                    Current.Dispatcher.BeginInvoke(new Action(() =>
                                                    {
                                                        var win = Current.MainWindow as MainWindow;
                                                        if (win != null)
                                                        {
                                                            Logger.LogToServer("NA", "Started Scheduled Mining");
                                                            win.SetControlsForScheduledMining();
                                                            MinerProcess.Instance().StartMining();
                                                        } 
                                                    }));                                                    
                                                }
                                                catch (Exception e)
                                                {
                                                    // push out to end of life ;)
                                                    var val = endTs.Ticks - DateTime.Now.Ticks;
                                                    if (val > 0)
                                                    {
                                                        t.Interval = val;
                                                    }

                                                    Logger.LogToServer("NA", "Error Starting Scheduled Mining [ " + e.Message + " ]");
                                                }
                                            }else if (Constants.ScheduleMissingUser)
                                            {
                                                // push out to end of life ;)
                                                var val = endTs.Ticks - DateTime.Now.Ticks;
                                                if (val > 0)
                                                {
                                                    t.Interval = val;
                                                }
                                                Logger.LogToServer("NA", "Missing User For Schedule");
                                            }
                                        };

                                        // Schedule and start instance ;)
                                        t.Start();
                                        _scheduleTimers.Add(t);
                                    }
                                    else
                                    {
                                        Logger.LogToServer("NA", "Scheduling Aborted Due to Manual Run");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogToServer("NA", "Failed to process schedule [ " + e.Message + " ]");
            }
        }

        private DateTime ParseScheduleValue(string val, out bool isError)
        {
            if (!string.IsNullOrEmpty(val))
            {
                try
                {
                    isError = false;
                    return DateTime.Parse(val);
                }
                catch
                {
                    
                }
            }

            isError = true;
            return DateTime.Now;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
            if (Constants.IsMining)
            {
                MinerProcess.Instance().StopMining();
            }

            KillHanginUpdates();
            _timer.Stop();
            _scheduler.Stop();


            // kill schedule timers ;)
            foreach (var timer in _scheduleTimers)
            {
                try
                {
                    timer.Stop();
                    timer.Dispose();
                }
                catch
                {
                    // best effort ;)
                }
            }

            // stop any mining processes too ;)
            if (Constants.IsMining)
            {
                try
                {
                    MinerProcess.Instance().StopMining();
                }
                catch { 
                    // best effort ;)
                }
            }
        }

        private static Attribute GetAttribute(Assembly assembly, Type attributeType)
        {
            object[] attributes = assembly.GetCustomAttributes(attributeType, false);
            if (attributes.Length == 0)
            {
                return null;
            }
            return (Attribute)attributes[0];
        }

        private void KillHanginUpdates()
        {
            // TODO : kill any remaining update processes, cuz they are zombies ;)
            var procs = Process.GetProcessesByName("UpdateBootstrapper");

            foreach (var proc in procs)
            {
                try
                {
                    proc.Kill();
                }catch{}
            }
        }

        private void CheckForUpdates()
        {
            KillHanginUpdates();

            Assembly mainAssembly = Assembly.GetEntryAssembly();
            var AppName = mainAssembly.GetName().Name;
            var AppLoc = mainAssembly.Location;

            var companyAttribute = (AssemblyCompanyAttribute)GetAttribute(mainAssembly, typeof(AssemblyCompanyAttribute));
            var appCompany = companyAttribute != null ? companyAttribute.Company : "";

            var version = mainAssembly.GetName().Version;

            var path = Path.GetDirectoryName(AppLoc);
            var bootStrapper = Path.Combine(path, Constants.BootstrapExe);
            var p = new Process
            {
                StartInfo =
                {
                    FileName = bootStrapper,
                    Arguments =
                        Constants.UpdateUrl + " " + AppName + " \"" + AppLoc + "\" \"" + appCompany + "\" " +
                        version,
                    UseShellExecute = true
                }
            };

            p.Start();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           var um = new UserMgt();
           um.LaunchBugWindow(e.ExceptionObject as Exception);
        }

    }
}
