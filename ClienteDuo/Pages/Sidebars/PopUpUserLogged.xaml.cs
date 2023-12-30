using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ClienteDuo.Pages.Sidebars
{
    /// <summary>
    /// Interaction logic for PopUpUserLogged.xaml
    /// </summary>
    public partial class PopUpUserLogged : UserControl
    {
        public PopUpUserLogged()
        {
            InitializeComponent();
        }

        public void SetLabelText(string username)
        {
            LblMessage.Content = username + " " + Properties.Resources.DlgIsNowActive;
        }
    }
}
