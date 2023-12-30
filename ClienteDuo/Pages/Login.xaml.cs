using ClienteDuo.DataService;
using ClienteDuo.Pages.Sidebars;
using ClienteDuo.Utilities;
using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ClienteDuo.Pages
{
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void BtnLoginEvent(object sender, RoutedEventArgs e)
        {
            CreateSession();
        }

        private void EnterKeyEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CreateSession();
            }
        }

        private void CreateSession()
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            string username = TBoxUsername.Text;
            string password = TBoxPassword.Password;
            
            UserDTO loggedUser = null;
            try
            {
                loggedUser = AreCredentialsValid(username, password);
            }
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
            }

            if (loggedUser == null)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgFailedLogin, MessageBoxImage.Warning);
            }
            else if (IsUserLoggedIn(loggedUser.ID))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgUserAlreadyLoggedIn, MessageBoxImage.Warning);
            }
            else
            {
                SessionDetails.UserId = loggedUser.ID;
                SessionDetails.Username = loggedUser.UserName;
                SessionDetails.Email = loggedUser.Email;
                SessionDetails.TotalWins = loggedUser.TotalWins;
                SessionDetails.PictureID = loggedUser.PictureID;
                SessionDetails.IsGuest = false;

                UserDTO user = new UserDTO
                {
                    UserName = SessionDetails.Username,
                    ID = SessionDetails.UserId
                };

                MainMenu mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
                MainWindow.NotifyLogin(user);
            }
        }

        private bool IsUserLoggedIn(int userId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsUserAlreadyLoggedIn(userId);
        }

        public UserDTO AreCredentialsValid(string username, string password)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsLoginValid(username, Sha256Encryptor.SHA256_hash(password));
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            Launcher launcher = new Launcher();
            Application.Current.MainWindow.Content = launcher;
        }

        private void LblResetPasswordEvent(object sender, MouseEventArgs e)
        {
            EmailConfirmation emailConfirmation = new EmailConfirmation();
            Application.Current.MainWindow.Content = emailConfirmation;
        }
    }
}
