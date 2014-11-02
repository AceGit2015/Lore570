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

namespace GPSTrack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVM m_ctx;

        public MainWindow()
        {
            InitializeComponent();

            m_ctx = DataContext as MainVM;
        }

        private void OpenGPSLogFile(object sender, System.Windows.RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".log";
            ofd.Filter = "Log Files(*.log)|*.log|All Files(*.*)|*.*||";
            if (ofd.ShowDialog() == true) {
                System.Diagnostics.Debugger.Log(1, null, ofd.FileName);
                m_ctx.LogFileName = ofd.FileName;
            }
        }

        private void Play(object sender, System.Windows.RoutedEventArgs e)
        {
            m_ctx.Play();
        }

        private void Stop(object sender, System.Windows.RoutedEventArgs e)
        {
        	m_ctx.Stop();
        }
    }
}
