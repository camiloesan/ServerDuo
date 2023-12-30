using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClienteDuo.Pages.Sidebars;
using System.Threading;
using System.Threading.Tasks;

namespace ClienteDuo.Pages
{
    public partial class InviteeLobby : Page, IPartyManagerCallback
    {
        const int MESSAGE_MAX_LENGTH = 250;
        private readonly bool _isWpfRunning = true;
        private readonly PartyManagerClient _partyManagerClient;
        private PopUpUserDetails _popUpUserDetails;
        
        public InviteeLobby(string username)
        {
            InitializeComponent();
            var instanceContext = new InstanceContext(this);
            _partyManagerClient = new PartyManagerClient(instanceContext);
            JoinGame(SessionDetails.PartyCode, username);
            LoadPopUpDetailsComponent();
        }

        public InviteeLobby()
        {
            var instanceContext = new InstanceContext(this);
            _partyManagerClient = new PartyManagerClient(instanceContext);
            try
            {
                _ = Application.Current.Windows;
            }
            catch
            {
                _isWpfRunning = false;
            }
        }

        private void LoadPopUpDetailsComponent()
        {
            _popUpUserDetails = new PopUpUserDetails
            {
                Width = 350,
                Height = 200,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = Visibility.Collapsed
            };
            masterGrid.Children.Add(_popUpUserDetails);
        }

        public void JoinGame(int partyCode, string username)
        {
            SessionDetails.PartyCode = partyCode;
            SessionDetails.Username = username;
            _partyManagerClient.NotifyJoinParty(partyCode, username);
        }

        private void CreatePlayerPanel(string username)
        {
            var backgroundColor = 
                username == SessionDetails.Username 
                ? new SolidColorBrush(Colors.Gold)
                : new SolidColorBrush(Colors.DimGray);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = backgroundColor,
                Margin = new Thickness(15, 20, 15, 20),
                Width = 200,
                Height = 40,
            };
            playersPanel.Children.Add(stackPanel);

            var usernameName = new Label
            {
                Foreground = new SolidColorBrush(Colors.Black),
                Content = username,
                Margin = new Thickness(10, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanel.Children.Add(usernameName);

            if (username == SessionDetails.Username) return;
            
            var btnViewProfile = new Button
            {
                Content = Properties.Resources.BtnProfile,
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                DataContext = username
            };
            btnViewProfile.Click += ViewProfileEvent;
            
            if (!username.Contains("guest"))
            {
                stackPanel.Children.Add(btnViewProfile);
            }
        }
        
        private void ViewProfileEvent(object sender, RoutedEventArgs e)
        {
            string username = ((FrameworkElement)sender).DataContext as string;
            _popUpUserDetails.SetDataContext(username, false);
            _popUpUserDetails.Visibility = Visibility.Visible;
        }

        private void OnEnterSendMessage(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && TBoxMessage.Text.Trim().Length > 0)
            {
                string message = SessionDetails.Username + ": " + TBoxMessage.Text;
                SendMessage(SessionDetails.PartyCode, message);
                TBoxMessage.Text = "";
            }
            else if (TBoxMessage.Text.Length > MESSAGE_MAX_LENGTH)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgMessageMaxCharacters);
            }
        }
        public void SendMessage(int partyCode, string message)
        {
            _partyManagerClient.NotifySendMessage(partyCode, message);
        }

        private void UpdatePlayerList(Dictionary<string, object> playersInLobby)
        {
            playersPanel.Children.Clear();
            foreach (var player in playersInLobby)
            {
                CreatePlayerPanel(player.Key);
            }
        }

        private void BtnExitLobby(object sender, RoutedEventArgs e)
        {
            if (SessionDetails.IsGuest)
            {
                var launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
            }
            else
            {
                var mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }

            ExitParty(SessionDetails.PartyCode, SessionDetails.Username);
        }

        public void ExitParty(int partyCode, string username)
        {
            _partyManagerClient.NotifyLeaveParty(partyCode, username);
        }

        public void MessageReceived(string messageSent)
        {
            var labelMessageReceived = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 14,
                Content = messageSent
            };

            chatPanel.Children.Add(labelMessageReceived);
            chatScrollViewer.ScrollToEnd();
        }

        public void PartyCreated(Dictionary<string, object> playersInLobby)
        {
            throw new NotImplementedException();
        }

        public void PlayerLeft(Dictionary<string, object> playersInLobby)
        {
            if (!_isWpfRunning) return;
            
            MusicManager.PlayPlayerLeftSound();
            UpdatePlayerList(playersInLobby);
        }

        public void PlayerJoined(Dictionary<string, object> playersInLobby)
        {
            if (!_isWpfRunning) return;
            
            MusicManager.PlayPlayerJoinedSound();
            UpdatePlayerList(playersInLobby);
        }

        public async void GameStarted()
        {
            CardTable cardTable = new CardTable();
            InstanceContext instanceContext = new InstanceContext(cardTable);
            DataService.MatchManagerClient client = new DataService.MatchManagerClient(instanceContext);
            client.Subscribe(SessionDetails.PartyCode, SessionDetails.Username);

            await Task.Delay(5000);
            
            cardTable.LoadPlayers();
            cardTable.UpdateTableCards();
            App.Current.MainWindow.Content = cardTable;
        }

        public void PlayerKicked()
        {
            if (!_isWpfRunning) return;
            
            if (SessionDetails.IsGuest)
            {
                var launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
            }
            else
            {
                var mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }
        }
    }
}
