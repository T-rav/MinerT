using System.Windows;
using AutoUpdaterDotNET;

namespace UpdateBootstraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Update(string url, string appName, string appLoc, string appCompany, string version)
        {
            AutoUpdater.Start(url, appName, appLoc, appCompany, version);
        }
    }
}
