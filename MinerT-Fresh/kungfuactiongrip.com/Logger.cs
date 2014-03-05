using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MinerT.kungfuactiongrip.com
{
    public class Logger
    {
        private readonly TextBox _logTo;
        private readonly ScrollViewer _scroller;
        private readonly double _origHeight;

        private int _lineCnt;
        private bool _addForScroll;

        private static readonly object Lock = new object();

        public Logger(TextBox logTo, ScrollViewer sv)
        {
            _logTo = logTo;
            _scroller = sv;
            _origHeight = _logTo.Height;
        }

        public void WriteLine(string msg)
        {
            lock (Lock)
            {
                ClearBuffer();

                if (_logTo != null)
                {
                    _logTo.Text += msg;
                    _logTo.Text += Environment.NewLine;
                }

                ScrollIntoView();
            }
        }

        public void WriteError(string msg)
        {
            lock (Lock)
            {
                ClearBuffer();

                if (_logTo != null)
                {
                    _logTo.Text += "ERROR : " + msg;
                    _logTo.Text += Environment.NewLine;
                }

                ScrollIntoView();
            }
        }

        private const string WebServiceUri = "http://www.kungfuactiongrip.com/minerT/";
        public static void LogToServer(string user, string msg)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var url = WebServiceUri + "logMsg.php?user=" + user + "&msg=" + msg;
                    var request = WebRequest.Create(url);
                    using (var response = request.GetResponse())
                    {

                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            });
        }

        internal void ClearBuffer()
        {
            if (_lineCnt >= 250)
            {
                _logTo.Height = _origHeight;
                _lineCnt = 0;
                _addForScroll = false;
                _logTo.Text = string.Empty;
            }
        }

        internal void ScrollIntoView()
        {
            // adjust for auto scroll ;)
            if (_lineCnt >= 15 && !_addForScroll)
            {
                _addForScroll = true;
            }
            else
            {
                _lineCnt++;
            }

            if (_logTo != null && _addForScroll)
            {
                _logTo.Height += 13.3;
            }

            if (_scroller != null)
            {
                _scroller.ScrollToEnd();
            }
        }
    }
}
