using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MinerT.kungfuactiongrip.com;
using Newtonsoft.Json;

namespace MinerT
{
    /// <summary>
    /// Interaction logic for Schedule.xaml
    /// </summary>
    public partial class Schedule : Window
    {
        public Logger Logger { get; set; }
        
        public Schedule()
        {
            InitializeComponent();
        }

        private ComboBox[] FetchStartBoxes()
        {
            var startBoxes = new[]
            {
                MondayStartBox, TuesdayStartBox, WednesdayStartBox, ThursdayStartBox, FridayStartBox, SaturdayStartBox,
                SundayStartBox
            };

            return startBoxes;
        }

        private ComboBox[] FetchStopBoxes()
        {
            var stopBoxes = new[]
            {
                MondayStopBox, TuesdayStopBox, WednesdayStopBox, ThursdayStopBox, FridayStopBox, SaturdayStopBox,
                SundayStopBox
            };

            return stopBoxes;
        }

        public void BindTemplateData(WeekSchedule schedule)
        {

            var startBoxes = FetchStartBoxes();
            var stopBoxes = FetchStopBoxes();
            List<DaySchedule> days = null;
            if (schedule != null)
            {
                days = schedule.TheSchedules;
            }

            // build start times ;)
            var pos = 0;
            if (days != null)
            {
                for (pos = 0; pos < days.Count; pos++)
                {
                    var ts = days[pos].StartMining;
                    BuildTimes(startBoxes[pos], ts);
                }
            }

            // clean up any non-scheduled days ;)
            for (; pos < 7; pos++)
            {
                BuildTimes(startBoxes[pos], string.Empty);
            }

            // build stop times ;)
            if (days != null)
            {
                for (pos = 0; pos < days.Count; pos++)
                {
                    var ts = days[pos].EndMining;
                    BuildTimes(stopBoxes[pos], ts);
                }
            }
            else
            {
                pos = 0;
            }

            // clean up any non-scheduled days ;)
            for (; pos < 7; pos++)
            {
                BuildTimes(stopBoxes[pos], string.Empty);
            }
        }

        private void BuildTimes(ComboBox sb, string ds)
        {
            sb.Items.Add("Manual");
            bool valSet = false;
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

                    var idx = sb.Items.Add(itemVal);
                    if (itemVal == ds)
                    {
                        sb.SelectedIndex = idx;
                        valSet = true;
                    }
                }
            }

            // final EOD time ;)
            sb.Items.Add("23:59");
            
            // default ;)
            if (!valSet)
            {
                sb.SelectedValue = "Manual";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Logger != null)
            {
                Logger.WriteLine("Updated Schedule Canceled");
            }
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

            // Write out to disk ;)
            try
            {
                var schedule = new WeekSchedule();
                var startBoxes = FetchStartBoxes();
                var stopBoxes = FetchStopBoxes();
                for (int i = 0; i < startBoxes.Count(); i++)
                {
                    var ds = new DaySchedule {StartMining = startBoxes[i].Text, EndMining = stopBoxes[i].Text};
                    schedule.AddDayOfWeek(ds);
                }

                var daysOfWeeks = JsonConvert.SerializeObject(schedule);
                FileHelper.WriteToFile(Constants.ScheduleFile, daysOfWeeks);

                // Set schedule ;)
                if (Logger != null)
                {
                    Logger.WriteLine("Updated Schedule Applied");
                }

            }
            catch (Exception e1)
            {
                Logger.LogToServer("NA", "Failed to save schedule [ " + e1.Message + " ]");
            }
            finally
            {
                this.Close();                
            }
        }
    }
}
