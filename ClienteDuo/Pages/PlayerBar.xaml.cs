using ClienteDuo.DataService;
using ClienteDuo.Pages.Sidebars;
using ClienteDuo.Utilities;
using System.ServiceModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClienteDuo.Pages
{
    public partial class PlayerBar : UserControl
    {
        private string _username;
        private MatchManagerClient _client;

        public PlayerBar()
        {
            InitializeComponent();
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                LblUsername.Content = _username;
            }
        }

        public void SetClient(MatchManagerClient client)
        {
            _client = client;
        }

        public void SetProfilePicture(int pictureId)
        {
            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp" + pictureId + ".png"))
            };

            ProfilePicture.Background = imageBrush;
        }

        private void BtnAddFriendEvent(object sender, RoutedEventArgs e)
        {
            bool result = false;
            try
            {
                result = UsersManager.SendFriendRequest(SessionDetails.Username, _username) == 1;
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
                BtnAddFriend.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgFriendRequestError, MessageBoxImage.Error);
            }
        }

        private void BtnKickEvent(object sender, RoutedEventArgs e)
        {
            PopUpKickPlayer popUpKickPlayer = new PopUpKickPlayer
            {
                KickedUsername = _username
            };
            popUpKickPlayer.SetClient(_client);
            popUpKickPlayer.Show();
        }
    }
}
