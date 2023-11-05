using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CommunicationService
{
    [ServiceContract(CallbackContract = typeof(IMatchManagerCallback))]
    internal interface IMatchManager
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe(int partyCode, string username);

        [OperationContract(IsOneWay = true)]
        void EndGame(int partyCode);

        [OperationContract(IsOneWay = true)]
        void EndRound(int partyCode);

        [OperationContract(IsOneWay = true)]
        void EndTurn(int partyCode);

        [OperationContract]
        string GetCurrentTurn(int partyCode);

        [OperationContract]
        Dictionary<string, int> GetPlayerScores(int partyCode);
    }

    [ServiceContract]
    internal interface IMatchManagerCallback
    {
        [OperationContract]
        void UpdateTableCards();

        [OperationContract]
        void TurnFinished(string currentTurn);

        [OperationContract]
        void RoundOver();

        [OperationContract]
        void GameOver();
    }

    [ServiceContract]
    internal interface ICardManager
    {
        [OperationContract]
        Card DrawCard();

        [OperationContract]
        Card[] GetCards(int partyCode);

        [OperationContract(IsOneWay = true)]
        void DealCards(int partyCode);

        [OperationContract(IsOneWay = true)]
        void PlayCard(int partyCode, int position);
    }

    [DataContract]
    public class Card
    {
        public Card()
        {
            Number = "2";
            Color = "#000000";
        }

        [DataMember]
        public string Number { get; set; }
        
        [DataMember]
        public string Color { get; set; }
    }
}
