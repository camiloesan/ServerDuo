using Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
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
    public partial class ServiceImplementation : IPartyManager
    {
        static Dictionary<string, IPartyManagerCallback> instanceContextsDictionary = new Dictionary<string, IPartyManagerCallback>();

        public void JoinParty(string email)
        {
            IPartyManagerCallback operationContext = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();

            instanceContextsDictionary.Add(email, operationContext);
            OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>().PlayerJoined(instanceContextsDictionary);
        }

        public void LeaveParty(string email)
        {
            instanceContextsDictionary.Remove(email);
            OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>().PlayerLeft(instanceContextsDictionary);
        }

        public void SendMessage(string message)
        {
            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in instanceContextsDictionary)
            {
                keyValuePair.Value.MessageReceived(message);
            }
        }
    }
}
