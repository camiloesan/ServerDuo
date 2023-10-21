using Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace CommunicationService
{
    public partial class ServiceImplementation : IUsersManager
    {
        // temporal fix of onlinplayers, should be binary tree somehow
        public List<string> onlineUsers = new List<string>();

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

        public List<string> GetOnlineFriends(string username)
        {
            throw new NotImplementedException();
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
        static Dictionary<int, Dictionary<string, IPartyManagerCallback>> activePartiesDictionary = new Dictionary<int, Dictionary<string, IPartyManagerCallback>>();
        static Dictionary<string, IPartyManagerCallback> partyContextsDictionary;

        public void NewParty(int partyCode, string email) //should be int, user
        {
            partyContextsDictionary = new Dictionary<string, IPartyManagerCallback>();
            IPartyManagerCallback operationContext = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            partyContextsDictionary.Add(email, operationContext);
            activePartiesDictionary.Add(partyCode, partyContextsDictionary);

            operationContext.PartyCreated(partyContextsDictionary);
        }

        public void JoinParty(int partyCode, string email) //should be int, user or at least username
        {
            IPartyManagerCallback operationContext = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();

            var partyMap = activePartiesDictionary[partyCode];

            partyMap.Add(email, operationContext);

            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in partyMap)
            {
                keyValuePair.Value.PlayerJoined(partyMap);
            }
        }

        public void LeaveParty(int partyCode, string email)
        {
            var partyMap = activePartiesDictionary[partyCode];
            partyMap.Remove(email);

            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in partyMap)
            {
                keyValuePair.Value.PlayerLeft(partyContextsDictionary);
            }
        }

        public void SendMessage(int partyCode, string message)
        {
            var partyMap = activePartiesDictionary[partyCode];

            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in partyMap)
            {
                keyValuePair.Value.MessageReceived(message);
            }
        }
    }
}
