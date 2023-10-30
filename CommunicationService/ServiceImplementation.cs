    using Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
<<<<<<< HEAD
using System.Net.Mime;
=======
using System.Runtime.Remoting.Metadata.W3cXsd2001;
>>>>>>> 386a2af845595e4a3d49aac3d51bf1d15f7a3ce9
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

        public bool AddUserToDatabase(User user)
        {
            using (var databaseContext = new DuoContext())
            {
                var databaseUser = new Users
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Password = user.Password
                };

                try
                {
                    databaseContext.Users.Add(databaseUser);
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

        public void GetFriendRequestsList(int userID)
        {
            using (var databaseContext = new DuoContext())
            {
                var requestInfo = databaseContext.FriendRequests.Where(request => request.UserReceiver == userID);
                
                foreach (var i in requestInfo)
                {
                    Console.Write(i.UserSender);
                }
            }
        }

        public List<string> GetOnlineFriends(string username)
        {
            throw new NotImplementedException();
        }

        public bool IsEmailTaken(string email)
        {
            using (var databaseContext = new DuoContext())
            {
                return databaseContext.Users.Any(user => user.Email == email);
            }
        }

        public bool IsLoginValid(string username, string password)
        {
            using (var databaseContext = new DuoContext())
            {
                bool exists = databaseContext.Users.Any(user => user.Username == username && user.Password == password);
                return exists;
            }
        }

        public bool IsUsernameTaken(string username)
        {
            using (var databaseContext = new DuoContext())
            {
                return databaseContext.Users.Any(user => username == user.Username);
            }
        }

        public bool SendFriendRequest(int senderID, int receiverID)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendRequest = new FriendRequests
                {
                    UserSender = senderID,
                    UserReceiver = receiverID,
                };

                try
                {
                    databaseContext.FriendRequests.Add(friendRequest);
                    databaseContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }
        }

        public bool SendFriendResponse()
        {
            throw new NotImplementedException();
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ServiceImplementation : IPartyManager
    {
        static Dictionary<int, Dictionary<string, IPartyManagerCallback>> activePartiesDictionary = new Dictionary<int, Dictionary<string, IPartyManagerCallback>>();
        static Dictionary<string, IPartyManagerCallback> partyContextsDictionary;

        public void NewParty(int partyCode, string username)
        {
            partyContextsDictionary = new Dictionary<string, IPartyManagerCallback>();
            IPartyManagerCallback operationContext = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();

            partyContextsDictionary.Add(username, operationContext);
            activePartiesDictionary.Add(partyCode, partyContextsDictionary);

            operationContext.PartyCreated(partyContextsDictionary);
        }

        public void JoinParty(int partyCode, string username)
        {
            IPartyManagerCallback operationContext = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();

            var partyMap = activePartiesDictionary[partyCode];

            partyMap.Add(username, operationContext);

            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in partyMap)
            {
                keyValuePair.Value.PlayerJoined(partyMap);
            }
        }

        public void LeaveParty(int partyCode, string username)
        {
            var partyMap = activePartiesDictionary[partyCode];
            partyMap.Remove(username);

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

        public void IsPlayerActive()
        {
            //var partyMap = activePartiesDictionary[partyCode];

            foreach (var value in activePartiesDictionary.Values)
            {
                foreach (var context in value)
                {
                    //if (context.Value.GetStatus()) ;
                }
            }
        }
    }

    public partial class ServiceImplementation : IPartyValidator
    {
        public bool IsPartyExistent(int partyCode)
        {
            return activePartiesDictionary.ContainsKey(partyCode);
        }

        public bool IsSpaceAvailable(int partyCode)
        {
            if (activePartiesDictionary.ContainsKey(partyCode))
            {
                var partyMap = activePartiesDictionary[partyCode];
                
                if (partyMap.Count == 4)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public partial class ServiceImplementation : IMatchManager
    {
        static Card[] _tableCards = new Card[3];
        static Random _numberGenerator = new Random();

        static readonly List<string> _cardColors = new List<string>()
        {
            "#0000FF", //Blue
            "#FFFF00", //Yellow
            "#008000", //Green
            "#FF0000"  //Red
        };
        static readonly List<(string, int)> _cardNumbers = new List<(string, int)>()
        {
            ("1", 12),
            ("2", 12),
            ("3", 12),
            ("4", 12),
            ("5", 12),
            ("6", 8),
            ("7", 8),
            ("8", 8),
            ("9", 8),
            ("10", 8),
            ("#", 8)
        };

        public void InitializeData()
        {
            for (int i = 0; i < _tableCards.Length; i++)
            {
                _tableCards[i] = new Card();
                _tableCards[i].Number = "";
            }
        }

        public void DealTableCards()
        {
            for (int i = 0; i < _tableCards.Length - 1; i++)
            {
                if (_tableCards[i].Number == "")
                {
                    do
                    {
                        _tableCards[i].Number = _numberGenerator.Next(1, 11).ToString();
                    } while (_tableCards[i].Number == "2");

                    _tableCards[i].Color = _cardColors[_numberGenerator.Next(0, 4)];
                }
            }
        }

        public Card DrawCard()
        {
            Card _card = new Card();
            int _accumulatedWeight = 0;
            int _cardNumber = _numberGenerator.Next(0, 120) + 1; //108 is the total of cards in a standard DUO deck

            foreach (var (number, weight) in _cardNumbers)
            {
                _accumulatedWeight += weight;

                if (_accumulatedWeight <= _cardNumber)
                {
                    _card.Number = number;
                }
            }

            if (_card.Number.CompareTo("2") != 0)
            {
                _card.Color = _cardColors[_numberGenerator.Next(0, 4)];
            }

            return _card;
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }

        public void EndRound()
        {
            throw new NotImplementedException();
        }

        public void EndTurn()
        {
            throw new NotImplementedException();
        }

        public Card[] GetTableCards()
        {
            return _tableCards;
        }

        public void PlayCard(int position)
        {
            _tableCards[position].Number = "";
        }
    }
}
