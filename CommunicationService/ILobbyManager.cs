using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;

namespace CommunicationService
{
    /// <summary>
    /// Validates conditions required to enter a party (lobby)
    /// </summary>
    [ServiceContract]
    public interface ILobbyValidator
    {
        /// <summary>
        /// Checks if a party with the partyCode provided is active
        /// </summary>
        /// <param name="lobbyCode"></param>
        /// <returns>Success status</returns>
        [OperationContract]
        bool IsLobbyExistent(int lobbyCode);

        /// <summary>
        /// Checks if there is a space available to join
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns>True if space available, false if not</returns>
        [OperationContract]
        bool IsSpaceAvailable(int lobbyCode);

        /// <summary>
        /// If an user with the provided username is inside the lobby with provided partyCode
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        /// <returns>Status</returns>
        [OperationContract]
        bool IsUsernameInLobby(int lobbyCode, string username);

        /// <summary>
        /// Counts every player on party hashmap
        /// </summary>
        /// <param name="partyCode"></param>
        /// <returns>Total players in party</returns>
        [OperationContract]
        List<string> GetPlayersInLobby(int lobbyCode);
    }

    /// <summary>
    /// Manages party interactions between useres
    /// </summary>
    [ServiceContract(CallbackContract = typeof(ILobbyManagerCallback))]
    public interface ILobbyManager
    {
        /// <summary>
        /// Notifies host that party has been created
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="hostUsername"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyCreateLobby(int lobbyCode, string hostUsername);

        /// <summary>
        /// Notifies new player joining to every player
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyJoinLobby(int lobbyCode, string username);

        /// <summary>
        /// Notifies every player in party a message (to chat)
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="message"></param>
        [OperationContract(IsOneWay = true)]
        void NotifySendMessage(int lobbyCode, string message);

        /// <summary>
        /// Notifies every player in party if someone leaves lobby
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyLeaveLobby(int lobbyCode, string username);

        /// <summary>
        /// Notifies every player in party if host has left lobby
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="hostName"></param>
        /// <param name="reason"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyCloseLobby(int lobbyCode, string hostName, string reason);

        /// <summary>
        /// Notifies game start to every player in party
        /// </summary>
        /// <param name="partyCode"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyStartGame(int lobbyCode);

        /// <summary>
        /// Notifies every player in party if host has kicked a player (including kicked player)
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        /// <param name="reason"></param>
        [OperationContract(IsOneWay = true)]
        void NotifyKickPlayer(int lobbyCode, string username, string reason);
    }

    /// <summary>
    /// Manages party manager callbacks
    /// </summary>
    [ServiceContract]
    public interface ILobbyManagerCallback
    {
        /// <summary>
        /// Receives updated player dictionary
        /// </summary>
        /// <param name="playersInLobby"></param>
        [OperationContract]
        void LobbyCreated(ConcurrentDictionary<string, ILobbyManagerCallback> playersInLobby);

        /// <summary>
        /// Receives updated player dictionary
        /// </summary>
        /// <param name="playersInLobby"></param>
        [OperationContract]
        void PlayerJoined(ConcurrentDictionary<string, ILobbyManagerCallback> playersInLobby);

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
        void PlayerLeft(ConcurrentDictionary<string, ILobbyManagerCallback> playersInLobby);

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
