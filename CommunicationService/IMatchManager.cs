using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;

namespace CommunicationService
{
    [ServiceContract(CallbackContract = typeof(IMatchManagerCallback))]
    public interface IMatchManager
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe(int partyCode, string username);

        [OperationContract(IsOneWay = true)]
        void SetGameScore(int partyCode, string username, int cardCount);

        [OperationContract(IsOneWay = true)]
        void ExitMatch(int partyCode, string username);

        [OperationContract]
        void KickPlayerFromGame(int partyCode, string username, string reason);

        [OperationContract]
        void EndGame(int partyCode);

        [OperationContract]
        void EndTurn(int partyCode);

        [OperationContract]
        string GetCurrentTurn(int partyCode);

        [OperationContract]
        List<string> GetPlayerList(int partyCode);

        [OperationContract]
        ConcurrentDictionary<string, int> GetMatchResults(int partyCode);
    }

    [ServiceContract]
    public interface IMatchManagerCallback
    {
        [OperationContract]
        void UpdateTableCards();

        [OperationContract]
        void PlayerLeftGame(string username, string reason);

        [OperationContract]
        void TurnFinished(string currentTurn);

        [OperationContract]
        void GameOver();
    }

    [ServiceContract]
    public interface ICardManager
    {
        [OperationContract]
        Card DrawCard();

        [OperationContract]
        Card[] GetCards(int partyCode);

        [OperationContract]
        void DealCards(int partyCode);

        [OperationContract]
        void PlayCard(int partyCode, int position, Card card);
    }

    [DataContract]
    public class Card
    {
        public Card()
        {
            Number = "2";
            Color = "#000000";
        }

        public Card(string number, string color)
        {
            Number = number;
            Color = color;
        }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Color { get; set; }

        [OperationContract]
        public override bool Equals(object obj)
        {
            bool result = false;
            var card = obj as Card;

            if (card != null)
            {
                result = card.Number.Equals(Number) && card.Color.Equals(Color);
            }

            return result;
        }
    }

    [DataContract]
    public class CardColors
    {
        [DataMember]
        public static readonly string BLUE = "#0000FF";

        [DataMember]
        public static readonly string YELLOW = "#FFFF00";

        [DataMember]
        public static readonly string GREEN = "#008000";

        [DataMember]
        public static readonly string RED = "#FF0000";

        [DataMember]
        public static readonly string BLACK = "#000000";
    }
}
