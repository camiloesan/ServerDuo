using System.Threading.Tasks;
using ClienteDuo.Pages.Sidebars;
using ClienteDuo.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System;
using System.ServiceModel;
using ClienteDuo.DataService;

namespace ClienteDuo.Pages
{
    public partial class MainMenu : Page
    {
        private static PopUpUserDetails _popUpUserDetails;
        private SidebarUserProfile _sidebarUserProfile;
        private SidebarFriends _sidebarFriends;
        private SidebarLeaderboard _sidebarLeaderboard;

        public MainMenu()
        {
            InitializeComponent();
            InitializeAddOns();
        }

        public static void ShowPopUpUserDetails(int friendshipId, string username)
        {
            _popUpUserDetails.InitializeUserInfo(friendshipId, username);
            _popUpUserDetails.Visibility = Visibility.Visible;
        }

        private void InitializeAddOns()
        {
            _popUpUserDetails = new PopUpUserDetails
            {
                Width = 350,
                Height = 240,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = Visibility.Collapsed
            };
            MainGrid.Children.Add(_popUpUserDetails);
        }

        private void BtnLogOutEvent(object sender, RoutedEventArgs e)
        {
            bool confirmation = MainWindow.ShowConfirmationBox(Properties.Resources.DlgLogOutConfirmation);
            if (confirmation)
            {
                UserConnectionHandlerClient client = new UserConnectionHandlerClient();
                try
                {
                    client.LogOut(SessionDetails.Username);
                }
                catch (CommunicationException)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
                }
                catch (TimeoutException)
                {
                    MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
                }
                SessionDetails.CleanSessionDetails();
                Application.Current.MainWindow.Content = new Launcher();
            }
        }

        private void BtnNewPartyEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                Lobby lobby = new Lobby(SessionDetails.Username);
                Application.Current.MainWindow.Content = lobby;
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

        private void BtnJoinPartyEvent(object sender, RoutedEventArgs e)
        {
            JoinParty joinParty = new JoinParty();
            Application.Current.MainWindow.Content = joinParty;
        }

        private void BtnMyProfileSidebarEvent(object sender, RoutedEventArgs e)
        {
            _sidebarUserProfile = new SidebarUserProfile
            {
                Width = 250,
                Height = 565,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Visibility = Visibility.Collapsed
            };
            MainGrid.Children.Add(_sidebarUserProfile);
            _sidebarUserProfile.Visibility = Visibility.Visible;
        }

        private void BtnFriendsSidebarEvent(object sender, RoutedEventArgs e)
        {
            _sidebarFriends = new SidebarFriends
            {
                Width = 250,
                Height = 565,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Visibility = Visibility.Collapsed
            };
            MainGrid.Children.Add(_sidebarFriends);
            _sidebarFriends.Visibility = Visibility.Visible;
        }

        private void BtnLeaderboardEvent(object sender, RoutedEventArgs e)
        {
            _sidebarLeaderboard = new SidebarLeaderboard
            {
                Width = 250,
                Height = 565,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Visibility = Visibility.Collapsed
            };
            MainGrid.Children.Add(_sidebarLeaderboard);
            _sidebarLeaderboard.Visibility = Visibility.Visible;
        }
    }
}
