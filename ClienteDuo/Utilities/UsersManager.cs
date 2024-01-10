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

        public static UserDTO GetUserInfoByUsername(string username)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.GetUserInfoByUsername(username);
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
