using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;

namespace CommunicationService
{
    [ServiceContract]
    public interface IPartyValidator
    {
        /// <summary>
        /// Checks if a party with the partyCode provided is active
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns>Success status</returns>
        [OperationContract]
        bool IsPartyExistent(int partyCode);

        /// <summary>
        /// Checks if there is a space available to join
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns>True if space available, false if not</returns>
        [OperationContract]
        bool IsSpaceAvailable(int partyCode);

        /// <summary>
        /// If an user with the provided username is inside the lobby with provided partyCode
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        /// <returns>Status</returns>
        [OperationContract]
        bool IsUsernameInParty(int partyCode, string username);

        /// <summary>
        /// Counts every player on party hashmap
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns>Total players in party</returns>
        [OperationContract]
        List<string> GetPlayersInParty(int partyCode);
    }

    [ServiceContract(CallbackContract = typeof(IPartyManagerCallback))]
    public interface IPartyManager
    {
        /// <summary>
        /// Notifies host that party has been created
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="hostUsername"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyCreateParty(int partyCode, string hostUsername);

        /// <summary>
        /// Notifies new player joining to every player
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyJoinParty(int partyCode, string username);

        /// <summary>
        /// Notifies every player in party a message (to chat)
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="message"></param>
        [OperationContract(IsOneWay = true)]
        void NotifySendMessage(int partyCode, string message);

        /// <summary>
        /// Notifies every player in party if someone leaves lobby
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyLeaveParty(int partyCode, string username);

        /// <summary>
        /// Notifies every player in party if host has left lobby
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="hostName"></param>
        /// <param name="reason"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyCloseParty(int partyCode, string hostName, string reason);

        /// <summary>
        /// Notifies game start to every player in party
        /// </summary>
        /// <param name="partyCode"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyStartGame(int partyCode);

        /// <summary>
        /// Notifies every player in party if host has kicked a player (including kicked player)
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        /// <param name="reason"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyKickPlayer(int partyCode, string username, string reason);
    }

    [ServiceContract]
    public interface IPartyManagerCallback
    {
        /// <summary>
        /// Receives updated player dictionary
        /// </summary>
        /// <param name="playersInLobby"></param>
        [OperationContract]
        void PartyCreated(ConcurrentDictionary<string, IPartyManagerCallback> playersInLobby);

        /// <summary>
        /// Receives updated player dictionary
        /// </summary>
        /// <param name="playersInLobby"></param>
        [OperationContract]
        void PlayerJoined(ConcurrentDictionary<string, IPartyManagerCallback> playersInLobby);

        /// <summary>
        /// Receives message sent by someone
        /// </summary>
        /// <param name="messageSent"></param>
        [OperationContract]
        void MessageReceived(string messageSent);

        /// <summary>
        /// Receives updated player dictionary
        /// </summary>
        /// <param name="playersInLobby"></param>
        [OperationContract]
        void PlayerLeft(ConcurrentDictionary<string, IPartyManagerCallback> playersInLobby);


        /// <summary>
        /// Receives notification if player himself has been kicked
        /// </summary>
        /// <param name="reason"></param>
        [OperationContract]
        void PlayerKicked(string reason);

        /// <summary>
        /// Receives game started notification
        /// </summary>
        [OperationContract]
        void GameStarted();
    }
}
