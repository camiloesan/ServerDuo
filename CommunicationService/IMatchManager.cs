using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;

namespace CommunicationService
{
    /// <summary>
    /// This contract manages match operations such as ending turns and matches and calling callbacks
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
        /// Sends the amount of remaining cards to calculate which one has the least cards and therefore wins the match
        /// </summary>
        /// <param name="partyCode">Code of the matchg</param>
        /// <param name="username"></param>
        /// <param name="cardCount">Amount of cards left, the winner usually has zero unless he wins by default if all players left</param>
        [OperationContract]
        void SetGameScore(int partyCode, string username, int cardCount);

        /// <summary>
        /// Get the results when a match is over
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        /// <returns></returns>
        [OperationContract]
        ConcurrentDictionary<string, int> GetMatchResults(int partyCode);
    }
}
