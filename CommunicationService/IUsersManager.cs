using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CommunicationService
{
    [ServiceContract]
    public interface IUsersManager
    {
        [OperationContract]
        bool UpdateProfilePictureByUserId(int userId, int pictureId);

        [OperationContract]
        UserDTO GetUserInfoByUsername(string username);

        [OperationContract]
        int AddUserToDatabase(UserDTO user);

        [OperationContract]
        bool DeleteUserFromDatabaseByUsername(String username);

        [OperationContract]
        UserDTO IsLoginValid(string username, string password);

        [OperationContract]
        bool IsUsernameTaken(string username);

        [OperationContract]
        bool IsEmailTaken(string email);

        [OperationContract]
        bool SendFriendRequest(string usernameSender, string usernameReceiver);

        [OperationContract]
        bool IsFriendRequestAlreadyExistent(string usernameSender, string usernameReceiver);

        [OperationContract]
        bool AcceptFriendRequest(FriendRequestDTO friendRequest);

        [OperationContract]
        bool RejectFriendRequest(int friendRequestId);

        [OperationContract]
        List<FriendRequestDTO> GetFriendRequestsList(int userId);

        [OperationContract]
        List<FriendshipDTO> GetFriendsList(int userId);

        [OperationContract]
        List<FriendshipDTO> GetOnlineFriends(int userId);

        [OperationContract]
        bool DeleteFriendshipById(int friendshipId);

        [OperationContract]
        bool IsAlreadyFriend(string senderUsername, string receiverUsername);

        [OperationContract]
        bool IsUserAlreadyLoggedIn(int userId);

        [OperationContract]
        int SendConfirmationCode(string email, string lang);

        [OperationContract]
        bool ModifyPasswordByEmail(string email, string newPassword);

        [OperationContract]
        bool BlockUserByUsername(string blockerUsername, string blockedUsername);

        [OperationContract]
        bool UnblockUserByBlockId(int blockId);

        [OperationContract]
        bool IsUserBlockedByUsername(string usernameBlocker, string usernameBlocked);

        [OperationContract]
        List<UserBlockedDTO> GetBlockedUsersListByUserId(int userId);

        [OperationContract]
        List<UserDTO> GetTopTenWinners();
    }

    [ServiceContract(CallbackContract = typeof(IUserConnectionHandlerCallback))]
    public interface IUserConnectionHandler
    {
        [OperationContract]
        void NotifyLogIn(UserDTO user);

        [OperationContract]
        void NotifyLogOut(UserDTO user);
    }

    [ServiceContract]
    public interface IUserConnectionHandlerCallback
    {
        [OperationContract]
        void UserLogged(string username);
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
    }
}
