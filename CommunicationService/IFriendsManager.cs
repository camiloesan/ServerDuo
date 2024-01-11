using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationService
{
    /// <summary>
    /// Handles friendships between users
    /// </summary>
    [ServiceContract]
    public interface IFriendsManager
    {
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
    }
}
