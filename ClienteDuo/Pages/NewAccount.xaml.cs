using ClienteDuo.DataService;
using ClienteDuo.Pages;
using ClienteDuo.Utilities;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Regex = System.Text.RegularExpressions.Regex;
using System;

namespace ClienteDuo
{
    public partial class NewAccount : Page
    {
        public NewAccount()
        {
            InitializeComponent();
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Launcher();
        }

        private void BtnContinueEvent(object sender, RoutedEventArgs e)
        {
            AddUser();
        }
        
        private void EnterKeyEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AddUser();
            }
        }

        private void AddUser()
        {
            string username = TBoxUsername.Text.Trim();
            string email = TBoxEmail.Text.Trim();
            string password = TBoxPassword.Password.Trim();

            bool areFieldsValid = false;
            try
            {
                areFieldsValid = AreFieldsValid();
            }
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
            }
            catch (TimeoutException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
            }

            if (areFieldsValid)
            {
                int result = 0;
                try
                {
                    result = UsersManager.AddUserToDatabase(username, email, password);
                }
                catch (CommunicationException)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
                }

                if (result == 1)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgNewAccountSuccess, MessageBoxImage.Information);
                    var launcher = new Launcher();
                    Application.Current.MainWindow.Content = launcher;
                }
            }
        }

        private bool AreFieldsValid()
        {
            string username = TBoxUsername.Text.Trim();
            string email = TBoxEmail.Text.Trim();
            string password = TBoxPassword.Password.Trim();
            string confirmedPassword = TBoxConfirmPassword.Password.Trim();

            bool result = false;
            if (AreFieldsEmpty())
            {
                TBoxUsername.BorderBrush = new SolidColorBrush(Colors.Red);
                TBoxEmail.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgEmptyFields, MessageBoxImage.Warning);
            }
            else if (!IsUsernameValid(username))
            {
                TBoxUsername.BorderBrush = new SolidColorBrush(Colors.Red);
                TBoxEmail.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgUsernameInvalid, MessageBoxImage.Warning);
            }
            else if (!IsPasswordSecure(password))
            {
                TBoxPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgInsecurePassword, MessageBoxImage.Warning);
            }
            else if (!IsPasswordMatch(password, confirmedPassword))
            {
                TBoxPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                TBoxConfirmPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgPasswordDoesNotMatch, MessageBoxImage.Warning);
            }
            else if (UsersManager.IsUsernameTaken(username))
            {
                TBoxUsername.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgUsernameTaken, MessageBoxImage.Warning);
            }
            else if (!IsEmailValid(email))
            {
                TBoxEmail.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgEmailInvalid, MessageBoxImage.Warning);
            }
            else if (UsersManager.IsEmailTaken(email))
            {
                TBoxEmail.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgEmailTaken, MessageBoxImage.Warning);
            }
            else
            {
                result = true;
            }
            return result;
        }

        public bool IsUsernameValid(string username)
        {
            var regex = new Regex("^[a-zA-Z0-9]{3,14}$");
            return regex.IsMatch(username) && !username.Contains("guest");
        }

        public bool IsEmailValid(string email)
        {
            var regex = new Regex("^(?=.{5,30}$)[\\w.%+-]+@[\\w.-]+\\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }

        public bool IsPasswordMatch(string password, string confirmedPassword)
        {
            return password.Equals(confirmedPassword);
        }

        public bool IsPasswordSecure(string password)
        {
            var regex = new Regex("^(?=.*[A-Z])(?=.*[!@?#$&$°¬|/%*])(?=.*[0-9].*[0-9])(?=.*[a-z]).{8,16}$");
            return regex.IsMatch(password);
        }

        private bool AreFieldsEmpty()
        {
            string usernameField = TBoxUsername.Text;
            string emailField = TBoxEmail.Text;
            string passwordField = TBoxPassword.Password;
            string confirmPasswordField = TBoxConfirmPassword.Password;

            return string.IsNullOrEmpty(usernameField)
                   || string.IsNullOrEmpty(emailField)
                   || string.IsNullOrEmpty(passwordField)
                   || string.IsNullOrEmpty(confirmPasswordField);
        }
    }
}
