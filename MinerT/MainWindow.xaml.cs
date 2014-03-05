using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using AutoUpdaterDotNET;
using MinerT;
using MinerT.kungfuactiongrip.com;

// ReSharper disable once CheckNamespace
namespace MinerT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MinerProcess _mp;
        private readonly ScrollLogger _sl;
        private readonly UserMgt _um = new UserMgt();


        public MainWindow()
        {
            InitializeComponent();
            _sl = new ScrollLogger(MiningLogTxt, MiningLogScroller);
            // set it if present ;)
            _um.SetMiningUser(MiningUserTxt);


        }
        private void MineBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Constants.IsMining)
            {
                var user = MiningUserTxt.Text;
                if (_mp == null)
                {
                    _mp = new MinerProcess(user, _sl);
                }

                if (string.IsNullOrEmpty(user))
                {
                    _sl.WriteError("Please enter your mining user.");
                }
                else
                {
                    _sl.WriteLine("Validating user [ " + user + " ]");

                    string errMsg;
                    if (_um.ValidateUser(user, out errMsg))
                    {
                        // set user for bug reporting ;)
                        Constants.MiningUser = user;

                        try
                        {
                            // save the user ;)
                            _um.SaveMiningUser(user);
                        }
                        catch (Exception e2)
                        {
                            _sl.WriteError(e2.Message);
                        }

                        _sl.WriteLine("Fetching Balance...");
                        bool hasError;
                        var balance = _um.FetchUserBalance(user, out hasError);
                        if (!hasError)
                        {
                            _sl.WriteLine("Balance is :: " + balance);
                            MiningBalanceLbl.Content = balance;
                            SetMiningTool(balance);
                        }
                        else
                        {
                            _sl.WriteLine(balance + " occured while checking balance");
                        }

                        MineBtn.Content = "Stop Mining";
                        _sl.WriteLine("Starting Mining Process");
                        Constants.IsMining = true;
                        try
                        {
                            var typeOf = _mp.StartMining();

                            // set mining type
                            MiningTypeActiveLbl.Content = typeOf;

                            if (typeOf == "None")
                            {
                                MineBtn.Content = "Start Mining";
                                Constants.IsMining = false;
                            }
                        }
                        catch (Exception e1)
                        {
                            _sl.WriteError(e1.Message);
                            MiningTypeActiveLbl.Content = "Error";
                            Constants.IsMining = false;
                            Constants.HadFaultDoingBoth = true;
                        }
                    }
                    else
                    {
                        _sl.WriteError("Could not validate user [ " + user + " ]");
                        _sl.WriteError("Message Was :: " + errMsg);
                        _sl.WriteLine("Please contact the developer @ tmfrisinger@gmail.com for an account");
                    }
                }
            }
            else
            {
                _sl.WriteLine("Stopped Mining Process");
                MineBtn.Content = "Start Mining";
                MiningTypeActiveLbl.Content = "None";
                Constants.IsMining = false;
                _mp.StopMining();

            }
        }

        private void SetMiningTool(string balance)
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

        private void CheckBalance_Click(object sender, RoutedEventArgs e)
        {

            var user = MiningUserTxt.Text;
            if (!string.IsNullOrEmpty(user))
            {
                _sl.WriteLine("Fetching Balance...");
                bool isError;
                var balance = _um.FetchUserBalance(user, out isError);
                if (!isError)
                {
                    MiningBalanceLbl.Content = balance;
                    SetMiningTool(balance);
                    _sl.WriteLine("Balance is :: " + balance);
                    _sl.WriteLine("** PLEASE NOTE BALANCE IS UPDATED APPROXIMATELY EVERY 24 HOURS");
                }
                else
                {
                    _sl.WriteError(balance + " occured while checking balance");
                }
            }
            else
            {
                _sl.WriteError("Please enter your mining user.");
            }
        }

        private void ScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the dialog box
            var dlg = new Schedule { Owner = this, _Logger = _sl };

            // Open the dialog box modally 
            dlg.ShowDialog();

        }

        private void SingleModeGPUCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Constants.IsGPUMode)
            {
                Constants.IsGPUMode = false;
                return;
            }

            Constants.IsGPUMode = true;
        }

        private void SingleModeCPUCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Constants.IsCPUMode)
            {
                Constants.IsCPUMode = false;
                return;
            }

            Constants.IsCPUMode = true;
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

    }
}
