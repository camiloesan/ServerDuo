using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;

namespace CommunicationService
{
    /// <summary>
    /// This contract manages match operations such as joining and kicking players out of match, and non-card-related operations such as ending turns and matches
    /// </summary>
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
        /// Sends the amount of remaining cards to calculate which one has the least cards and therefore wins the match
        /// </summary>
        /// <param name="partyCode">Code of the matchg</param>
        /// <param name="username"></param>
        /// <param name="cardCount">Amount of cards left, the winner usually has zero unless he wins by default if all players left</param>
        [OperationContract]
        void SetGameScore(int partyCode, string username, int cardCount);
        /// <summary>
        /// Deletes the player that calls this method from the match and notifies que exit to the other players
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        /// <param name="username">Name of the player that is leaving</param>
        [OperationContract(IsOneWay = true)]
        void ExitMatch(int partyCode, string username);

        /// <summary>
        /// Expells a player from a match and if there is only the host left, he wins
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        /// <param name="username">Name of the player is kicked</param>
        /// <param name="reason"></param>
        [OperationContract]
        void KickPlayerFromGame(int partyCode, string username, string reason);

        /// <summary>
        /// End a match, this is normally called when a player reaches zero cards or when there is less than 2 players in a match
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        [OperationContract(IsOneWay = true)]
        void EndGame(int partyCode);

        /// <summary>
        /// Ends a player turn and notifies the others
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        [OperationContract(IsOneWay = true)]
        void EndTurn(int partyCode);

        /// <summary>
        /// Get the current turn in the match
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        /// <returns></returns>
        [OperationContract]
        string GetCurrentTurn(int partyCode);

        /// <summary>
        /// Get list of players in the match
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        /// <returns></returns>
        [OperationContract]
        List<string> GetPlayerList(int partyCode);

        /// <summary>
        /// Get the results when a match is over
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        /// <returns></returns>
        [OperationContract]
        ConcurrentDictionary<string, int> GetMatchResults(int partyCode);
    }

    /// <summary>
    /// Callback interface for Match Manager
    /// </summary>
    [ServiceContract]
    public interface IMatchManagerCallback
    {
        /// <summary>
        /// Called automatically to retrieve current cards in the table after a turn is over
        /// </summary>
        [OperationContract]
        void UpdateTableCards();

        /// <summary>
        /// Called when a player is expelled from a game to delete in each client list (It also notifies the kicked player)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="reason">Reason the player was kicked for (i.e. Cheating)</param>
        [OperationContract]
        void PlayerLeftGame(string username, string reason);

        /// <summary>
        /// Called when a player finishes their turn
        /// </summary>
        /// <param name="currentTurn">Current player turn</param>
        [OperationContract]
        void TurnFinished(string currentTurn);

        /// <summary>
        /// Called when a player plays all their cards or all other players left
        /// </summary>
        [OperationContract]
        void GameOver();
    }

    [ServiceContract]
    public interface ICardManager
    {
        /// <summary>
        /// Generate a card with random number and color
        /// </summary>
        /// <returns>A card with a number in range [0-9] or wildcard '#' with a color of class CardColors (If the card has 2 value the color is black)</returns>
        [OperationContract]
        Card DrawCard();

        /// <summary>
        /// Get cards from a match
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns></returns>
        [OperationContract]
        Card[] GetCards(int partyCode);

        /// <summary>
        /// Shuffle table cards
        /// </summary>
        /// <param name="partyCode"></param>
        [OperationContract]
        void DealCards(int partyCode);

        /// <summary>
        /// Play a card in an specific position
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="position"></param>
        /// <param name="card"></param>
        [OperationContract]
        void PlayCard(int partyCode, int position, Card card);
    }

    [ServiceContract]
    public class Card
    {
        /// <summary>
        /// Constructor with default values for a DOS card with color black
        /// </summary>
        public Card()
        {
            Number = "2";
            Color = "#000000";
        }

        /// <summary>
        /// Auxiliary constructor with set data
        /// </summary>
        /// <param name="number">Value for the Number property</param>
        /// <param name="color">Value for the Color property</param>
        public Card(string number, string color)
        {
            Number = number;
            Color = color;
        }

        /// <summary>
        /// Number of the card with ranges [0-9] and a Wildcard '#'
        /// </summary>
        [DataMember]
        public string Number { get; set; }

        /// <summary>
        /// Color of the card, must be a CardColors string (Black must only be used when the card is a Number 2)
        /// </summary>
        [DataMember]
        public string Color { get; set; }

        /// <summary>
        /// Compares color and number of a Card to check if they are equal
        /// </summary>
        /// <param name="obj"> Card object to be compared</param>
        /// <returns>Result of number and color comparison</returns>
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

    /// <summary>
    /// Stores color hexes to avoid code duplication
    /// </summary>
    [DataContract]
    public class CardColors
    {
        /// <summary>
        /// Hex for color blue used in cards
        /// </summary>
        [DataMember]
        public static readonly string BLUE = "#0000FF";

        /// <summary>
        /// Hex for color yellow used in cards
        /// </summary>
        [DataMember]
        public static readonly string YELLOW = "#FFFF00";

        /// <summary>
        /// Hex for color green used in cards
        /// </summary>
        [DataMember]
        public static readonly string GREEN = "#008000";

        /// <summary>
        /// Hex for color red used in cards
        /// </summary>
        [DataMember]
        public static readonly string RED = "#FF0000";

        /// <summary>
        /// Hex for color black used in cards with the number 2 only
        /// </summary>
        [DataMember]
        public static readonly string BLACK = "#000000";
    }
}
