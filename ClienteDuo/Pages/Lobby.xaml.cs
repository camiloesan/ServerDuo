using ClienteDuo.DataService;
using ClienteDuo.Pages.Sidebars;
using ClienteDuo.Utilities;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClienteDuo.Pages
{
    public partial class Lobby : Page, ILobbyManagerCallback
    {
        private readonly LobbyManagerClient _lobbyManagerClient;
        private readonly LobbyValidatorClient _lobbyValidatorClient = new LobbyValidatorClient();
        private PopUpUserDetails _popUpUserDetails;

        public Lobby(string hostUsername)
        {
            InitializeComponent();
            SessionDetails.IsHost = true;
            InstanceContext instanceContext = new InstanceContext(this);
            _lobbyManagerClient = new LobbyManagerClient(instanceContext);

            int lobbyCode;
            lobbyCode = CreateNewLobby(hostUsername);

            if (lobbyCode != 0)
            {
                LoadVisualComponents();
                MusicManager.PlayPlayerJoinedSound();
            }
        }

        public Lobby(string username, int lobbyCode)
        {
            InitializeComponent();
            SessionDetails.IsHost = false;
            InstanceContext instanceContext = new InstanceContext(this);
            _lobbyManagerClient = new LobbyManagerClient(instanceContext);
            JoinGame(lobbyCode, username);
            LoadVisualComponents();
            MusicManager.PlayPlayerJoinedSound();
        }

        public int CreateNewLobby(string hostUsername)
        {
            SessionDetails.LobbyCode = GenerateNewLobbyCode();
            SessionDetails.Username = hostUsername;
            _lobbyManagerClient.NotifyCreateLobby(SessionDetails.LobbyCode, SessionDetails.Username);

            return SessionDetails.LobbyCode;
        }

        private int GenerateNewLobbyCode()
        {
            Random random = new Random();
            int randomCode = random.Next(1000, 10000);

            bool IsLobbyExistent = false; 
            try
            {
                IsLobbyExistent = _lobbyValidatorClient.IsLobbyExistent(randomCode);
            }
            catch (CommunicationException)
            {
                randomCode = 0;
            }
            catch (TimeoutException)
            {
                randomCode = 0;
            }

            if (IsLobbyExistent)
            {
                GenerateNewLobbyCode();
            }

            return randomCode;
        }

        public void JoinGame(int lobbyCode, string username)
        {
            try
            {
                _lobbyManagerClient.NotifyJoinLobby(lobbyCode, username);
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

        private void LoadVisualComponents()
        {
            LblLobbyCode.Content = Properties.Resources.LblPartyCode + ": " + SessionDetails.LobbyCode;

            if (!SessionDetails.IsHost)
            {
                BtnStartGame.Visibility = Visibility.Collapsed;
            }

            _popUpUserDetails = new PopUpUserDetails
            {
                Width = 350,
                Height = 240,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = Visibility.Collapsed
            };
            MasterGrid.Children.Add(_popUpUserDetails);
        }

        private void OnEnterSendMessage(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && TBoxMessage.Text.Trim().Length > 0)
            {
                string message = SessionDetails.Username + ": " + TBoxMessage.Text;

                SendMessage(SessionDetails.LobbyCode, message);

                TBoxMessage.Text = "";
            }
            else if (TBoxMessage.Text.Length > 64)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgMessageMaxCharacters, MessageBoxImage.Warning);
            }
        }

        public void SendMessage(int lobbyCode, string message)
        {
            try
            {
                _lobbyManagerClient.NotifySendMessage(lobbyCode, message);
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

        private void BtnExitLobbyEvent(object sender, RoutedEventArgs e)
        {
            ExitLobby();
        }

        public void ExitLobby()
        {
            if (SessionDetails.IsHost)
            {
                SessionDetails.IsHost = false;
                try
                {
                    CloseLobby(SessionDetails.LobbyCode);
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
            else
            {
                try
                {
                    _lobbyManagerClient.NotifyLeaveLobby(SessionDetails.LobbyCode, SessionDetails.Username);
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
            
            SessionDetails.LobbyCode = 0;
            if (SessionDetails.IsGuest)
            {
                Application.Current.MainWindow.Content = new Launcher();
            }
            else
            {
                Application.Current.MainWindow.Content = new MainMenu();
            }

            MusicManager.PlayPlayerLeftSound();
        }

        public void CloseLobby(int lobbyCode)
        {
            try
            {
                _lobbyManagerClient.NotifyCloseLobby(lobbyCode, SessionDetails.Username, Properties.Resources.DlgHostHasClosedTheParty);
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

        private void UpdatePlayerList(Dictionary<string, object> playersInLobby)
        {
            PanelPlayers.Children.Clear();
            foreach (var player in playersInLobby)
            {
                CreatePlayerPanel(player.Key);
            }
        }

        private void CreatePlayerPanel(string playerUsername)
        {
            SolidColorBrush backgroundColor;
            if (playerUsername == SessionDetails.Username)
            {
                backgroundColor = new SolidColorBrush(Colors.Gold);
            }
            else
            {
                backgroundColor = new SolidColorBrush(Colors.DimGray);
            }

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = backgroundColor,
                Margin = new Thickness(15, 20, 15, 20),
                Width = 200,
                Height = 40,
            };
            PanelPlayers.Children.Add(stackPanel);

            Label usernameName = new Label
            {
                Foreground = new SolidColorBrush(Colors.Black),
                Content = playerUsername,
                Margin = new Thickness(10, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanel.Children.Add(usernameName);

            if (playerUsername != SessionDetails.Username)
            {
                Button btnKick = new Button
                {
                    Content = Properties.Resources.BtnKick,
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    DataContext = playerUsername
                };
                btnKick.Click += KickPlayerEvent;

                Button btnViewProfile = new Button
                {
                    Content = Properties.Resources.BtnProfile,
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    DataContext = playerUsername
                };
                btnViewProfile.Click += ViewProfileEvent;

                if (SessionDetails.IsHost)
                {
                    stackPanel.Children.Add(btnKick);
                }

                if (!playerUsername.Contains("guest"))
                {
                    stackPanel.Children.Add(btnViewProfile);
                }
            }
        }

        private void ViewProfileEvent(object sender, RoutedEventArgs e)
        {
            string username = ((FrameworkElement)sender).DataContext as string;
            _popUpUserDetails.InitializeUserInfo(username);
            _popUpUserDetails.Visibility = Visibility.Visible;
        }

        private void KickPlayerEvent(object sender, RoutedEventArgs e)
        {
            string username = ((FrameworkElement)sender).DataContext as string;
            PopUpKickPlayer popUpKickPlayer = new PopUpKickPlayer
            {
                KickedUsername = username
            };
            popUpKickPlayer.SetClient(_lobbyManagerClient);
            popUpKickPlayer.Show();
        }

        private void StartGameEvent(object sender, RoutedEventArgs e)
        {
            LobbyValidatorClient lobbyValidatorClient = new LobbyValidatorClient();

            if (lobbyValidatorClient.GetPlayersInLobby(SessionDetails.LobbyCode).Length > 1)
            {
                BtnStartGame.Visibility = Visibility.Collapsed;
                LobbyManagerClient lobbyManagerClient = new LobbyManagerClient(new InstanceContext(this));

                try
                {
                    lobbyManagerClient.NotifyStartGame(SessionDetails.LobbyCode);
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
            else
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgNotEnoughPlayers, MessageBoxImage.Exclamation);
            }
        }

        public void MessageReceived(string messageSent)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = messageSent,
                Margin = new Thickness(0, 4, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = new SolidColorBrush(Colors.White),
                Width = 248,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            PanelChat.Children.Add(textBlock);
            ScrollViewerChat.ScrollToEnd();
        }

        public void PlayerJoined(Dictionary<string, object> playersInLobby)
        {
            MusicManager.PlayPlayerJoinedSound();
            UpdatePlayerList(playersInLobby);
        }

        public void PlayerLeft(Dictionary<string, object> playersInLobby)
        {
            MusicManager.PlayPlayerLeftSound();
            UpdatePlayerList(playersInLobby);
        }

        public void LobbyCreated(Dictionary<string, object> playersInLobby)
        {
            UpdatePlayerList(playersInLobby);
        }

        public async void GameStarted()
        {
            CardTable cardTable = new CardTable();
            InstanceContext tableContext = new InstanceContext(cardTable);
            MatchManagerClient tableClient = new MatchManagerClient(tableContext);
            LblLoading.Visibility = Visibility.Visible;

            try
            {
                tableClient.Subscribe(SessionDetails.LobbyCode, SessionDetails.Username);
                await Task.Delay(3000);
                cardTable.LoadPlayers();
                cardTable.UpdateTableCards();
            }
            catch (CommunicationException)
            {
                SessionDetails.AbortOperation();
            }
            catch (TimeoutException)
            {
                SessionDetails.AbortOperation();
            }

            Application.Current.MainWindow.Content = cardTable;
        }

        public void PlayerKicked(string reason)
        {
            if (SessionDetails.IsGuest)
            {
                Application.Current.MainWindow.Content = new Launcher();
            }
            else
            {
                Application.Current.MainWindow.Content = new MainMenu();
            }
            string message = Properties.Resources.DlgKickedPlayer + Properties.Resources.DlgKickingReason + reason;
            MainWindow.ShowMessageBox(message, MessageBoxImage.Exclamation);
        }
    }
}