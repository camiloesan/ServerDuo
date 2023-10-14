using Database;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CommunicationService
{
    public partial class ServiceImplementation : IUsersManager
    {
        public bool AddUserToDatabase(string username, string email, string password)
        {
            using (var databaseContext = new DuoContext())
            {
                var user = new Users
                {
                    Username = username,
                    Email = email,
                    Password = password
                };

                try
                {
                    databaseContext.Users.Add(user);
                    databaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsLoginValid(string email, string password)
        {
            using (var databaseContext = new DuoContext())
            {
                bool exists = databaseContext.Users.Any(user => user.Email == email && user.Password == password);
                return exists;
            }
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ServiceImplementation : IMessageService
    {
        public void SendMessage(string message)
        {
            OperationContext.Current.GetCallbackChannel<IMessageServiceCallback>().MessageReceived(message);
        }
    }
}
