using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CommunicationService
{
    /// <summary>
    /// Handles user actions
    /// </summary>
    [ServiceContract]
    public interface IUsersManager
    {
        /// <summary>
        /// Updates profile picture id to a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pictureId"></param>
        /// <returns>Rows affected</returns>
        [OperationContract]
        int UpdateProfilePictureByUserId(int userId, int pictureId);

        /// <summary>
        /// Gets all information about a player (except password)
        /// </summary>
        /// <param name="username"></param>
        /// <returns>UserDTO user details</returns>
        [OperationContract]
        UserDTO GetUserInfoByUsername(string username);

        /// <summary>
        /// Adds an user to database
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Rows affected</returns>
        [OperationContract]
        int AddUserToDatabase(UserDTO user);

        /// <summary>
        /// Deletes user from database
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Success status</returns>
        [OperationContract]
        bool DeleteUserFromDatabaseByUsername(String username);

        /// <summary>
        /// Checks if login credentials are valid
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>UserDTO if valid, null if not</returns>
        [OperationContract]
        UserDTO IsLoginValid(string username, string password);

        /// <summary>
        /// Checks if provided username exists in database
        /// </summary>
        /// <param name="username"></param>
        /// <returns>True if exists, false if not</returns>
        [OperationContract]
        bool IsUsernameTaken(string username);

        /// <summary>
        /// Checks if provided email exists in database
        /// </summary>
        /// <param name="email"></param>
        /// <returns>True if exists, false if not</returns>
        [OperationContract]
        bool IsEmailTaken(string email);

        /// <summary>
        /// Adds a friend request to database
        /// </summary>
        /// <param name="usernameSender"></param>
        /// <param name="usernameReceiver"></param>
        /// <returns>Rows affected</returns>
        [OperationContract]
        int SendFriendRequest(string usernameSender, string usernameReceiver);

        /// <summary>
        /// Checks if sender has already sent a request to receiver
        /// </summary>
        /// <param name="usernameSender"></param>
        /// <param name="usernameReceiver"></param>
        /// <returns>True if a request if found, false if not</returns>
        [OperationContract]
        bool IsFriendRequestAlreadyExistent(string usernameSender, string usernameReceiver);

        /// <summary>
        /// Adds request user id's to friendship table
        /// </summary>
        /// <param name="friendRequest"></param>
        /// <returns>True if succeded, false if not</returns>
        [OperationContract]
        bool AcceptFriendRequest(FriendRequestDTO friendRequest);

        /// <summary>
        /// Deletes from friend request table the request
        /// </summary>
        /// <param name="friendRequestId"></param>
        /// <returns>True if succeded, false if not</returns>
        [OperationContract]
        bool RejectFriendRequest(int friendRequestId);

        /// <summary>
        /// Gets user's friend requests
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List FriendRequestDTO</returns>
        [OperationContract]
        List<FriendRequestDTO> GetFriendRequestsList(int userId);

        /// <summary>
        /// Gets user's friends
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List FriendshipDTO</returns>
        [OperationContract]
        List<FriendshipDTO> GetFriendsList(int userId);

        /// <summary>
        /// Gets user´s online friends
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List FriendshipDTO</returns>
        [OperationContract]
        List<FriendshipDTO> GetOnlineFriends(int userId);

        /// <summary>
        /// Deletes friendship from database
        /// </summary>
        /// <param name="friendshipId"></param>
        /// <returns>True if success, false if not</returns>
        [OperationContract]
        bool DeleteFriendshipById(int friendshipId);

        /// <summary>
        /// Checks if receiver username is sender's friend
        /// </summary>
        /// <param name="senderUsername"></param>
        /// <param name="receiverUsername"></param>
        /// <returns>True if is friend, false if not</returns>
        [OperationContract]
        bool IsAlreadyFriend(string senderUsername, string receiverUsername);

        /// <summary>
        /// Checks if user is logged in (online)
        /// </summary>
        /// <param name="username"></param>
        /// <returns>True if is online, false if not</returns>
        [OperationContract]
        bool IsUserAlreadyLoggedIn(string username);

        /// <summary>
        /// Sends a confirmation code to provided email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="lang"></param>
        /// <returns>confirmation code</returns>
        [OperationContract]
        int SendConfirmationCode(string email, string lang);

        /// <summary>
        /// Modifies password by email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="newPassword"></param>
        /// <returns>Rows affected</returns>
        [OperationContract]
        int ModifyPasswordByEmail(string email, string newPassword);

        /// <summary>
        /// Adds user to blocked user's list
        /// </summary>
        /// <param name="blockerUsername"></param>
        /// <param name="blockedUsername"></param>
        /// <returns>Rows affected: 1 if succeed, 2 if banned definitely</returns>
        [OperationContract]
        int BlockUserByUsername(string blockerUsername, string blockedUsername);

        /// <summary>
        /// Deletes user from user's block list
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns>Rows affected</returns>
        [OperationContract]
        int UnblockUserByBlockId(int blockId);

        /// <summary>
        /// Checks if user is blocked by another user
        /// </summary>
        /// <param name="usernameBlocker"></param>
        /// <param name="usernameBlocked"></param>
        /// <returns>True if is blocked, false if not</returns>
        [OperationContract]
        bool IsUserBlockedByUsername(string usernameBlocker, string usernameBlocked);

        /// <summary>
        /// Gets user's blocked list
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List UserBlockedDTO</returns>
        [OperationContract]
        List<UserBlockedDTO> GetBlockedUsersListByUserId(int userId);

        /// <summary>
        /// Gets top 10 global winners
        /// </summary>
        /// <returns>List UserDTO</returns>
        [OperationContract]
        List<UserDTO> GetTopTenWinners();

        /// <summary>
        /// Checks if user is banned from game
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True if banned, false if not</returns>
        [OperationContract]
        bool IsUserBanned(int userId);
    }

    /// <summary>
    /// Handles user connections
    /// </summary>
    [ServiceContract]
    public interface IUserConnectionHandler
    {
        /// <summary>
        /// Notifies user log in to server
        /// </summary>
        /// <param name="user"></param>
        [OperationContract]
        void NotifyLogIn(UserDTO user);

        /// <summary>
        /// Notifies log out to server and closes party resources if on-party
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isHost"></param>
        [OperationContract]
        void NotifyLogOut(UserDTO user, bool isHost);

        /// <summary>
        /// Notifies if a guest has left a party or game
        /// </summary>
        /// <param name="partyCode"></param>
        /// <param name="username"></param>
        [OperationContract]
        void NotifyGuestLeft(int partyCode, string username);

        /// <summary>
        /// Logs out user from online users
        /// </summary>
        /// <param name="username"></param>
        [OperationContract]
        void LogOut(string username);
    }

    [DataContract]
    public class UserDTO
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public int TotalWins { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public int PictureID { get; set; }

        [DataMember] 
        public string Password { get; set; }

        [DataMember]
        public int PartyCode { get; set; }
    }

    [DataContract]
    public class FriendshipDTO
    {
        [DataMember]
        public int FriendshipID { get; set; }

        [DataMember]
        public int Friend1ID { get; set; }

        [DataMember]
        public string Friend1Username { get; set; }

        [DataMember]
        public int Friend2ID { get; set; }

        [DataMember]
        public string Friend2Username { get; set; }
    }

    [DataContract]
    public class FriendRequestDTO
    {
        [DataMember]
        public int FriendRequestID { get; set; }

        [DataMember]
        public int SenderID { get; set; }

        [DataMember]
        public string SenderUsername { get; set; }

        [DataMember]
        public int ReceiverID { get; set; }

        [DataMember]
        public string ReceiverUsername { get; set; }

        [DataMember]
        public int Status { get; set; }
    }

    [DataContract]
    public class UserBlockedDTO
    {
        [DataMember]
        public int BlockID { get; set; }


        [DataMember]
        public int BlockedID { get; set; }

        [DataMember]
        public string BlockedUsername { get; set; }

        [DataMember]
        public string Reason { get; set; }
    }
}
