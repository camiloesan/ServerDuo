using ClienteDuo.Utilities;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClienteDuo.DataService;
using System.Collections;
using System.ServiceModel;

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
            string partyCodeString = TBoxPartyCode.Text.Trim();

            bool isPartyValid = false;
            try
            {
                isPartyValid = ArePartyConditionsValid(partyCodeString);
            }
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
            }

            if (isPartyValid)
            {
                if (SessionDetails.IsGuest)
                {
                    GenerateGuestName(partyCodeString);
                }
                
                SessionDetails.PartyCode = int.Parse(partyCodeString);
                Lobby lobby = new Lobby(SessionDetails.Username, SessionDetails.PartyCode);
                Application.Current.MainWindow.Content = lobby;
            }
        }

        private bool ArePartyConditionsValid(string partyCode)
        {
            if (!IsInputInteger(partyCode))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgInvalidPartyCodeFormat, MessageBoxImage.Information);
            }
            else if (!IsPartyCodeExistent(int.Parse(partyCode)))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgPartyNotFound, MessageBoxImage.Information);
            }
            else if (!IsSpaceAvailable(int.Parse(partyCode)))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgFullParty, MessageBoxImage.Information);
            }
            else if (SessionDetails.IsGuest == false && IsUserInPartyBlocked(int.Parse(partyCode)))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgUserBlockedInParty, MessageBoxImage.Information);
            }
            else
            {
                return true;
            }

            return false;
        }

        private void GenerateGuestName(string partyCodeString)
        {
            PartyValidatorClient partyValidatorClient = new PartyValidatorClient();
            Random randomId = new Random();
            int id = randomId.Next(0,1000);
            string randomUsername = "guest" + id;
            if (partyValidatorClient.IsUsernameInParty(int.Parse(partyCodeString), randomUsername))
            {
                GenerateGuestName(randomUsername);
            }
            SessionDetails.Username = randomUsername;
        }

        public bool IsInputInteger(string code)
        {
            return int.TryParse(code, out _);
        }

        public bool IsPartyCodeExistent(int partyCode)
        {
            PartyValidatorClient partyValidatorClient = new PartyValidatorClient();
            return partyValidatorClient.IsPartyExistent(partyCode);
        }

        public bool IsSpaceAvailable(int partyCode)
        {
            PartyValidatorClient partyValidatorClient = new PartyValidatorClient();
            return partyValidatorClient.IsSpaceAvailable(partyCode);
        }

        public bool IsUserInPartyBlocked(int partyCode)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            PartyValidatorClient partyValidatorClient = new PartyValidatorClient();
            var playersInPartyList = partyValidatorClient.GetPlayersInParty(partyCode);
            foreach (var player in playersInPartyList)
            {
                if (usersManagerClient.IsUserBlockedByUsername(SessionDetails.Username ,player)
                    || usersManagerClient.IsUserBlockedByUsername(player, SessionDetails.Username))
                {
                    return true;
                }
            }
            return false;
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
