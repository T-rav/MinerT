using System.Windows;
using System.Windows.Controls;
using MinerT.kungfuactiongrip.com;

namespace MinerT
{
    /// <summary>
    /// Interaction logic for Schedule.xaml
    /// </summary>
    public partial class Schedule : Window
    {
        public ScrollLogger _Logger { get; set; }
        public Schedule()
        {
            InitializeComponent();
            // TODO : populate with data ;)
            BindTemplateData();
        }

        private void BindTemplateData()
        {
            var startBoxes = new[]
            {
                MondayStartBox, TuesdayStartBox, WednesdayStartBox, ThursdayStartBox, FridayStartBox, SaturdayStartBox,
                SundayStartBox
            };

            var stopBoxes = new[]
            {
                MondayStopBox, TuesdayStopBox, WednesdayStopBox, ThursdayStopBox, FridayStopBox, SaturdayStopBox,
                SundayStopBox
            };

            // add manual option to boxes ;)
            foreach (var sb in startBoxes)
            {
                BuildTimes(sb);
            }

            foreach (var sb in stopBoxes)
            {
                BuildTimes(sb);
            }
        }

        private void BuildTimes(ComboBox sb)
        {
            sb.Items.Add("Manual");

            for (var i = 0; i < 24; i++)
            {
                for (var y = 0; y < 60; y += 15)
                {
                    string itemVal;
                    if (i < 10)
                    {
                        itemVal = "0" + i + ":";
                    }
                    else
                    {
                        itemVal = i + ":";
                    }

                    if (y < 10)
                    {
                        itemVal += "0" + y;
                    }
                    else
                    {
                        itemVal += y;
                    }

                    sb.Items.Add(itemVal);
                }
            }
            
            // default ;)
            sb.SelectedValue = "Manual";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Logger != null)
            {
                _Logger.WriteLine("Updated Schedule Canceled");
            }
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO : Set schedule ;)
            if (_Logger != null)
            {
                _Logger.WriteLine("Scheduling Experimental. It has not been updated. Manual mode only!");
                //_Logger.WriteLine("Updated Schedule Applied");
            }
            this.Close();
        }
    }
}
