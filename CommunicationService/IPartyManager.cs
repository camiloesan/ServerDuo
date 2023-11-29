using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;

namespace CommunicationService
{
    [ServiceContract]
    public interface IPartyValidator
    {
        [OperationContract]
        bool IsPartyExistent(int partyCode);

        [OperationContract]
        bool IsSpaceAvailable(int partyCode);

        [OperationContract]
        bool IsUsernameInParty(int partyCode, string username);

        [OperationContract]
        List<string> GetPlayersInParty(int partyCode);
    }

    [ServiceContract(CallbackContract = typeof(IPartyManagerCallback))]
    public interface IPartyManager
    {
        [OperationContract(IsOneWay = true)]
        void NotifyCreateParty(int partyCode, string hostUsername);

        [OperationContract(IsOneWay = true)]
        void NotifyJoinParty(int partyCode, string username);

        [OperationContract(IsOneWay = true)]
        void NotifySendMessage(int partyCode, string message);

        [OperationContract(IsOneWay = true)]
        void NotifyLeaveParty(int partyCode, string username);

        [OperationContract(IsOneWay = true)]
        void NotifyCloseParty(int partyCode);

        [OperationContract(IsOneWay = true)]
        void NotifyStartGame(int partyCode);

        [OperationContract(IsOneWay = true)]
        void NotifyKickPlayer(int partyCode, string username);
    }

    [ServiceContract]
    public interface IPartyManagerCallback
    {
        [OperationContract]
        void PartyCreated(ConcurrentDictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void PlayerJoined(ConcurrentDictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void MessageReceived(string messageSent);

        [OperationContract]
        void PlayerLeft(ConcurrentDictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void PlayerKicked();

        [OperationContract]
        void GameStarted();
    }
}
