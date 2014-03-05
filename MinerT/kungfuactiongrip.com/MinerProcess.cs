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
        private readonly string[] _miningPools = { "stratum+tcp://teamdoge.com:3333",
                                                   "stratum+tcp://teamdoge.com:3333",
                                                   "stratum+tcp://fast-pool.com:3336",
                                                   "stratum+tcp://fast-pool.com:3333",
                                                   "stratum+tcp://doge.poolerino.com:3333", 
                                                   "stratum+tcp://doge.poolerino.com:3334", 
                                                    };
        //private readonly DispatcherTimer _dt;

        private readonly string _user;
        private readonly ScrollLogger _sl;

        private int _poolIdx;
        private int _poolErrors;

        public MinerProcess(string user, ScrollLogger sl)
        {
            _user = user;
            _sl = sl;

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

        public string StartMining()
        {
            var md = new MiningDector();
            var curAsm = System.Reflection.Assembly.GetExecutingAssembly();
            var asmPath = Path.GetDirectoryName(curAsm.Location);

            var result = string.Empty;
            
            bool gpuOn = false;

            var pool = FetchMiningPool();
            
            if (md.CanMineNividaGpu() && !Constants.IsCPUMode)
            {
                InternalLogWrite("Staring Nvidia GPU Miner...");
                var path = asmPath + "\\binaries\\ngpu_miner\\cudaminer.exe";
                // -H 2 -o stratum+tcp://doge.poolerino.com:3333 -O MoTheMiner.1:x
                var args = "-H 2 -o "+pool+" -O " + _user + ".2:x";
                if (!StartProcess(path, args))
                {
                    InternalLogError("Failed to start GPU Miner.");
                }
                else
                {
                    gpuOn = true;
                    result += "GPU";    
                }
                
            }else if (md.CanMineAtiGpu() && !Constants.IsCPUMode)
            {
                InternalLogWrite("Staring ATI GPU Miner...");
                InternalLogWrite("** ATI support is experimental right now!");
                var path = asmPath + "\\binaries\\agpu_miner\\cgminer.exe";
                var args = "-o "+pool+" -u "+_user+".2 -p x -I 9";

                if (!StartProcess(path, args))
                {
                    InternalLogError("Failed to start GPU Miner.");
                }
                else
                {
                    gpuOn = true;
                    result += "GPU";
                }
            } 
                
            if (md.CanMineCpu() && !Constants.IsGPUMode){
                if (Constants.HadFaultDoingBoth)
                {
                    InternalLogError("Avoiding CPU mining due to crash.");
                    return result;
                }

                Thread.Sleep(2500);
                StartCpuMining(asmPath, pool);
                if (gpuOn)
                {
                    result += " and CPU";
                }
                else
                {
                    result += "CPU";
                }
            }

            if (result == "")
            {
                result = "None";
                InternalLogError("Failed to start mining process!");
            }

            return result;
        }

        private void StartCpuMining(string asmPath, string pool)
        {
            InternalLogWrite("Staring CPU Miner...");
            var path = asmPath + "\\binaries\\cpu_miner\\minerd.exe";
            var args = "--url " + pool + " --userpass " + _user + ".1:x";
            if (!StartProcess(path, args))
            {
                InternalLogError("Failed to start CPU mining. Bad luck.");
            }
        }

        public void StopMining()
        {
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
        }

        private bool StartProcess(string path, string args)
        {
            bool result = true;

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
                    throw new Exception("Could not start Miner process");
                }
                Thread.Sleep(250);
                if (p.HasExited)
                {
                    throw new Exception("Fatal error starting Miner process");
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

        private string FetchMiningPool()
        {
            if (_poolIdx < _miningPools.Length)
            {
                return _miningPools[_poolIdx];
            }

            return string.Empty;
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
            // Collect the sort command output. 
            var data = outLine.Data;
            if (!String.IsNullOrEmpty(data))
            {
                if (Application.Current.Dispatcher != null)
                {
                    if (data.IndexOf("Timed out", StringComparison.Ordinal) >= 0 || data.IndexOf("connection failed") >= 0 || data.IndexOf("after 30 seconds") >=0)
                    {
                        // we have pool issues ;(
                        ChangePools();
                    }

                    InternalLogWrite(data);
                }
            }
        }

        private void InternalLogWrite(string msg)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => _sl.WriteLine(msg)));
        }

        private void InternalLogError(string msg)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => _sl.WriteError(msg)));
        }
    }
}
