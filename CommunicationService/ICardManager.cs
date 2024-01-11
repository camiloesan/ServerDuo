using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;

namespace CommunicationService
{
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
}
