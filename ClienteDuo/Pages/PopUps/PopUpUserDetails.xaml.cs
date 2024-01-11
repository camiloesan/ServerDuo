using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClienteDuo.Pages.Sidebars
{
    public partial class PopUpUserDetails : UserControl
    {
        private string _userSelectedName;

        public PopUpUserDetails()
        {
            InitializeComponent();
        }

        public void InitializeUserInfo(string username)
        {
            UserDTO userInfo = null;
            try
            {
                userInfo = UsersManager.GetUserInfoByUsername(username);
            }
            catch (CommunicationException)
            {
                MessageBox.Show(Properties.Resources.DlgConnectionError);
            }
            catch (TimeoutException)
            {
                MessageBox.Show(Properties.Resources.DlgConnectionError);
            }

            _userSelectedName = username;

            if (userInfo != null)
            {
                SetProfilePicture(userInfo.PictureID);
                LblUsername.Content = Properties.Resources.LblUsername + ": " + username;
                LblTrophies.Content = Properties.Resources.LblTotalWins + ": " + userInfo.TotalWins;
                bool isFriend = FriendsManager.IsAlreadyFriend(SessionDetails.Username, username);
                if (isFriend)
                {
                    BtnAddFriend.Visibility = Visibility.Collapsed;
                    BtnBlock.Visibility = Visibility.Collapsed;
                }
                if (SessionDetails.IsGuest)
                {
                    BtnAddFriend.Visibility = Visibility.Collapsed;
                    BtnBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void InitializeUserInfo(int friendshipId, string username)
        {
            UserDTO userInfo = null;
            try
            {
                userInfo = UsersManager.GetUserInfoByUsername(username);
            }
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
            }
            catch (TimeoutException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
            }

            if (userInfo != null) 
            {
                DataContext = friendshipId;
                _userSelectedName = username;

                SetProfilePicture(userInfo.PictureID);
                LblUsername.Content = Properties.Resources.LblUsername + ": " + username;
                LblTrophies.Content = Properties.Resources.LblTotalWins + ": " + userInfo.TotalWins;
                BtnAddFriend.Visibility = Visibility.Collapsed;
            }
        }

        private void SetProfilePicture(int pictureId)
        {
            BitmapImage bitmapImage = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp0.png"));
            switch (pictureId)
            {
                case 0:
                    bitmapImage = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp0.png"));
                    break;
                case 1:
                    bitmapImage = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp1.png"));
                    break;
                case 2:
                    bitmapImage = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp2.png"));
                    break;
                case 3:
                    bitmapImage = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp3.png"));
                    break;
            }
            ImageProfilePicture.Source = bitmapImage;
            ImageProfilePicture.Stretch = Stretch.UniformToFill;
        }

        private void BtnAddFriendEvent(object sender, RoutedEventArgs e)
        {
            string usernameSender = SessionDetails.Username;
            string usernameReceiver = DataContext as string;

            try
            {
                AddFriend(usernameSender, usernameReceiver);
            }
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
            }
            catch (TimeoutException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
            }
        }

        private void AddFriend(string usernameSender, string usernameReceiver)
        {
            if (FriendsManager.IsFriendRequestAlreadySent(usernameSender, usernameReceiver))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgFriendRequestAlreadySent, MessageBoxImage.Information);
            }
            else
            {
                if (FriendsManager.SendFriendRequest(usernameSender, usernameReceiver) == 1)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgFriendRequestSent, MessageBoxImage.Information);
                    Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BtnBlockEvent(object sender, RoutedEventArgs e)
        {
            bool confirmation = MainWindow.ShowConfirmationBox(Properties.Resources.DlgBlockConfirmation);
            if (confirmation)
            {
                bool isFriend = false;
                try
                {
                    isFriend = FriendsManager.IsAlreadyFriend(SessionDetails.Username, _userSelectedName);
                }
                catch (CommunicationException)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
                }
                catch (TimeoutException)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
                }

                if (isFriend)
                {
                    int friendshipId = (int)DataContext;
                    FriendsManager.DeleteFriendshipById(friendshipId);
                }

                if (!BlockManager.IsUserBlocked(SessionDetails.Username, _userSelectedName))
                {
                    int result = 0;
                    try
                    {
                        result = BlockUser(SessionDetails.Username, _userSelectedName);
                    }
                    catch (CommunicationException)
                    {
                        MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
                    }
                    catch (TimeoutException)
                    {
                        MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
                    }

                    //if it was blocked 1, if it was blocked and therefore banned 2, else 0.
                    switch (result) { 
                        case 1:
                            MainWindow.ShowMessageBox(Properties.Resources.DlgBlockedUser, MessageBoxImage.Exclamation);
                            break;
                        case 2:
                            MainWindow.ShowMessageBox(Properties.Resources.DlgUserBanned, MessageBoxImage.Exclamation);
                            break;
                        default:
                            MainWindow.ShowMessageBox(Properties.Resources.DlgCouldntBlockUser, MessageBoxImage.Exclamation);
                            break;
                    }
                }
                else
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgUserAlreadyBlocked, MessageBoxImage.Exclamation);
                }
            }
        }

        private int BlockUser(string usernameSender, string usernameReceiver)
        {
            int result = 0;
            try
            {
                result = BlockManager.BlockUserByUsername(usernameSender, usernameReceiver);
            }
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
            }
            catch (TimeoutException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
            }

            if (result > 0 && SessionDetails.LobbyCode == 0)
            {
                var mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }
            return result;
        }


        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
