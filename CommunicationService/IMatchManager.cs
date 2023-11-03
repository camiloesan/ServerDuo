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
        void EndTurn(int gameId);

        [OperationContract(IsOneWay = true)]
        void EndRound(int gameId);

        [OperationContract]
        void EndGame(int gameId);
    }

    [ServiceContract]
    internal interface IMatchManagerCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateTableCards();

        [OperationContract(IsOneWay = true)]
        void NotifyEndRound(int gameId);

        [OperationContract(IsOneWay = true)]
        void NotifyEndGame(int gameId);
    }

    [ServiceContract]
    internal interface ICardManager
    {
        [OperationContract]
        Card DrawCard();

        [OperationContract]
        Card[] GetCards(int gameId);

        [OperationContract(IsOneWay = true)]
        void DealCards(int gameId);

        [OperationContract(IsOneWay = true)]
        void PlayCard(int gameId, int position);
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
