using Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

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
                catch
                {
                    return false;
                }
                return true;
            }
        }

        public List<FriendRequest> GetFriendRequestsList(int userID)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendRequestsList = databaseContext.FriendRequests
                    .Where(request => request.UserReceiver == userID)
                    .ToList();

                List<FriendRequest> resultList = new List<FriendRequest>();
                foreach (var friendRequestItem in friendRequestsList)
                {
                    FriendRequest friendRequest = new FriendRequest
                    {
                        FriendRequestID = friendRequestItem.RequestID,
                        SenderID = (int)friendRequestItem.UserSender,
                        SenderUsername = friendRequestItem.Users1.Username,
                        ReceiverID = (int)friendRequestItem.UserReceiver,
                        ReceiverUsername = friendRequestItem.Users.Username
                    };
                    resultList.Add(friendRequest);
                }
                return resultList;
            }
        }

        public List<string> GetOnlineFriends(string username)
        {
            //todo
            return null;
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

                User resultUser = new User
                {
                    ID = databaseUser.UserID,
                    UserName = databaseUser.Username,
                    Email = databaseUser.Email
                };
                return resultUser;
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
                int receiverID;
                try
                {
                    senderID = databaseContext.Users.First(user => user.Username == usernameSender).UserID;
                    receiverID = databaseContext.Users.First(user => user.Username == usernameReceiver).UserID;
                }
                catch
                {
                    return false;
                }

                FriendRequests friendRequest = new FriendRequests
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
                    var friendRequestToDelete = databaseContext.FriendRequests.Find(friendRequest.FriendRequestID);
                    databaseContext.FriendRequests.Remove(friendRequestToDelete);
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

                List<Friendship> resultList = new List<Friendship>();
                foreach (var friendshipItem in friendshipsList)
                {
                    Friendship friendship = new Friendship()
                    {
                        FriendshipID = friendshipItem.FriendshipID,
                        Friend1ID = (int)friendshipItem.User1,
                        Friend1Username = friendshipItem.Users.Username,
                        Friend2ID = (int)friendshipItem.User2,
                        Friend2Username = friendshipItem.Users1.Username
                    };
                    resultList.Add(friendship);
                }
                return resultList;
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

        public bool DeleteFriendshipByID(int friendshipID)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = false;
                var friendshipEntity = databaseContext.Friendships.FirstOrDefault(friendship => friendship.FriendshipID == friendshipID);

                if (friendshipEntity != null)
                {
                    databaseContext.Friendships.Remove(friendshipEntity);
                    databaseContext.SaveChanges();
                    result = true;
                }
                return result;
            }
        }

        public bool IsFriendRequestAlreadyExistent(string usernameSender, string usernameReceiver)
        {
            using (var databaseContext = new DuoContext())
            {
                int senderID;
                int receiverID;
                try
                {
                    senderID = databaseContext.Users.First(user => user.Username == usernameSender).UserID;
                    receiverID = databaseContext.Users.First(user => user.Username == usernameReceiver).UserID;
                }
                catch
                {
                    return false;
                }

                var friendRequestEntity = databaseContext.FriendRequests
                    .FirstOrDefault(friendRequest =>
                    (friendRequest.UserSender == senderID && friendRequest.UserReceiver == receiverID)
                    || (friendRequest.UserSender == receiverID && friendRequest.UserReceiver == senderID));
                return friendRequestEntity != null;
            }
        }

        public bool IsAlreadyFriend(string senderUsername, string receiverUsername)
        {
            using (var databaseContext = new DuoContext())
            {
                Users userSender;
                Users userReceiver;
                try
                {
                    userSender = databaseContext.Users
                        .First(user => user.Username == senderUsername);
                    userReceiver = databaseContext.Users
                        .First(user => user.Username == receiverUsername);
                }
                catch
                {
                    return false;
                }

                var friendshipEntity = databaseContext.Friendships
                    .FirstOrDefault(friendship =>
                    (friendship.User1 == userSender.UserID && friendship.User2 == userReceiver.UserID)
                    || (friendship.User1 == userReceiver.UserID && friendship.User2 == userSender.UserID));
                return friendshipEntity != null;
            }
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ServiceImplementation : IPartyManager
    {
        static ConcurrentDictionary<int, ConcurrentDictionary<string, IPartyManagerCallback>> _activePartiesDictionary
            = new ConcurrentDictionary<int, ConcurrentDictionary<string, IPartyManagerCallback>>();

        public void NotifyCreateParty(int partyCode, string hostUsername)
        {
            IPartyManagerCallback callback;
            callback = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();

            ConcurrentDictionary<string, IPartyManagerCallback> partyContextsDictionary;
            partyContextsDictionary = new ConcurrentDictionary<string, IPartyManagerCallback>();

            partyContextsDictionary.TryAdd(hostUsername, callback);
            _activePartiesDictionary.TryAdd(partyCode, partyContextsDictionary);

            callback.PartyCreated(partyContextsDictionary);
        }

        public void NotifyJoinParty(int partyCode, string username)
        {
            IPartyManagerCallback callback;
            callback = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();

            _activePartiesDictionary[partyCode].TryAdd(username, callback);
            foreach (var activeParty in _activePartiesDictionary[partyCode])
            {
                activeParty.Value.PlayerJoined(_activePartiesDictionary[partyCode]);
            }
        }

        public void NotifyLeaveParty(int partyCode, string username)
        {
            _activePartiesDictionary[partyCode].TryRemove(username, out _);
            foreach (var player in _activePartiesDictionary[partyCode])
            {
                player.Value.PlayerLeft(_activePartiesDictionary[partyCode]);
            }
        }

        public void NotifyCloseParty(int partyCode)
        {
            foreach (var player in _activePartiesDictionary[partyCode])
            {
                player.Value.PlayerKicked();
            }
            _activePartiesDictionary.TryRemove(partyCode, out _);
        }

        public void NotifySendMessage(int partyCode, string message)
        {
            foreach (var player in _activePartiesDictionary[partyCode])
            {
                player.Value.MessageReceived(message);
            }
        }

        public void NotifyStartGame(int partyCode)
        {
            foreach (var player in _activePartiesDictionary[partyCode])
            {
                player.Value.GameStarted();
            }
        }

        public void NotifyKickPlayer(int partyCode, string username)
        {
            _activePartiesDictionary[partyCode][username].PlayerKicked();

            _activePartiesDictionary[partyCode].TryRemove(username, out _);

            foreach (var player in _activePartiesDictionary[partyCode])
            {
                player.Value.PlayerLeft(_activePartiesDictionary[partyCode]);
            }
        }
    }

    public partial class ServiceImplementation : IUserConnectionHandler
    {
        static ConcurrentDictionary<int, IUserConnectionHandlerCallback> _onlineUsers = new ConcurrentDictionary<int, IUserConnectionHandlerCallback>();

        public void NotifyLogIn(User user)
        {
            IUserConnectionHandlerCallback operationContext = OperationContext.Current.GetCallbackChannel<IUserConnectionHandlerCallback>();
            _onlineUsers.TryAdd(user.ID, operationContext);

            using (var databaseContext = new DuoContext())
            {
                var friendshipsList = databaseContext.Friendships
                                    .Where(friendship => friendship.User2 == user.ID || friendship.User1 == user.ID)
                                    .ToList();
                foreach (var friend in friendshipsList)
                {
                    if (friend.User1 == user.ID)
                    {
                        if (_onlineUsers.ContainsKey((int)friend.User2))
                        {
                            _onlineUsers[friend.Users1.UserID].UserLogged(friend.Users.Username);
                        }
                    }
                    else if (friend.User2 == user.ID)
                    {
                        if (_onlineUsers.ContainsKey((int)friend.User1))
                        {
                            _onlineUsers[friend.Users.UserID].UserLogged(friend.Users1.Username);
                        }
                    }
                }
            }
        }

        public void NotifyLogOut(User user)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ServiceImplementation : IPartyValidator
    {
        public bool IsPartyExistent(int partyCode)
        {
            return _activePartiesDictionary.ContainsKey(partyCode);
        }

        public bool IsSpaceAvailable(int partyCode)
        {
            return _activePartiesDictionary[partyCode].Count < 4;
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
