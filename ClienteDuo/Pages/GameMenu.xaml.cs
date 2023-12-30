using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClienteDuo.Pages
{
    public partial class GameMenu : UserControl
    {
        private MatchManagerClient _client;

        public GameMenu()
        {
            InitializeComponent();

            PlayerBar yourBar = new PlayerBar();
            yourBar.Username = SessionDetails.Username;
            yourBar.BtnAddFriend.Visibility = Visibility.Collapsed;
            yourBar.BtnKick.Visibility = Visibility.Collapsed;
            yourBar.SetProfilePicture(SessionDetails.PictureID);
            yourBar.Background = new SolidColorBrush(Colors.Gold);
            yourBar.Visibility = Visibility.Visible;

            playerStackPanel.Children.Add(yourBar);
        }

        public void LoadPlayers(List<string> playerList, MatchManagerClient client)
        {
            foreach (string playerUsername in playerList)
            {
                if (!playerUsername.Equals(SessionDetails.Username))
                {
                    PlayerBar playerBar = new PlayerBar();
                    playerBar.Username = playerUsername;
                    playerBar.SetClient(client);

                    if (SessionDetails.IsHost)
                    {
                        playerBar.BtnKick.Visibility = Visibility.Visible;
                    }

                    if (SessionDetails.IsGuest)
                    {
                        playerBar.BtnAddFriend.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (!playerUsername.Contains("guest"))
                        {
                            UsersManagerClient userClient = new UsersManagerClient();

                            if (userClient.IsAlreadyFriend(SessionDetails.Username, playerBar.Username) ||
                                userClient.IsFriendRequestAlreadyExistent(SessionDetails.Username, playerUsername))
                            {
                                UserDTO userData = userClient.GetUserInfoByUsername(playerUsername);

                                playerBar.SetProfilePicture(userData.PictureID);
                                playerBar.BtnAddFriend.Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            playerBar.BtnAddFriend.Visibility = Visibility.Collapsed;
                        }
                    }

                    playerBar.Visibility = Visibility.Visible;
                    playerBar.Name = playerUsername;

                    playerStackPanel.Children.Add(playerBar);
                }
            }
        }

        public void RemovePlayer(string username)
        {
            PlayerBar kickedPlayer = new PlayerBar();

            foreach (PlayerBar playerBar in playerStackPanel.Children)
            {
                if (playerBar.Username.Equals(username))
                {
                    kickedPlayer = playerBar;
                }
            }

            playerStackPanel.Children.Remove(kickedPlayer);
        }

        public void setClient(MatchManagerClient client)
        {
            _client = client;
        }

        private void BtnExitEvent(object sender, RoutedEventArgs e)
        {
            if (MainWindow.ShowConfirmationBox(Properties.Resources.DlgExitMatchConfirmation))
            {
                _client.ExitMatch(SessionDetails.PartyCode, SessionDetails.Username);

                if (SessionDetails.IsGuest)
                {
                    Launcher launcher = new Launcher();

                    App.Current.MainWindow.Content = launcher;
                }
                else
                {
                    MainMenu mainMenu = new MainMenu();

                    App.Current.MainWindow.Content = mainMenu;
                }
            }
        }

        private void BtnHideMenuEvent(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
