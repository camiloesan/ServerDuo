using ClienteDuo.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ClienteDuo.TestClasses
{
    public class TestPartyManager : IPartyManagerCallback
    {
        private readonly PartyManagerClient _partyManagerClient;
        private int _partyCode = 0;
        private string _hostUsername;

        public TestPartyManager()
        {
            InstanceContext instanceContext = new InstanceContext(this);
            _partyManagerClient = new PartyManagerClient(instanceContext);
        }

        public void NotifyCreateParty(int partyCode, string hostUseraname)
        {
            _partyCode = partyCode;
            _hostUsername = hostUseraname;
            _partyManagerClient.NotifyCreateParty(partyCode, hostUseraname);
        }

        public void NotifyCloseParty(int partyCode, string hostUsername, string reason)
        {
            _partyManagerClient.NotifyCloseParty(partyCode, hostUsername , reason);
        }

        public void NotifySendMessage(string message)
        {
            _partyManagerClient.NotifySendMessage(_partyCode, message);
        }

        public void NotifyJoinParty(int partyCode, string username)
        {
            _partyManagerClient.NotifyJoinParty(partyCode, username);
        }

        public void NotifyKickPlayer(int partyCode, string username, string reason)
        {
            _partyManagerClient.NotifyKickPlayer(partyCode, username, reason);
        }

        public void NotifyStartGame(int partyCode)
        {
            _partyManagerClient.NotifyStartGame(partyCode);
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

        public void PartyCreated(Dictionary<string, object> playersInLobby)
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
