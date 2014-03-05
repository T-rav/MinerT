using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows;

namespace MinerT.kungfuactiongrip.com
{
    public class MinerProcess : IDisposable
    {
        private List<Process> _miningProces = new List<Process>();

        private readonly MiningPool[] _dogeMiningPools =
        {
            /*new MiningPool {IsManagedPool = false, Pool = "stratum+tcp://pool.shitpost.asia:1917"},*/
            new MiningPool {IsManagedPool = true, Pool = "stratum+tcp://stratum.fast-pool.com:3001"},
            new MiningPool {IsManagedPool = true, Pool = "stratum+tcp://stratum.fast-pool.com:3002"},
            new MiningPool {IsManagedPool = true, Pool = "stratum+tcp://stratum.fast-pool.com:3003"},
            new MiningPool {IsManagedPool = true, Pool = "stratum+tcp://stratum.fast-pool.com:3005"},
            new MiningPool {IsManagedPool = true, Pool = "stratum+tcp://teamdoge.com:3333"}
        };

        private readonly MiningPool[] _profitMiningPools =
        {
            new MiningPool {IsManagedPool = false, Pool = "stratum+tcp://eu.wafflepool.com:3333"},
            new MiningPool {IsManagedPool = false, Pool = "stratum+tcp://useast.wafflepool.com:3333"},
            new MiningPool {IsManagedPool = false, Pool = "stratum+tcp://uswest.wafflepool.com:3333"},
            new MiningPool {IsManagedPool = false, Pool = "stratum+tcp://eu.clevermining.com:3333"},
            new MiningPool {IsManagedPool = false, Pool = "stratum+tcp://us.clevermining.com:3333"},
            new MiningPool {IsManagedPool = false, Pool = "stratum+tcp://mine.coinshift.com:3333"},
            new MiningPool {IsManagedPool = false, Pool = "stratum+tcp://backup.coinshift.com:3333"}
        };

        private MiningPool[] _miningPools;
        public string User { get; set;}
        
        private Logger _sl;
        public Logger Logger
        {
            get { return _sl; }
            set
            {
                if (_sl == null)
                {
                    _sl = value;
                }
            }
        }
        private int _poolIdx;
        private int _poolErrors;

        private static MinerProcess _instance;

        public static MinerProcess Instance()
        {
            return _instance ?? (_instance = new MinerProcess());
        }

        private MinerProcess()
        {
            //// every 15 min check mining pool config ;)
            //_dt = new DispatcherTimer { Interval = new TimeSpan(0, 15, 0) };
            //_dt.Tick += ((sender, e) =>
            //{
            //    CheckAndUpdatePools();   
            //});

            //_dt.Start();
        }

        //private void CheckAndUpdatePools()
        //{
        //    var um = new UserMgt();
        //    bool isError;
        //    var pool = um.FetchMiningPoolConfig(_user, out isError);
        //    var pools = pool.Split(',');
        //    _miningPools = pools;

        //    if (!isError)
        //    {
        //        // TODO : restart mining operation - We need to lock around this operation or disable the button
        //        _poolIdx = 0;
        //        StopMining();
        //        StartMining();
        //    }
        //}

        // Needs Constants.MiningMode set ;)
        public string StartMining()
        {
            var error = string.Empty;
            if (Constants.MiningUser == null)
            {
                // load from file ;)
                var um = new UserMgt();
                var miningUser = um.GetMiningUserFromFile();
                var theUserObj = um.FetchUserDetails(miningUser, Constants.MiningMode.ToString());
                if (theUserObj != null)
                {
                    Instance().User = miningUser;
                    Constants.MiningUser = theUserObj;
                }
                else
                {
                    Constants.ScheduleMissingUser = true;
                    return "Error : Could not validate user object";
                }
            }

            var md = new MiningDector();
            var curAsm = System.Reflection.Assembly.GetExecutingAssembly();
            var asmPath = Path.GetDirectoryName(curAsm.Location);

            var result = string.Empty;
            var gpuOn = false;
            var pool = FetchMiningPool();

            LogToServer("GRAPHICS CARD :: " + md.FetchGraphicsCardInfo());

            if (md.CanMineNividaGpu() && Constants.IsGpuMode)
            {
                InternalLogWrite("Staring Nvidia GPU Miner...");
                
                LogToServer("Starting Nivida GPU Miner...");

                var path = asmPath + "\\binaries\\ngpu_miner\\cudaminer.exe";
                var args = "-H 2 -o "+pool.Pool+" -O " + User + ".2:x";

                if (!pool.IsManagedPool)
                {
                    args = "-H 2 -o " + pool.Pool + " -O " + Constants.MiningUser.MiningWallet + ":x";
                }

                if (!StartProcess(path, args))
                {
                    InternalLogError("Failed to start NVIDIA GPU Miner.");
                }
                else
                {
                    gpuOn = true;
                    result += "GPU";    
                }
            }
            else if(md.CanMineAtiGpu(out error) && Constants.IsGpuMode)
            {
                InternalLogWrite("Staring ATI GPU Miner...");
                LogToServer("Trying ATI Mining....");

                InternalLogWrite("** ATI support is experimental right now!");
                var path = asmPath + "\\binaries\\agpu_miner\\cgminer.exe";
                var args = "-o " + pool.Pool + " -u " + User + ".2 -p x -I 9";

                if (!pool.IsManagedPool)
                {
                    args = "-o " + pool.Pool + " -u " + Constants.MiningUser.MiningWallet + " -p x -I 9";    
                }
                
                if (!StartProcess(path, args))
                {
                    InternalLogError("Failed to start ATI GPU Miner.");
                }
                else
                {
                    gpuOn = true;
                    result += "GPU";
                } 
            }

            if (!string.IsNullOrEmpty(error))
            {
                InternalLogWrite(error);
                InternalLogError("If you have onboard graphics and an ATI card. Please turn your onboard off!");
            }

            if (md.CanMineCpu() && Constants.IsCpuMode)
            {
                if (Constants.HadFaultDoingBoth)
                {
                    LogToServer("Avoiding CPU mining due to crash.");
                    InternalLogError("Avoiding CPU mining due to crash.");
                    return result;
                }

                Thread.Sleep(2500);
                LogToServer("Starting CPU mining...");

                StartCpuMining(asmPath, pool, Constants.MiningUser);

                if (gpuOn)
                {
                    result += " and CPU";
                }
                else
                {
                    result += "CPU";
                }
            }
            else
            {
                LogToServer("Skipping CPU because of settings");
                InternalLogWrite("Skipping CPU because of settings");
            }

            if (result == "")
            {
                result = "None";
                InternalLogError("Failed to start mining process!");
                LogToServer("Failed to start mining process");
            }

            return result;
        }

        private void StartCpuMining(string asmPath, MiningPool pool, MiningUserConfig user)
        {
            InternalLogWrite("Staring CPU Miner...");
            var path = asmPath + "\\binaries\\cpu_miner\\minerd.exe";

            var args = "--url " + pool.Pool + " --userpass " + User + ".1:x";
            if (!pool.IsManagedPool)
            {
                args = "--url " + pool.Pool + " --userpass " + user.MiningWallet + ":x"; 
            }
            
            if (!StartProcess(path, args))
            {
                InternalLogError("Failed to start CPU mining. Bad luck.");
            }
        }

        public void StopMining()
        {
            LogToServer("Stopped Mining");

            foreach (var proc in _miningProces)
            {
                try
                {
                    proc.Kill();
                }
                catch (Exception e) {
                    InternalLogError(e.Message);
                }
            }

            // reset the list of procs ;)
            _miningProces = new List<Process>();
        }

        public void Dispose()
        {
            //if (_dt != null)
            //{
            //    _dt.Stop();
            //}

            StopMining();
        }

        private bool StartProcess(string path, string args)
        {
            var result = true;

            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            p.OutputDataReceived += OutputHandler;
            p.ErrorDataReceived += OutputHandler;
            p.EnableRaisingEvents = true;
           
            p.StartInfo.FileName = "\""+ path + "\"";
            p.StartInfo.Arguments = args;

            try
            {
                if(!p.Start()){
                    throw new Exception("Could not start Miner process. Process did not start.");
                }
                Thread.Sleep(250);
                if (p.HasExited)
                {
                    throw new Exception("Could not start Miner process. It has exited.");
                }

                // enable output reading ;)
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                _miningProces.Add(p);
            }
            catch (Exception e)
            {
                result = false;
                InternalLogError(e.Message);
            }

            return result;
        }

        /*
         * TODO : Change over to fetch from server, this way I can manage who gets what pool
         */
        private MiningPool FetchMiningPool()
        {
            // set pool based on type of mining ;)
            if (Constants.MiningMode == MiningType.Profitable)
            {
                _miningPools = _profitMiningPools;
                _poolIdx = 0;
                _poolErrors = 0;
            }
            else if (Constants.MiningMode == MiningType.Doge)
            {
                _miningPools = _dogeMiningPools;
                _poolIdx = 0;
                _poolErrors = 0;
            }

            if (_poolIdx < _miningPools.Length)
            {
                var result = _miningPools[_poolIdx];
                Logger.LogToServer(Constants.MiningUser.MiningUser, "Using Pool [ " + result.Pool + "]");
                return result;
            }

            return null;
        }

        private void ChangePools()
        {
            _poolErrors++;
            if (_poolErrors <= 5)
            {
                return;
            }

            if (_poolIdx < (_miningPools.Length - 1))
            {
                _poolIdx++;
                _poolErrors = 0;
            }
            else
            {
                _poolIdx = 0;
                _poolErrors = 0;
            }

            InternalLogError("Current pool/port has network faults. Most likey DDOS. Swapping to new pool/port.");

            StopMining();
            StartMining();
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            try
            {
                // Collect the sort command output. 
                var sendingProc = sendingProcess as Process;
                var logName = "Unknown -> ";
                if (sendingProc != null)
                {
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (sendingProc.MainModule.FileName.IndexOf("minerd", StringComparison.Ordinal) >= 0)
                    {
                        logName = "CPU -> ";
                    }
                    else
                    {
                        logName = "GPU -> ";
                    }
                }
                var data = outLine.Data;
                if (!String.IsNullOrEmpty(data))
                {
                    if (Application.Current.Dispatcher != null)
                    {
                        if (data.IndexOf("Timed out", StringComparison.Ordinal) >= 0 || data.IndexOf("connection failed", StringComparison.Ordinal) >= 0 || data.IndexOf("after 30 seconds", StringComparison.Ordinal) >= 0)
                        {
                            // we have pool issues ;(
                            ChangePools();
                            InternalLogError(logName+"Connection issue [ " + data + "]");
                        }
                        else
                        {
                            // scrub out the pool portion ;)
                            if (data.IndexOf("Starting Stratum", StringComparison.Ordinal) < 0)
                            {
                                InternalLogWrite(logName+data);
                            }
                        }
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // just best effort ;)
            }
        }

        private void LogToServer(string msg)
        {
            var user = "Unknown";
            if (Constants.MiningUser != null)
            {
                user = Constants.MiningUser.MiningUser;
            }
            
            Logger.LogToServer(user, msg);
        }

        private void InternalLogWrite(string msg)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Logger.WriteLine(msg)));
        }

        private void InternalLogError(string msg)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Logger.WriteError(msg)));
        }

    }
}
