    using Database;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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

        public List<FriendRequest> GetFriendRequestsList(int userID)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendResquestsList = databaseContext.FriendRequests
                    .Where(request => request.UserReceiver == userID)
                    .ToList();

                List<FriendRequest> list = new List<FriendRequest>();
                foreach (var item in friendResquestsList)
                {
                    FriendRequest friendRequest = new FriendRequest
                    {
                        FriendRequestID = item.RequestID,
                        SenderID = (int)item.UserSender,
                        SenderUsername = item.Users1.Username,
                        ReceiverID = (int)item.UserReceiver,
                        ReceiverUsername = item.Users.Username
                    };

                    list.Add(friendRequest);
                }

                return list;
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

        public User IsLoginValid(string username, string password)
        {
            using (var databaseContext = new DuoContext())
            {
                Users databaseUser;
                try
                {
                    databaseUser = databaseContext.Users.First(user => user.Username == username && user.Password == password);
                }
                catch
                {
                    return null;
                }

                User userData = new User
                {
                    ID = databaseUser.UserID,
                    UserName = databaseUser.Username,
                    Email = databaseUser.Email
                };
                return userData;
            }
        }

        public bool IsUsernameTaken(string username)
        {
            using (var databaseContext = new DuoContext())
            {
                return databaseContext.Users.Any(user => username == user.Username);
            }
        }

        public bool SendFriendRequest(string usernameSender, string usernameReceiver)
        {
            using (var databaseContext = new DuoContext())
            {
                int senderID;
                try
                {
                    Users userSender = databaseContext.Users.First(user => user.Username == usernameSender);
                    senderID = userSender.UserID;
                }
                catch
                {
                    return false;
                }

                int receiverID;
                try
                {
                    Users userReceiver = databaseContext.Users.First(user => user.Username == usernameReceiver);
                    receiverID = userReceiver.UserID;
                }
                catch
                {
                    return false;
                }
                

                var friendRequest = new FriendRequests
                {
                    UserSender = senderID,
                    UserReceiver = receiverID,
                    Status = "pending"
                };

                try
                {
                    databaseContext.FriendRequests.Add(friendRequest);
                    databaseContext.SaveChanges();
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        public bool AcceptFriendRequest(FriendRequest friendRequest)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendship = new Friendships
                {
                    User1 = friendRequest.SenderID, 
                    User2 = friendRequest.ReceiverID,
                };

                try
                {
                    databaseContext.Friendships.Add(friendship);
                    var requestToDelete = databaseContext.FriendRequests.Find(friendRequest.FriendRequestID);
                    databaseContext.FriendRequests.Remove(requestToDelete);
                    databaseContext.SaveChanges();
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        public bool RejectFriendRequest(int friendRequestID)
        {
            using (var databaseContext = new DuoContext())
            {
                try
                {
                    var requestToDelete = databaseContext.FriendRequests.Find(friendRequestID);
                    databaseContext.FriendRequests.Remove(requestToDelete);
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        public List<Friendship> GetFriendsList(int userID)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendshipsList = databaseContext.Friendships
                    .Where(friendship => friendship.User2 == userID || friendship.User1 == userID)
                    .ToList();

                List<Friendship> friendships = new List<Friendship>();
                foreach (var item in friendshipsList)
                {
                    Friendship friendship = new Friendship()
                    {
                        FriendshipID = item.FriendshipID,
                        Friend1ID = (int)item.User1,
                        Friend1Username = item.Users.Username,
                        Friend2ID = (int)item.User2,
                        Friend2Username = item.Users1.Username
                    };
                    friendships.Add(friendship);
                }
                return friendships;
            }
        }

        public bool DeleteUserFromDatabaseByUsername(string username)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = false;
                var userEntity = databaseContext.Users.FirstOrDefault(user => user.Username == username);

                if (userEntity != null)
                {
                    databaseContext.Users.Remove(userEntity);
                    databaseContext.SaveChanges();
                    result = true;
                }

                return result;
            }
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ServiceImplementation : IPartyManager
    {
        static ConcurrentDictionary<int, ConcurrentDictionary<string, IPartyManagerCallback>> activePartiesDictionary = new ConcurrentDictionary<int, ConcurrentDictionary<string, IPartyManagerCallback>>();

        public void NewParty(int partyCode, string username)
        {
            IPartyManagerCallback operationContext = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();
            ConcurrentDictionary<string, IPartyManagerCallback> partyContextsDictionary = new ConcurrentDictionary<string, IPartyManagerCallback>();
            partyContextsDictionary.TryAdd(username, operationContext);
            activePartiesDictionary.TryAdd(partyCode, partyContextsDictionary);

            operationContext.NotifyPartyCreated(partyContextsDictionary);
        }

        public void JoinParty(int partyCode, string username)
        {
            IPartyManagerCallback operationContext = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();

            activePartiesDictionary[partyCode].TryAdd(username, operationContext);
            foreach (var item in activePartiesDictionary[partyCode])
            {
                item.Value.NotifyPlayerJoined(activePartiesDictionary[partyCode]);
            }

        }

        public void LeaveParty(int partyCode, string username)
        {
            activePartiesDictionary[partyCode].TryRemove(username, out _);
            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in activePartiesDictionary[partyCode])
            {
                keyValuePair.Value.NotifyPlayerLeft(activePartiesDictionary[partyCode]);
            }
        }

        public void CloseParty(int partyCode)
        {
            foreach (var item in activePartiesDictionary[partyCode])
            {
                item.Value.NotifyPlayerKicked();
            }
            activePartiesDictionary.TryRemove(partyCode, out _);
        }

        public void SendMessage(int partyCode, string message)
        {
            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in activePartiesDictionary[partyCode])
            {
                keyValuePair.Value.NotifyMessageReceived(message);
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

        public void StartGame(int partyCode)
        {
            var partyMap = activePartiesDictionary[partyCode];

            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in activePartiesDictionary[partyCode])
            {
                keyValuePair.Value.NotifyGameStarted();
            }
        }

        public void KickPlayer(int partyCode, string username)
        {
            activePartiesDictionary[partyCode][username].NotifyPlayerKicked();

            activePartiesDictionary[partyCode].TryRemove(username, out _);

            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in activePartiesDictionary[partyCode])
            {
                keyValuePair.Value.NotifyPlayerLeft(activePartiesDictionary[partyCode]);
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
            return activePartiesDictionary[partyCode].Count < 4;
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
