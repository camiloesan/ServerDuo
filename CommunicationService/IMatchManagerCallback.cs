using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationService
{
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
}
