using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IliadWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IliadScraper _iliadScraper;
        private DispatcherTimer _timer;
        private IliadData _data;
        public MainWindow()
        {
            InitializeComponent();

            _iliadScraper = new IliadScraper("username", "password");
            _data = _iliadScraper.GetIliadDataAsync().Result;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(1);
            //_timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _data = _iliadScraper.GetIliadDataAsync().Result;
            /*
            DataUsageLabel.Content = _data.DataUsage;
            DataLimitLabel.Content = _data.DataLimit;
            ExpirationDateLabel.Content = _data.ExpirationDate;
            */
        }
    }
}
