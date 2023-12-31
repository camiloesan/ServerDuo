﻿    using Database;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Runtime.InteropServices;
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
                try
                {
                    userSender = databaseContext.Users.First(user => user.Username == senderUsername);
                }
                catch
                {
                    return false;
                }

                Users userReceiver;
                try
                {
                    userReceiver = databaseContext.Users.First(user => user.Username == receiverUsername);
                }
                catch
                {
                    return false;
                }

                var friendshipEntity = databaseContext.Friendships.FirstOrDefault(friendship => 
                (friendship.User1 == userSender.UserID && friendship.User2 == userReceiver.UserID)
                || (friendship.User1 == userReceiver.UserID && friendship.User2 == userSender.UserID));

                return friendshipEntity != null;
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
            
            _gameCards.TryAdd(partyCode, new Card[3]);

            for (int i = 0; i < _gameCards[partyCode].Length; i++)
            {
                _gameCards[partyCode][i] = new Card();
                _gameCards[partyCode][i].Number = "";
            }

            DealCards(partyCode);

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
                        if(_onlineUsers.ContainsKey((int)friend.User1))
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
            return activePartiesDictionary.ContainsKey(partyCode);
        }

        public bool IsSpaceAvailable(int partyCode)
        {
            return activePartiesDictionary[partyCode].Count < 4;
        }
    }

    public partial class ServiceImplementation : IMatchManager
    {
        private static ConcurrentDictionary<int, ConcurrentDictionary<string, int>> _matchResults = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();
        private static ConcurrentDictionary<int, ConcurrentDictionary<string, IMatchManagerCallback>> _playerCallbacks = new ConcurrentDictionary<int, ConcurrentDictionary<string, IMatchManagerCallback>>();
        private static ConcurrentDictionary<int, int> currentTurn = new ConcurrentDictionary<int, int>();

        public void Subscribe(int partyCode, string username)
        {
            IMatchManagerCallback playerCallback = OperationContext.Current.GetCallbackChannel<IMatchManagerCallback>();

            if (_playerCallbacks.ContainsKey(partyCode))
            {
                _playerCallbacks[partyCode].TryAdd(username, playerCallback);
            }
            else
            {
                ConcurrentDictionary<string, IMatchManagerCallback> player = new ConcurrentDictionary<string, IMatchManagerCallback>();
                player.TryAdd(username, playerCallback);

                _playerCallbacks.TryAdd(partyCode, player);
            }

            if (_matchResults.ContainsKey(partyCode))
            {
                _matchResults[partyCode].TryAdd(username, 5);
            }
            else
            {
                ConcurrentDictionary<string, int> playerScore = new ConcurrentDictionary<string, int>();
                playerScore.TryAdd(username, 5);

                _matchResults.TryAdd(partyCode, playerScore);
            }
        }

        public void setGameScore(int partyCode, string username, int cardCount)
        {
            _matchResults[partyCode][username] = cardCount;
        }

        public void EndGame(int partyCode)
        {
            _gameCards.TryRemove(partyCode, out _);

            NotifyEndGame(partyCode);
        }

        public void EndTurn(int partyCode)
        {
            List<string> playerList = new List<string>(_playerCallbacks[partyCode].Keys);
            currentTurn[partyCode] = (currentTurn[partyCode] + 1) % playerList.Count;

            NotifyEndTurn(partyCode);
        }

        public string GetCurrentTurn(int partyCode)
        {
            List<string> playerList = new List<string>(_playerCallbacks[partyCode].Keys);

            return playerList[currentTurn[partyCode]];
        }

        public List<string> GetPlayerList(int partyCode)
        {
            return new List<string>(_playerCallbacks[partyCode].Keys);
        }

        public ConcurrentDictionary<string, int> GetMatchResults(int partyCode)
        {
            return _matchResults[partyCode];
        }

        private void NotifyEndTurn(int partyCode)
        {
            List<string> playerList = new List<string>(_playerCallbacks[partyCode].Keys);

            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                try
                {
                    player.Value.TurnFinished(playerList[currentTurn[partyCode]]);
                }
                catch
                {
                    _playerCallbacks[partyCode].TryRemove(player.Key, out _);
                    NotifyPlayerQuit(partyCode);
                }
            }
        }

        private void NotifyEndGame(int partyCode)
        {
            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                try
                {
                    player.Value.GameOver();
                }
                catch
                {
                    _playerCallbacks[partyCode].TryRemove(player.Key, out _);
                    NotifyPlayerQuit(partyCode);
                }
            }
        }

        private void NotifyPlayerQuit(int partyCode)
        {
            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                try
                {
                    player.Value.PlayerLeft(player.Key);
                }
                catch
                {
                    //TODO Log it
                }
            }
        }
    }

    public partial class ServiceImplementation : ICardManager
    {
        //Lists are stored for random card generation
        private static readonly List<string> _cardColors = new List<string>()
        {
            CardColors.BLUE,
            CardColors.YELLOW,
            CardColors.GREEN,
            CardColors.RED,
        };
        private static readonly List<(string, int)> _cardNumbers = new List<(string, int)>()
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
        private static ConcurrentDictionary<int, Card[]> _gameCards = new ConcurrentDictionary<int, Card[]>();
        private static Random _numberGenerator = new Random();

        public void DealCards(int gameId)
        {
            for (int i = 0; i < _gameCards[gameId].Length - 1; i++)
            {
                if (_gameCards[gameId][i].Number.Equals(""))
                {
                    do
                    {
                        _gameCards[gameId][i].Number = _numberGenerator.Next(1, 11).ToString();
                    } while (_gameCards[gameId][i].Number.Equals("2"));

                    _gameCards[gameId][i].Color = _cardColors[_numberGenerator.Next(0, 4)];
                }
            }
        }

        public Card DrawCard()
        {
            Card card = new Card();
            int accumulatedWeight = 0;
            int cardNumber = _numberGenerator.Next(0, 120) + 1;

            foreach (var (number, weight) in _cardNumbers)
            {
                accumulatedWeight += weight;

                if (accumulatedWeight <= cardNumber)
                {
                    card.Number = number;
                }
            }

            if (!card.Number.Equals("2"))
            {
                card.Color = _cardColors[_numberGenerator.Next(0, 4)];
            }

            return card;
        }

        public Card[] GetCards(int gameId)
        {
            return _gameCards[gameId];
        }

        public void PlayCard(int partyCode, int position, Card card)
        {
            if (_gameCards[partyCode][position].Number.Equals(""))
            {
                _gameCards[partyCode][position] = card;
            }
            else
            {
                _gameCards[partyCode][position].Number = "";
            }
        }
    }
}
