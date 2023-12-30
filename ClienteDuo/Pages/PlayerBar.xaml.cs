using ClienteDuo.DataService;
using ClienteDuo.Pages.Sidebars;
using ClienteDuo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public string Username {
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
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp" + pictureId + ".png"));

            ProfilePicture.Background = imageBrush;
        }

        private void BtnAddFriendEvent(object sender, RoutedEventArgs e)
        {
            UsersManagerClient client = new UsersManagerClient();
            if (client.SendFriendRequest(SessionDetails.Username, _username))
            {
                BtnAddFriend.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindow.ShowMessageBox("There was an error sending friend request to this user", MessageBoxImage.Error);
            }
        }

        private void BtnKickEvent(object sender, RoutedEventArgs e)
        {
            PopUpKickPlayer popUpKickPlayer = new PopUpKickPlayer();
            popUpKickPlayer.KickedUsername = _username;
            popUpKickPlayer.SetClient(_client);
            popUpKickPlayer.Show();
        }
    }
}
