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
        bool AddUserToDatabase(string username, string email, string password);

        [OperationContract]
        bool IsLoginValid(string email, string password);

        [OperationContract]
        bool IsUsernameTaken(string username);

        [OperationContract]
        bool IsEmailTaken(String email);

        [OperationContract]
        List<String> GetOnlineFriends(string username);
    }
}
