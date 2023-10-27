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
        bool IsLoginValid(string username, string password);

        [OperationContract]
        bool IsUsernameTaken(string username);

        [OperationContract]
        bool IsEmailTaken(String email);

        [OperationContract]
        bool SendFriendRequest(int senderID, int receiverID);

        [OperationContract]
        bool SendFriendResponse();

        [OperationContract]
        void GetFriendRequestsList(int userID);

        [OperationContract]
        List<String> GetOnlineFriends(string username);
    }

    [DataContract]
    public class User
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember] public string Password { get; set; }
    }

    [DataContract]
    public class FriendRequest
    {
        [DataMember]
        public string SenderID { get; set; }

        [DataMember]
        public string ReceiverID { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}
