using ClienteDuo.Utilities;
using System.Windows;
using System.Windows.Controls;
using ClienteDuo.DataService;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Input;
using System.ServiceModel;

namespace ClienteDuo.Pages.Sidebars
{
    public partial class SidebarAddFriend : UserControl
    {

        public SidebarAddFriend()
        {
            InitializeComponent();
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
        
        private void EnterKeyEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string usernameSender = SessionDetails.Username;
                string usernameReceiver = TBoxUserReceiver.Text.Trim();
                try
                {
                    SendRequest(usernameSender, usernameReceiver);
                }
                catch (CommunicationException)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSendFriendRequestEvent(object sender, RoutedEventArgs e)
        {
            string usernameSender = SessionDetails.Username;
            string usernameReceiver = TBoxUserReceiver.Text.Trim();
            try
            {
                SendRequest(usernameSender, usernameReceiver);
            }
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
            }
        }

        private bool SendRequest(string usernameSender, string usernameReceiver)
        {
            bool result = false;
            if (usernameReceiver == SessionDetails.Username)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgFriendYourself,
                    MessageBoxImage.Information);
            } 
            else if (!IsUsernameTaken(usernameReceiver))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgUsernameDoesNotExist, 
                    MessageBoxImage.Information);
            }
            else if (IsAlreadyFriend(usernameSender, usernameReceiver))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgAlreadyFriends, 
                    MessageBoxImage.Information);
            }
            else if (IsFriendRequestAlreadySent(usernameSender, usernameReceiver))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgFriendRequestAlreadySent, 
                    MessageBoxImage.Information);
            }
            else if (IsUserBlocked(SessionDetails.Username, usernameReceiver) 
                     || IsUserBlocked(usernameReceiver, SessionDetails.Username))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError,
                    MessageBoxImage.Information);
            }
            else
            {
                result = SendFriendRequest(usernameSender, usernameReceiver);
                if (result)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgFriendRequestSent, 
                        MessageBoxImage.Information);
                    Visibility = Visibility.Collapsed;
                }
                TBoxUserReceiver.Clear();
            }
            return result;
        }

        private bool IsUsernameTaken(string username)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsUsernameTaken(username);
        }

        private bool IsUserBlocked(string usernameBlocker, string usernameBlocked)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsUserBlockedByUsername(usernameBlocker, usernameBlocked);
        }

        private bool SendFriendRequest(string usernameSender, string usernameReceiver)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.SendFriendRequest(usernameSender, usernameReceiver);
        }

        private bool IsAlreadyFriend(string usernameSender, string usernameReceiver)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsAlreadyFriend(usernameSender, usernameReceiver);
        }

        private bool IsFriendRequestAlreadySent(string usernameSender, string usernameReceiver)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsFriendRequestAlreadyExistent(usernameSender, usernameReceiver);
        }
    }
}
