using ClienteDuo.DataService;
using System.Collections.Generic;

namespace ClienteDuo.Utilities
{
    public static class UsersManager
    {
        public static bool IsUsernameTaken(string username)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsUsernameTaken(username);
        }

        public static bool IsEmailTaken(string email)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsEmailTaken(email);
        }

        public static int AddUserToDatabase(string username, string email, string password)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            UserDTO databaseUser = new UserDTO
            {
                UserName = username,
                Email = email,
                Password = Sha256Encryptor.SHA256_hash(password)
            };

            return usersManagerClient.AddUserToDatabase(databaseUser);
        }

        public static int UpdateProfilePicture(int userId, int pictureId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.UpdateProfilePictureByUserId(userId, pictureId);
        }

        public static bool IsUserLoggedIn(string username)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsUserAlreadyLoggedIn(username);
        }

        public static UserDTO AreCredentialsValid(string username, string password)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsLoginValid(username, Sha256Encryptor.SHA256_hash(password));
        }

        public static IEnumerable<UserDTO> GetLeaderboard()
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.GetTopTenWinners();
        }

        public static bool AcceptFriendRequest(FriendRequestDTO friendRequest)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.AcceptFriendRequest(friendRequest);
        }

        public static bool DeclineFriendRequest(FriendRequestDTO friendRequest)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.RejectFriendRequest(friendRequest.FriendRequestID);
        }

        public static IEnumerable<FriendRequestDTO> GetFriendRequestsByUserId(int userId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.GetFriendRequestsList(userId);
        }

        public static int UnblockUserByBlockId(int blockId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.UnblockUserByBlockId(blockId);
        }

        public static IEnumerable<UserBlockedDTO> GetBlockedUsersListByUserId(int userId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.GetBlockedUsersListByUserId(userId);
        }

        public static bool IsUserBlocked(string usernameBlocker, string usernameBlocked)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsUserBlockedByUsername(usernameBlocker, usernameBlocked);
        }

        public static int SendFriendRequest(string usernameSender, string usernameReceiver)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.SendFriendRequest(usernameSender, usernameReceiver);
        }

        public static bool IsAlreadyFriend(string usernameSender, string usernameReceiver)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsAlreadyFriend(usernameSender, usernameReceiver);
        }

        public static bool IsFriendRequestAlreadySent(string usernameSender, string usernameReceiver)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsFriendRequestAlreadyExistent(usernameSender, usernameReceiver);
        }

        public static int BlockUserByUsername(string blockerUsername, string blockedUsername)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.BlockUserByUsername(blockerUsername, blockedUsername);
        }

        public static UserDTO GetUserInfoByUsername(string username)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.GetUserInfoByUsername(username);
        }

        public static IEnumerable<FriendshipDTO> GetOnlineFriends(int userId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.GetOnlineFriends(userId);
        }

        public static IEnumerable<FriendshipDTO> GetFriendsListByUserId(int userId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.GetFriendsList(userId);
        }

        public static bool DeleteFriendshipById(int friendshipId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.DeleteFriendshipById(friendshipId);
        }

        public static int ModifyPasswordByEmail(string email, string newPassword)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.ModifyPasswordByEmail(email, newPassword);
        }

        public static int SendConfirmationCode(string email, string lang)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.SendConfirmationCode(email, lang);
        }

        public static bool DeleteUserFromDatabase(string username)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.DeleteUserFromDatabaseByUsername(username);
        }

        public static bool IsUserBanned(int userId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.IsUserBanned(userId);
        }
    }
}
