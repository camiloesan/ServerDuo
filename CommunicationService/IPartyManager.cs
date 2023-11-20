using System.Collections.Concurrent;
using System.ServiceModel;

namespace CommunicationService
{
    [ServiceContract]
    internal interface IPartyValidator
    {
        [OperationContract]
        bool IsPartyExistent(int partyCode);

        [OperationContract]
        bool IsSpaceAvailable(int partyCode);
    }

    [ServiceContract(CallbackContract = typeof(IPartyManagerCallback))]
    internal interface IPartyManager
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
    internal interface IPartyManagerCallback
    {
        [OperationContract]
        void PartyCreated();

        [OperationContract]
        void PlayerJoined();

        [OperationContract]
        void MessageReceived(string messageSent);

        [OperationContract]
        void PlayerLeft();

        [OperationContract]
        void PlayerKicked();

        [OperationContract]
        void GameStarted();
    }
}
