using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationService
{
    [ServiceContract(CallbackContract = typeof(IPartyManagerCallback))]
    internal interface IPartyManager
    {
        [OperationContract(IsOneWay = true)]
        void JoinParty(string email);

        [OperationContract(IsOneWay = true)]
        void SendMessage(string message);

        [OperationContract(IsOneWay = true)]
        void LeaveParty(string email);
    }

    [ServiceContract]
    internal interface IPartyManagerCallback
    {
        [OperationContract]
        void PlayerJoined(Dictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void MessageReceived(string messageSent);

        [OperationContract]
        void PlayerLeft(Dictionary<string, IPartyManagerCallback> playersInLobby);
    }
}
