using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Net;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using System.Drawing.Imaging;
using System.Drawing;

namespace MinerT.kungfuactiongrip.com
{
    public class UserMgt
    {
        private const string WebServiceUri = "http://www.kungfuactiongrip.com/minerT/";

        #region WebService Methods

        public bool ValidateUser(string user, out string errMsg)
        {
            bool result = false;
            errMsg = string.Empty;
            if (!string.IsNullOrEmpty(user))
            {
                try
                {
                    var url = WebServiceUri + "userValidate.php?user=" + user;
                    var request = WebRequest.Create(url);
                    var response = request.GetResponse();

                    // Get the stream containing content returned by the server.
                    using (var dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        if (dataStream != null)
                            using (var reader = new StreamReader(dataStream))
                            {
                                // Read the content.
                                string responseFromServer = reader.ReadToEnd().Trim();
                                if (responseFromServer == "yay")
                                {
                                    result = true;
                                }
                            }
                    }
                }
                catch (Exception e)
                {
                    errMsg = e.Message;
                }
            }

            return result;
        }

        public string FetchUserBalance(string user, out bool isError)
        {
            isError = false;
            if (!string.IsNullOrEmpty(user))
            {
                try
                {
                    var url = WebServiceUri + "userBalance.php?user=" + user;
                    var request = WebRequest.Create(url);
                    var response = request.GetResponse();

                    // Get the stream containing content returned by the server.
                    using (var dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        if (dataStream != null)
                            using (var reader = new StreamReader(dataStream))
                            {
                                // Read the content.
                                var responseFromServer = reader.ReadToEnd().Trim();

                                return responseFromServer;
                            }
                    }
                }
                catch (Exception e)
                {
                    isError = true;
                    return e.Message;
                }
            }

            return string.Empty;
        }

        public string[] FetchUserTool(string balanceValue)
        {
            var parts = balanceValue.Split('@');
            var result = new[]{"lvl2", "A basic shovel"};
            if (parts.Length == 2)
            {
                double amt;
                double.TryParse(parts[0].Trim(), out amt);

                if (amt >= 10 && amt <= 499)
                {
                    result[0] = "lvl1";
                    result[1] = "A pickaxe. A bit better";
                }else if (amt >= 500 && amt <= 999)
                {
                    result[0] = "lvl2";
                    result[1] = "Golden pickaxe. Moving up";
                }else if (amt >= 1000 && amt <= 4499)
                {
                    result[0] = "lvl3";
                    result[1] = "Crystal pickaxe. Its unique";
                }
                else if (amt >= 4500 && amt <= 9999)
                {
                    result[0] = "lvl4";
                    result[1] = "Platinum pickaxe. Its rare";
                }
                else if (amt >= 10000)
                {
                    result[0] = "lvl5";
                    result[1] = "Ancient Pickaxe of Eminence";
                }
            }

            return result;
        }

        public string FetchMiningPoolConfig(string user, out bool isError)
        {
            isError = false;
            if (!string.IsNullOrEmpty(user))
            {
                try
                {
                    var url = WebServiceUri + "userPool.php?user=" + user;
                    var request = WebRequest.Create(url);
                    var response = request.GetResponse();

                    // Get the stream containing content returned by the server.
                    using (var dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        if (dataStream != null)
                            using (var reader = new StreamReader(dataStream))
                            {
                                // Read the content.
                                var responseFromServer = reader.ReadToEnd().Trim();

                                return responseFromServer;
                            }
                    }
                }
                catch (Exception e)
                {
                    isError = true;
                    return e.Message;
                }
            }

            isError = true;
            return "You need to enter a mining user";
        }

        #endregion

        public void SetMiningUser(TextBox tb)
        {
            var curAsm = System.Reflection.Assembly.GetExecutingAssembly();
            var asmPath = Path.GetDirectoryName(curAsm.Location);
            var config = asmPath + "\\settings.config";

            if (File.Exists(config))
            {
                var user = File.ReadAllText(config);
                tb.Text = user;
            }
        }

        public void SaveMiningUser(string user)
        {
            var curAsm = System.Reflection.Assembly.GetExecutingAssembly();
            var asmPath = Path.GetDirectoryName(curAsm.Location);
            var config = asmPath + "\\settings.config";

            if (!string.IsNullOrEmpty(user))
            {
                File.WriteAllText(config, user);
            }
        }

        public void LaunchBugWindow(Exception ex)
        {
            var user = Constants.MiningUser;

            try
            {
                var height = (int)Application.Current.MainWindow.RenderSize.Height;
                var width = (int)Application.Current.MainWindow.RenderSize.Width;
                var left = (int)Application.Current.MainWindow.Left;
                var top = (int)Application.Current.MainWindow.Top;
                var s = new Size(width, height);
                string fileName;

                using (var bitmap = new Bitmap(width, height))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(new Point(left, top), Point.Empty, s);
                    }

                    fileName = Path.GetTempFileName()+".jpg";
                    bitmap.Save(fileName, ImageFormat.Jpeg);
                }

                var br = new BugReport { ReportingUser = user, Fault = ex, ScreenshotFileName = fileName };
                br.LoadScreenshot();
                br.Show();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        
    }
}
