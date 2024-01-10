using ClienteDuo.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClienteDuo.TestClasses
{
    public class TestPartyManager : ILobbyManagerCallback
    {
        private readonly LobbyManagerClient _LobbyManagerClient;
        private int _lobbyCode = 0;
        private string _username;

        public TestPartyManager()
        {
            InstanceContext instanceContext = new InstanceContext(this);
            _LobbyManagerClient = new LobbyManagerClient(instanceContext);
        }

        public void NotifyCreateParty(int partyCode, string hostUseraname)
        {
            _lobbyCode = partyCode;
            _username = hostUseraname;
            _LobbyManagerClient.NotifyCreateLobby(partyCode, hostUseraname);
        }

        public void NotifyCloseParty(int partyCode, string hostUsername, string reason)
        {
            _LobbyManagerClient.NotifyCloseLobby(partyCode, hostUsername , reason);
        }

        public void NotifySendMessage(string message)
        {
            _LobbyManagerClient.NotifySendMessage(_lobbyCode, message);
        }

        public void NotifyJoinParty(int partyCode, string username)
        {
            _LobbyManagerClient.NotifyJoinLobby(partyCode, username);
        }

        public void NotifyKickPlayer(int partyCode, string username, string reason)
        {
            _LobbyManagerClient.NotifyKickPlayer(partyCode, username, reason);
        }

        public void NotifyStartGame(int partyCode)
        {
            _LobbyManagerClient.NotifyStartGame(partyCode);
        }

        //callback section
        public void GameStarted()
        {
            IsGameStarted = true;
        }

        public void MessageReceived(string messageSent)
        {
            ReceivedMessage = messageSent;
        }

        public void LobbyCreated(Dictionary<string, object> playersInLobby)
        {
            PlayersInParty = playersInLobby;
        }

        public void PlayerJoined(Dictionary<string, object> playersInLobby)
        {
            PlayersInParty = playersInLobby;
        }

        public void PlayerKicked(string reason)
        {
            PlayerKickedReason = reason;
        }

        public void PlayerLeft(Dictionary<string, object> playersInLobby)
        {
            PlayersInParty = playersInLobby;
        }

        public static bool IsGameStarted { get; set; }
        public static string PlayerKickedReason { get; set; }
        public static string ReceivedMessage {  get; set; }
        public static Dictionary<string, object> PlayersInParty { get; set; } = null;
    }
}
