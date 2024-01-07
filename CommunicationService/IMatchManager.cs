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
        /// <summary>
        /// Subscribes the user to the service
        /// </summary>
        /// <param name="partyCode">Code of the match the user is subscribing</param>
        /// <param name="username">Name of the user that subscribes</param>
        [OperationContract]
        void Subscribe(int partyCode, string username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        /// <param name="cardCount"></param>
        [OperationContract]
        void SetGameScore(int partyCode, string username, int cardCount);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        [OperationContract(IsOneWay = true)]
        void ExitMatch(int partyCode, string username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        /// <param name="reason"></param>
        [OperationContract]
        void KickPlayerFromGame(int partyCode, string username, string reason);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partyCode"></param>
        [OperationContract(IsOneWay = true)]
        void EndGame(int partyCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partyCode"></param>
        [OperationContract(IsOneWay = true)]
        void EndTurn(int partyCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns></returns>
        [OperationContract]
        string GetCurrentTurn(int partyCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns></returns>
        [OperationContract]
        List<string> GetPlayerList(int partyCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns></returns>
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
