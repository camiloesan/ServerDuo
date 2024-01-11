using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationService
{
    /// <summary>
    /// Contract to manage players during match
    /// </summary>
    [ServiceContract]
    public interface IMatchPlayerManager
    {
        /// <summary>
        /// Get list of players in the match
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        /// <returns></returns>
        [OperationContract]
        List<string> GetPlayerList(int partyCode);

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
        /// Get the current turn in the match
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        /// <returns></returns>
        [OperationContract]
        string GetCurrentTurn(int partyCode);
    }
}
