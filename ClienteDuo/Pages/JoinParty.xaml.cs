using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClienteDuo.Pages
{
    public partial class JoinParty : Page
    {
        public JoinParty()
        {
            InitializeComponent();
        }

        private void EnterKeyEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                JoinLobby();
            }
        }

        private void BtnJoinEvent(object sender, RoutedEventArgs e)
        {
            JoinLobby();
        }

        public void JoinLobby()
        {
            string lobbyCodeString = TBoxPartyCode.Text.Trim();

            bool isLobbyValid = false;
            try
            {
                isLobbyValid = AreLobbyConditionsValid(lobbyCodeString);
            }
            catch (CommunicationException)
            {
                SessionDetails.AbortOperation();
            }
            catch (TimeoutException)
            {
                SessionDetails.AbortOperation();
            }

            if (isLobbyValid)
            {
                if (SessionDetails.IsGuest)
                {
                    GenerateGuestName(lobbyCodeString);
                }

                SessionDetails.LobbyCode = int.Parse(lobbyCodeString);
                Lobby lobby = new Lobby(SessionDetails.Username, SessionDetails.LobbyCode);
                Application.Current.MainWindow.Content = lobby;
            }
        }

        private bool AreLobbyConditionsValid(string lobbyCode)
        {
            bool result = false;
            if (!IsInputInteger(lobbyCode))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgInvalidPartyCodeFormat, MessageBoxImage.Information);
            }
            else if (!IsLobbyCodeExistent(int.Parse(lobbyCode)))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgPartyNotFound, MessageBoxImage.Information);
            }
            else if (!IsSpaceAvailable(int.Parse(lobbyCode)))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgFullParty, MessageBoxImage.Information);
            }
            else if (SessionDetails.IsGuest && IsUserBlockedByPlayerInLobby(SessionDetails.Username, int.Parse(lobbyCode)))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgUserBlockedInParty, MessageBoxImage.Information);
            }
            else
            {
                result = true;
            }

            return result;
        }

        private void GenerateGuestName(string lobbyCodeString)
        {
            LobbyValidatorClient lobbyValidatorClient = new LobbyValidatorClient();
            Random randomId = new Random();
            int id = randomId.Next(0, 1000);
            string randomUsername = "guest" + id;
            if (lobbyValidatorClient.IsUsernameInLobby(int.Parse(lobbyCodeString), randomUsername))
            {
                GenerateGuestName(randomUsername);
            }
            SessionDetails.Username = randomUsername;
        }

        public bool IsInputInteger(string code)
        {
            return int.TryParse(code, out _);
        }

        public bool IsLobbyCodeExistent(int lobbyCode)
        {
            LobbyValidatorClient lobbyValidatorClient = new LobbyValidatorClient();
            return lobbyValidatorClient.IsLobbyExistent(lobbyCode);
        }

        public bool IsSpaceAvailable(int lobbyCode)
        {
            LobbyValidatorClient lobbyValidatorClient = new LobbyValidatorClient();
            return lobbyValidatorClient.IsSpaceAvailable(lobbyCode);
        }

        public bool IsUserBlockedByPlayerInLobby(string usernameBlocker, int lobbyCode)
        {
            LobbyValidatorClient lobbyValidatorClient = new LobbyValidatorClient();

            var playersInLobbyList = lobbyValidatorClient.GetPlayersInLobby(lobbyCode);

            bool result = false;
            foreach (var player in playersInLobbyList)
            {
                if (BlockManager.IsUserBlocked(usernameBlocker, player)
                    || BlockManager.IsUserBlocked(player, usernameBlocker))
                {
                    result = true;
                }
            }
            return result;
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            if (SessionDetails.IsGuest)
            {
                Launcher launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
            }
            else
            {
                MainMenu mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }
        }
    }
}
