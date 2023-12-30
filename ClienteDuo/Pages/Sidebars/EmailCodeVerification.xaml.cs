using ClienteDuo.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace ClienteDuo.Pages.Sidebars
{
    /// <summary>
    /// Interaction logic for EmailCodeVerification.xaml
    /// </summary>
    public partial class EmailCodeVerification : Page
    {
        public EmailCodeVerification(string email, int confirmationCode)
        {
            InitializeComponent();
            SessionDetails.Email = email;
            DataContext = confirmationCode;
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            if (SessionDetails.IsGuest)
            {
                SessionDetails.CleanSessionDetails();
                Login login = new Login();
                Application.Current.MainWindow.Content = login;
            }
            else
            {
                MainMenu mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }
        }

        private void BtnContinueEvent(object sender, RoutedEventArgs e)
        {
            ValidateCode();
        }

        private void ValidateCode()
        {
            if (!int.TryParse(TBoxConfirmationCode.Text, out int inputCode))
            {
                MainWindow.ShowMessageBox("invalid format", MessageBoxImage.Warning);
                return;
            }

            if (inputCode == (int)DataContext) 
            {
                PasswordReset passwordReset = new PasswordReset();
                Application.Current.MainWindow.Content = passwordReset;
            } 
            else
            {
                MainWindow.ShowMessageBox("wrong code", MessageBoxImage.Warning);
            }
        }
    }
}
