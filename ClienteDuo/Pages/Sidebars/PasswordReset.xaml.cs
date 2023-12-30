using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ClienteDuo.Pages.Sidebars
{
    /// <summary>
    /// Interaction logic for PasswordReset.xaml
    /// </summary>
    public partial class PasswordReset : Page
    {
        public PasswordReset()
        {
            InitializeComponent();
        }

        private bool IsInputValid()
        {
            bool result = false;
            if (!TBoxNewPassword.Password.Equals(TBoxConfirmNewPassword.Password))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgPasswordDoesNotMatch, MessageBoxImage.Information);
            }
            else if (!IsPasswordSecure(TBoxNewPassword.Password))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgInsecurePassword, MessageBoxImage.Information);
            } 
            else
            {
                result = true;
            }

            return result;
        }

        private bool IsPasswordSecure(string newPassword)
        {
            Regex regex = new Regex("^(?=.*[A-Z])(?=.*[!@?#$&$°¬|/%*])(?=.*[0-9].*[0-9])(?=.*[a-z]).{8,16}$");
            return regex.IsMatch(newPassword);
        }

        private void ModifyPassword(string email, string newPassword)
        {
            bool result = false;
            try
            {
                result = UsersManager.ModifyPasswordByEmail(email, newPassword) == 1;
            }
            catch (CommunicationException)
            {
                SessionDetails.AbortOperation();
            }
            catch (TimeoutException)
            {
                SessionDetails.AbortOperation();
            }

            if (result)
            {
                ReturnToPage();
                MainWindow.ShowMessageBox("password modified succesfully", MessageBoxImage.Information);
            }
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            ReturnToPage();
        }

        private void ReturnToPage()
        {
            if (SessionDetails.IsGuest)
            {
                SessionDetails.CleanSessionDetails();
                Launcher launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
            }
            else
            {
                MainMenu mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }
        }

        private void BtnContinueEvent(object sender, RoutedEventArgs e)
        {
            if (IsInputValid())
            {
                ModifyPassword(SessionDetails.Email, Sha256Encryptor.SHA256_hash(TBoxNewPassword.Password));
            }
        }
    }
}
