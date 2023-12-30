using ClienteDuo.Pages;
using ClienteDuo.Utilities;
using System;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ClienteDuo.DataService;
using System.Windows.Media;
using System.ServiceModel;

namespace ClienteDuo
{
    public partial class NewAccount : Page
    {
        private const int MAX_LENGTH_EMAIL = 30;

        public NewAccount()
        {
            InitializeComponent();
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            var launcher = new Launcher();
            Application.Current.MainWindow.Content = launcher;
        }

        private void BtnContinueEvent(object sender, RoutedEventArgs e)
        {
            LogIn();
        }

        private void LogIn()
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
            
            if (areFieldsValid)
            {
                bool result = false;
                try
                {
                    result = AddUserToDatabase(username, email, password);
                }
                catch (CommunicationException)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
                }

                if (result)
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

            bool result = true;
            if (AreFieldsEmpty())
            {
                TBoxUsername.BorderBrush = new SolidColorBrush(Colors.Red);
                TBoxEmail.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgEmptyFields, MessageBoxImage.Warning);
                return false;
            }

            if (!IsUsernameValid(username))
            {
                TBoxUsername.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgUsernameInvalid, MessageBoxImage.Warning);
                return false;
            }

            if (!IsPasswordSecure(password))
            {
                TBoxPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgInsecurePassword, MessageBoxImage.Warning);
                return false;
            }

            if (!IsPasswordMatch(password, confirmedPassword))
            {
                TBoxPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                TBoxConfirmPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgPasswordDoesNotMatch, MessageBoxImage.Warning);
                return false;
            }

            if (IsUsernameTaken(username))
            {
                TBoxUsername.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgUsernameTaken, MessageBoxImage.Warning);
                return false;
            }

            if (!IsEmailValid(email))
            {
                TBoxEmail.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgEmailInvalid, MessageBoxImage.Warning);
                return false;
            }

            if (IsEmailTaken(email))
            {
                TBoxEmail.BorderBrush = new SolidColorBrush(Colors.Red);
                MainWindow.ShowMessageBox(Properties.Resources.DlgEmailTaken, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public bool IsUsernameValid(string username)
        {
            var regex = new Regex("^[a-zA-Z0-9]{3,14}$");
            return regex.IsMatch(username) && !username.Contains("guest");
        }

        public bool IsEmailValid(string email)
        {
            var regex = new Regex("^[\\w.%+-]+@[\\w.-]+\\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email) && email.Length <= MAX_LENGTH_EMAIL;
        }

        public bool IsPasswordMatch(string password, string confirmedPassword)
        {
            return password.Equals(confirmedPassword);
        }

        public bool IsPasswordSecure(string password)
        {
            var regex = new Regex("^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9].*[0-9])(?=.*[a-z]).{8,16}$");
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

        public bool IsUsernameTaken(string username)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsUsernameTaken(username);
        }

        public bool IsEmailTaken(string email)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsEmailTaken(email);
        }

        public bool AddUserToDatabase(string username, string email, string password)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            UserDTO databaseUser = new UserDTO
            {
                UserName = username,
                Email = email,
                Password = Sha256Encryptor.SHA256_hash(password)
            };

            return usersManagerClient.AddUserToDatabase(databaseUser) > 0;
        }
    }
}
