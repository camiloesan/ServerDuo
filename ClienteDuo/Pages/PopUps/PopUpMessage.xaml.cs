using ClienteDuo.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClienteDuo.Pages.Sidebars
{
    public partial class PopUpMessage : Window
    {
        public PopUpMessage()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Closing += OnWindowClosing;
        }

        private void BtnOkEvent(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.IsEnabled = true;
            Close();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.IsEnabled = true;
        }

        public string Message { get { return LblMessage.Text; } set { LblMessage.Text = value; } }
    }
}
