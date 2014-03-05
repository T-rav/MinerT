using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Net.Cache;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace AutoUpdaterDotNET
{
    internal partial class DownloadUpdateDialog : Form
    {
        private readonly string _downloadUrl;
        private readonly string _processName;
        private readonly string _processLocation;

        private string _tempPath;

        public DownloadUpdateDialog(string downloadUrl, string processName, string processLocation)
        {
            InitializeComponent();

            _processName = processName;
            _processLocation = processLocation;
            _downloadUrl = downloadUrl;
        }

        private void DownloadUpdateDialogLoad(object sender, EventArgs e)
        {
            var webClient = new WebClient();

            var uri = new Uri(_downloadUrl);

            _tempPath = string.Format(@"{0}{1}", Path.GetTempPath(), GetFileName(_downloadUrl));

            webClient.DownloadProgressChanged += OnDownloadProgressChanged;

            webClient.DownloadFileCompleted += OnDownloadComplete;

            webClient.DownloadFileAsync(uri, _tempPath);
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void KillCurrentProcess()
        {
            if (!string.IsNullOrEmpty(_processName))
            {
                var procList = Process.GetProcessesByName(_processName);
                foreach (var proc in procList)
                {
                    try
                    {
                        proc.Kill();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(@"Error Updating : " + e.Message);
                    }
                }
            }
        }

        private bool InstallMsi(string fileName, string targetDir)
        {
            try
            {
                // run msi installer ;)
                var p = Process.Start(fileName);

                if (p != null)
                {
                    p.WaitForExit(60);
                }

                return (p != null && p.HasExited);

                //var asmName = Path.GetFileName(_processLocation);
                //if (asmName != null)
                //{
                //    var targetLoc = Path.Combine(targetDir, asmName);
                //    File.Copy(fileName, targetLoc,true);
                //}
                //return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Error Updating : " + e.Message);
                return false;
            }
        }

        // TODO : Force a .zip or msi download so we can send lots of goodies ;)
        private void OnDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            // kill it ;)
            KillCurrentProcess();

            // replace it ;)
            var targetDir = Path.GetDirectoryName(_processLocation);
            var updateOk = InstallMsi(_tempPath, targetDir);

            // start it ;)
            if (updateOk)
            {
                if (targetDir != null)
                {
                    var processStartInfo = new ProcessStartInfo { FileName = _processLocation, UseShellExecute = true, WorkingDirectory = targetDir};
                    Process.Start(processStartInfo);
                }
            }

            AutoUpdater.IsDone = true;

            if (AutoUpdater.IsWinFormsApplication)
            {
                Application.Exit();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private static string GetFileName(string url)
        {
            var fileName = string.Empty;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            httpWebRequest.Method = "HEAD";
            httpWebRequest.AllowAutoRedirect = false;
            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Redirect) || httpWebResponse.StatusCode.Equals(HttpStatusCode.Moved) || httpWebResponse.StatusCode.Equals(HttpStatusCode.MovedPermanently))
            {
                if (httpWebResponse.Headers["Location"] != null)
                {
                    var location = httpWebResponse.Headers["Location"];
                    fileName = GetFileName(location);
                    return fileName;
                }
            }
            if (httpWebResponse.Headers["content-disposition"] != null)
            {
                var contentDisposition = httpWebResponse.Headers["content-disposition"];
                if (!string.IsNullOrEmpty(contentDisposition))
                {
                    const string lookForFileName = "filename=";
                    var index = contentDisposition.IndexOf(lookForFileName, StringComparison.CurrentCultureIgnoreCase);
                    if (index >= 0)
                        fileName = contentDisposition.Substring(index + lookForFileName.Length);
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Substring(1, fileName.Length - 2);
                    }
                }
            }
            if (string.IsNullOrEmpty(fileName))
            {
                var uri = new Uri(url);

                fileName = Path.GetFileName(uri.LocalPath);
            }
            return fileName;
        }
    }
}
