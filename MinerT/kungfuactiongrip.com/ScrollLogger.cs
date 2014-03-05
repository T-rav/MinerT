using System;
using System.Windows.Controls;

namespace MinerT.kungfuactiongrip.com
{
    public class ScrollLogger
    {
        private readonly TextBox _logTo;
        private readonly ScrollViewer _scroller;
        private readonly double _origHeight;

        private int _lineCnt;
        private bool _addForScroll;

        private static readonly object Lock = new object();

        public ScrollLogger(TextBox logTo, ScrollViewer sv)
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
