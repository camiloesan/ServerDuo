﻿using System;
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
        [OperationContract(IsOneWay = true)]
        void NewParty(int partyCode, string username);

        [OperationContract(IsOneWay = true)]
        void JoinParty(int partyCode, string username);

        [OperationContract(IsOneWay = true)]
        void SendMessage(int partyCode ,string message);

        [OperationContract(IsOneWay = true)]
        void LeaveParty(int partyCode, string username);

        [OperationContract(IsOneWay = true)]
        void StartGame(int partyCode);

        [OperationContract(IsOneWay = true)]
        void IsPlayerActive();
    }

    [ServiceContract]
    internal interface IPartyManagerCallback
    {
        [OperationContract]
        void PartyCreated(Dictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void PlayerJoined(Dictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void MessageReceived(string messageSent);

        [OperationContract]
        void PlayerLeft(Dictionary<string, IPartyManagerCallback> playersInLobby);

        [OperationContract]
        void GameStarted();
    }
}
