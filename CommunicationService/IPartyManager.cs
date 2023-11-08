using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

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
        [OperationContract (IsOneWay = true)]
        void NewParty(int partyCode, string username);

        [OperationContract (IsOneWay = true)]
        void JoinParty(int partyCode, string username);

        [OperationContract (IsOneWay = true)]
        void SendMessage(int partyCode ,string message);

        [OperationContract (IsOneWay = true)]
        void LeaveParty(int partyCode, string username);

        [OperationContract(IsOneWay = true)]
        void CloseParty(int partyCode);

        [OperationContract (IsOneWay = true)]
        void StartGame(int partyCode);

        [OperationContract (IsOneWay = true)]
        void IsPlayerActive();

        [OperationContract (IsOneWay = true)]
        void KickPlayer(int partyCode, string username);
    }

    [ServiceContract]
    internal interface IPartyManagerCallback
    {
        [OperationContract]
        void NotifyPartyCreated(ConcurrentDictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void NotifyPlayerJoined(ConcurrentDictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void NotifyMessageReceived(string messageSent);

        [OperationContract]
        void NotifyPlayerLeft(ConcurrentDictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void NotifyPlayerKicked();

        [OperationContract]
        void NotifyGameStarted();
    }
}
