using ClienteDuo.DataService;
using ClienteDuo.Pages.Sidebars;
using ClienteDuo.Utilities;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            string username = TBoxUsername.Text;
            string password = TBoxPassword.Password;

            UserDTO loggedUser = null;
            bool result = true;
            bool isUserBanned = false;
            try
            {
                loggedUser = UsersManager.AreCredentialsValid(username, password);
                if (loggedUser != null)
                {
                    isUserBanned = UsersManager.IsUserBanned(loggedUser.ID);
                }
            }
            catch (CommunicationException)
            {
                result = false;
                SessionDetails.AbortOperation();
            }
            catch (TimeoutException)
            {
                result = false;
                SessionDetails.AbortOperation();
            }

            if (result)
            {
                if (loggedUser == null)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgFailedLogin, MessageBoxImage.Warning);
                }
                else if (isUserBanned)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgLoginBanned, MessageBoxImage.Warning);
                }
                else if (UsersManager.IsUserLoggedIn(loggedUser.UserName))
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
                    Application.Current.MainWindow.Content = new MainMenu();

                    var userConnectionHandler = new UserConnectionHandlerClient();
                    try
                    {
                        userConnectionHandler.NotifyLogIn(user);
                    }
                    catch (CommunicationException)
                    {
                        SessionDetails.AbortOperation();
                    }
                    catch (TimeoutException)
                    {
                        SessionDetails.AbortOperation();
                    }
                }
            }
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
