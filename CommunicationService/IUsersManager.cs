using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CommunicationService
{
    [ServiceContract]
    public interface IUsersManager
    {
        [OperationContract]
        bool AddUserToDatabase(User user);

        [OperationContract]
        bool DeleteUserFromDatabaseByUsername(String username);

        [OperationContract]
        User IsLoginValid(string username, string password);

        [OperationContract]
        bool IsUsernameTaken(string username);

        [OperationContract]
        bool IsEmailTaken(String email);

        [OperationContract]
        bool SendFriendRequest(string usernameSender, string usernameReceiver);

        [OperationContract]
        bool AcceptFriendRequest(FriendRequest friendRequest);

        [OperationContract]
        bool RejectFriendRequest(int friendRequestID);

        [OperationContract]
        List<FriendRequest> GetFriendRequestsList(int userID);

        [OperationContract]
        List<Friendship> GetFriendsList(int userID);

        [OperationContract]
        List<String> GetOnlineFriends(string username);
    }

    [DataContract]
    public class User
    {
        [DataMember]
        public int ID {  get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember] public string Password { get; set; }
    }

    [DataContract]
    public class Friendship
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
    public class FriendRequest
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
}
