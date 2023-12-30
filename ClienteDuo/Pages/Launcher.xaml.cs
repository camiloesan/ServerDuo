using System.Windows;
using System.Windows.Controls;

namespace ClienteDuo.Pages
{
    public partial class Launcher : Page
    {
        public Launcher()
        {
            InitializeComponent();
        }

        private void BtnNewAccountEvent(object sender, RoutedEventArgs e)
        {
            var newAccount = new NewAccount();
            Application.Current.MainWindow.Content = newAccount;
        }

        private void BtnLoginEvent(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            Application.Current.MainWindow.Content = login;
        }

        private void BtnJoinAsGuestEvent(object sender, RoutedEventArgs e)
        {
            var joinParty = new JoinParty();
            Application.Current.MainWindow.Content = joinParty;
        }

        private void BtnLocalizationEnUSEvent(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            var launcher = new Launcher();
            Application.Current.MainWindow.Content = launcher;
        }

        private void BtnLocalizationEsMXEvent(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es");
            var launcher = new Launcher();
            Application.Current.MainWindow.Content = launcher;
        }

        private void BtnLocalizationFrFREvent(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr");
            var launcher = new Launcher();
            Application.Current.MainWindow.Content = launcher;
        }
    }
}
