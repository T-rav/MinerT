using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MinerT.kungfuactiongrip.com;
using System.Windows.Controls;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MinerT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Logger _sl;
        private readonly UserMgt _um = new UserMgt();

        public MainWindow()
        {
            InitializeComponent();
            _sl = new Logger(MiningLogTxt, MiningLogScroller);
            // set it if present ;)
            _um.SetMiningUser(MiningUserTxt);
            MinerProcess.Instance().Logger = _sl;
        }
        
        private void MineBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Constants.IsMining)
            {
                var user = MiningUserTxt.Text;
                MinerProcess.Instance().User = user;
                
                if (string.IsNullOrEmpty(user))
                {
                    _sl.WriteError("Please enter your mining user.");
                }
                else
                {
                    _sl.WriteLine("Fetching Details For [ " + user + " ]");
                    var userConfig = _um.FetchUserDetails(user, CoinType.Text);

                    if (userConfig.IsValid)
                    {
                        // set user for bug reporting ;)
                        Constants.MiningUser = userConfig;
                        SaveUser(user);
                        SetBalanceAndTool(userConfig);

                        if (userConfig.IsUserMessageError)
                        {
                            _sl.WriteError(userConfig.UserMessage);
                        }
                        else
                        {
                            MineBtn.Content = "Stop Mining";
                            _sl.WriteLine("Starting Mining Process");
                            Constants.IsMining = true;
                            CoinType.IsEnabled = false;
                            
                            // print any message for the user ;)
                            _sl.WriteLine(userConfig.UserMessage);

                            StartMiningProcess();
                        }

                    }else{
                        _sl.WriteError("Could not validate user [ " + user + " ]");
                        _sl.WriteError("Message Was :: " + userConfig.UserMessage);
                    }
                }
            }
            else
            {
                _sl.WriteLine("Stopped Mining Process");
                MineBtn.Content = "Start Mining";
                Constants.IsMining = false;
                CoinType.IsEnabled = true;
                MinerProcess.Instance().StopMining();

            }
        }

        public void SetControlsForScheduledMining()
        {
            MineBtn.Content = "Stop Mining";
            _sl.WriteLine("Starting Mining Process");
            Constants.IsMining = true;
            CoinType.IsEnabled = false;
        }
        
        private void StartMiningProcess()
        {
            // set the mining type cpu / gpu
            try
            {
                var typeOf = MinerProcess.Instance().StartMining();
                // set mining type
                SetCheckboxType(typeOf);
            }
            catch (Exception e1)
            {
                _sl.WriteError(e1.Message);
                //MiningTypeActiveLbl.Content = "Error";
                Constants.IsMining = false;
                CoinType.IsEnabled = true;
                Constants.HadFaultDoingBoth = true;
            }
        }

        
        private void SetBalanceAndTool(MiningUserConfig userConfig)
        {
            double balance = SetBalances(userConfig);
            SetMiningTool(balance);

            // set user rank
            var rank = userConfig.UserRank;
            RankLabelValue.Content = rank;
        }

        private double SetBalances(MiningUserConfig userConfig)
        {
            var result = 0.0;
            try
            {
                foreach (var bal in userConfig.TheBalances)
                {
                    var balance = bal.Balance;
                    var cur = bal.BalanceCurrency.ToUpper();

                    if (cur == "DOGE")
                    {
                        result = balance;
                        DOGE.Content = balance + " " + cur;
                    }
                    else if (cur == "BTC")
                    {
                        BTC.Content = balance + " " + cur;
                    }

                    _sl.WriteLine("Balance is :: " + bal);
                }
            }
            catch (Exception e)
            {
                _sl.WriteError(e.Message);
            }

            return result;
        }

        private void SaveUser(string user)
        {
            try
            {
                // save the user ;)
                _um.SaveMiningUser(user);
            }
            catch (Exception e2)
            {
                _sl.WriteError(e2.Message);
            }
        }

        private void SetCheckboxType(string typeOf)
        {
            // set mining type
            if (typeOf == "None")
            {
                MineBtn.Content = "Start Mining";
                Constants.IsMining = false;
                CoinType.IsEnabled = true;
                SingleCpuModeCheckBox.IsChecked = false;
                SingleGpuModeCheckBox.IsChecked = false;
            }
            else if (!typeOf.Contains("CPU"))
            {
                SingleCpuModeCheckBox.IsChecked = false;
            }
            else if (!typeOf.Contains("GPU"))
            {
                SingleGpuModeCheckBox.IsChecked = false;
            }
        }

        private void SetMiningTool(double balance)
        {
            try
            {
                var tool = _um.FetchUserTool(balance);
                MiningRewardImage.Source = (ImageSource)FindResource(tool[0]);
                MiningRewardLabel.Content = tool[1];
            }
            catch (Exception e1)
            {
                _sl.WriteError(e1.Message);
            }
        }

        private void CheckStats_Click(object sender, RoutedEventArgs e)
        {

            var user = MiningUserTxt.Text;
            if (!string.IsNullOrEmpty(user))
            {
                _sl.WriteLine("Fetching Balance...");

                var userConfig = _um.FetchUserDetails(user, "NONE");
                SetBalanceAndTool(userConfig);
                _sl.WriteLine("** PLEASE NOTE BALANCE IS UPDATED APPROXIMATELY EVERY 24 HOURS");
            }
            else
            {
                _sl.WriteError("Please enter your mining user.");
            }
        }

        private void ScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the dialog box
            var dlg = new Schedule { Owner = this, Logger = _sl };

            var scheduleData = FileHelper.ReadFromFile(Constants.ScheduleFile);

            WeekSchedule schedule = null;

            if (!string.IsNullOrEmpty(scheduleData))
            {
                try
                {
                    schedule = JsonConvert.DeserializeObject<WeekSchedule>(scheduleData);
                }
                catch (Exception e1)
                {
                    _sl.WriteError("Failed to hydrate schedule [ " + e1.Message + " ]");
                }
            }

            dlg.BindTemplateData(schedule);

            // Open the dialog box modally 
            dlg.ShowDialog();

        }

        private void SingleModeGPUCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Constants.IsGpuMode)
            {
                Constants.IsGpuMode = false;
                return;
            }

            Constants.IsGpuMode = true;
        }

        private void SingleModeCPUCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Constants.IsCpuMode)
            {
                Constants.IsCpuMode = false;
                return;
            }

            Constants.IsCpuMode = true;
        }

        private void ReportBugButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _um.LaunchBugWindow(null);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {

            }
        }

        private void CoinType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var comboBoxItem = e.AddedItems[0] as ComboBoxItem;
                if (comboBoxItem != null)
                {
                    var user = MiningUserTxt.Text;
                    var text = comboBoxItem.Content as string;
                    BTC.IsSelected = true;
                    if (text != null && text.Equals("Most Profitable"))
                    {
                        Constants.MiningMode = MiningType.Profitable;
                    }
                    /*
                    else if(text != null && text.Equals("Doge Coin"))
                    {
                        Constants.MiningMode = MiningType.Doge;
                        DOGE.IsSelected = true;
                    }
                    */
                }
            }
            catch(Exception e1)
            {
                if (_sl != null)
                {
                    _sl.WriteError(e1.Message);
                }
            }
        }

    }
}
