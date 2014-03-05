using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MinerT.kungfuactiongrip.com;


namespace MinerT
{
    /// <summary>
    /// Interaction logic for BugReport.xaml
    /// </summary>
    public partial class BugReport : Window
    {
        public string ReportingUser { get; set; }
        public Exception Fault { get; set; }
        public string ScreenshotFileName { get; set; }

        public BugReport()
        {
            InitializeComponent();
        }

        public void LoadScreenshot()
        {
            try
            {
                ScreenshotImage.Source = new BitmapImage(new Uri(ScreenshotFileName));
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                DrawErrorBitmap();
                try
                {
                    ScreenshotImage.Source = new BitmapImage(new Uri(ScreenshotFileName));
                }
                catch
                {
                    
                }
            }
        }

        private void DrawErrorBitmap()
        {
            var bmp = new Bitmap(300, 300);
            using (var g = Graphics.FromImage(bmp))
            {
                var font = new Font("Arial", 20, GraphicsUnit.Point);
               // g.Clear(Color.White);
                g.DrawString("Failed to load screenshot",font,Brushes.Black,(float)5.0,(float)5.0);
            }
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            var newBitmap = new Bitmap(300, 300, bmpData.Stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, bmpData.Scan0);
            var path = Path.GetTempFileName();
            newBitmap.Save(path);
            ScreenshotFileName = path;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // submit email with screen shot and error
            var data = AdditionalDetailsTextBox.Text;
            Task.Factory.StartNew(()=>SendEmail(data));
            
            this.Close();
        }

        private void SendEmail(string data)
        {
            var mMailMessage = new MailMessage {From = new MailAddress(Constants.BugFromEmail)};

            // address of sender
            // recipient address
            mMailMessage.To.Add(new MailAddress(Constants.BugToEmail));

            mMailMessage.Subject = Constants.BugSubjectEmail;
            // Set the body of the mail message
            mMailMessage.Body = data;
            // Set the format of the mail message body as HTML
            mMailMessage.IsBodyHtml = false;
            // Set the priority of the mail message to normal
            mMailMessage.Priority = MailPriority.Normal;
            mMailMessage.Attachments.Add(new  Attachment(ScreenshotFileName));

            // Instantiate a new instance of SmtpClient
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("dogecoin99@gmail.com", "$uperman11"),
                EnableSsl = true
            };

            // Send the mail message
            try
            {
                client.Send(mMailMessage);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch(Exception ex)
            {
                mMailMessage.Subject = ex.Message;
            }
            finally
            {
                mMailMessage.Dispose();
            }
        }
    }
}
