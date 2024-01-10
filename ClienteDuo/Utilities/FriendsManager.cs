using ClienteDuo.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteDuo.Utilities
{
    public static class FriendsManager
    {
        public static int SendFriendRequest(string usernameSender, string usernameReceiver)
        {
            FriendsManagerClient friendsManagerClient = new FriendsManagerClient();
            return friendsManagerClient.SendFriendRequest(usernameSender, usernameReceiver);
        }

        public static bool IsFriendRequestAlreadySent(string usernameSender, string usernameReceiver)
        {
            FriendsManagerClient friendsManagerClient = new FriendsManagerClient();
            return friendsManagerClient.IsFriendRequestAlreadyExistent(usernameSender, usernameReceiver);
        }

        public static bool AcceptFriendRequest(FriendRequestDTO friendRequest)
        {
            FriendsManagerClient friendsManagerClient = new FriendsManagerClient();
            return friendsManagerClient.AcceptFriendRequest(friendRequest);
        }

        public static bool DeclineFriendRequest(FriendRequestDTO friendRequest)
        {
            FriendsManagerClient friendsManagerClient = new FriendsManagerClient();
            return friendsManagerClient.RejectFriendRequest(friendRequest.FriendRequestID);
        }

        public static IEnumerable<FriendRequestDTO> GetFriendRequestsByUserId(int userId)
        {
            FriendsManagerClient friendsManagerClient = new FriendsManagerClient();
            return friendsManagerClient.GetFriendRequestsList(userId);
        }

        public static IEnumerable<FriendshipDTO> GetFriendsListByUserId(int userId)
        {
            FriendsManagerClient friendsManagerClient = new FriendsManagerClient();
            return friendsManagerClient.GetFriendsList(userId);
        }

        public static IEnumerable<FriendshipDTO> GetOnlineFriends(int userId)
        {
            FriendsManagerClient friendsManagerClient = new FriendsManagerClient();
            return friendsManagerClient.GetOnlineFriends(userId);
        }

        public static bool DeleteFriendshipById(int friendshipId)
        {
            FriendsManagerClient friendsManagerClient = new FriendsManagerClient();
            return friendsManagerClient.DeleteFriendshipById(friendshipId);
        }

        public static bool IsAlreadyFriend(string usernameSender, string usernameReceiver)
        {
            FriendsManagerClient friendsManagerClient = new FriendsManagerClient();
            return friendsManagerClient.IsAlreadyFriend(usernameSender, usernameReceiver);
        }
    }
}
