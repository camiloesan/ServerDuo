﻿using Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

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
                    Password = user.Password,
                    TotalWins = 0
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

        public List<FriendRequest> GetFriendRequestsList(int userId)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendRequestsList = databaseContext.FriendRequests
                    .Where(request => request.UserReceiver == userId).Include(friendRequests => friendRequests.Users1)
                    .Include(friendRequests1 => friendRequests1.Users)
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

                var resultUser = new User
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
                int senderId;
                int receiverId;
                try
                {
                    senderId = databaseContext.Users.First(user => user.Username == usernameSender).UserID;
                    receiverId = databaseContext.Users.First(user => user.Username == usernameReceiver).UserID;
                }
                catch
                {
                    return false;
                }

                var friendRequest = new FriendRequests
                {
                    UserSender = senderId,
                    UserReceiver = receiverId,
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

        public bool RejectFriendRequest(int friendRequestId)
        {
            using (var databaseContext = new DuoContext())
            {
                try
                {
                    var requestToDelete = databaseContext.FriendRequests.Find(friendRequestId);
                    databaseContext.FriendRequests.Remove(requestToDelete);
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        public List<Friendship> GetFriendsList(int userId)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendshipsList = databaseContext.Friendships
                    .Where(friendship => friendship.User2 == userId || friendship.User1 == userId)
                    .Include(friendships => friendships.Users).Include(friendships1 => friendships1.Users1)
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
                var userEntity = databaseContext.Users.FirstOrDefault(user => user.Username == username);

                if (userEntity == null) return false;
                
                databaseContext.Users.Remove(userEntity);
                databaseContext.SaveChanges();
                return true;
            }
        }

        public bool DeleteFriendshipById(int friendshipId)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendshipEntity = databaseContext
                    .Friendships.FirstOrDefault(friendship => friendship.FriendshipID == friendshipId);

                if (friendshipEntity == null) return false;
                
                databaseContext.Friendships.Remove(friendshipEntity);
                databaseContext.SaveChanges();
                
                return true;
            }
        }

        public bool IsFriendRequestAlreadyExistent(string usernameSender, string usernameReceiver)
        {
            using (var databaseContext = new DuoContext())
            {
                int senderId;
                int receiverId;
                try
                {
                    senderId = databaseContext.Users.First(user => user.Username == usernameSender).UserID;
                    receiverId = databaseContext.Users.First(user => user.Username == usernameReceiver).UserID;
                }
                catch
                {
                    return false;
                }

                var friendRequestEntity = databaseContext.FriendRequests
                    .FirstOrDefault(friendRequest =>
                    (friendRequest.UserSender == senderId && friendRequest.UserReceiver == receiverId)
                    || (friendRequest.UserSender == receiverId && friendRequest.UserReceiver == senderId));
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

        public bool IsUserAlreadyLoggedIn(int userId)
        {
            return _onlineUsers.ContainsKey(userId);
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ServiceImplementation : IPartyManager
    {
        static ConcurrentDictionary<int, ConcurrentDictionary<string, IPartyManagerCallback>> _activePartiesDictionary = new ConcurrentDictionary<int, ConcurrentDictionary<string, IPartyManagerCallback>>();

        public void NotifyCreateParty(int partyCode, string hostUsername)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();

            var partyContextsDictionary = new ConcurrentDictionary<string, IPartyManagerCallback>();

            partyContextsDictionary.TryAdd(hostUsername, callback);
            _activePartiesDictionary.TryAdd(partyCode, partyContextsDictionary);

            callback.PartyCreated(partyContextsDictionary);
        }

        public void NotifyJoinParty(int partyCode, string username)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IPartyManagerCallback>();

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


        public void NotifyKickPlayer(int partyCode, string username)
        {
            _activePartiesDictionary[partyCode][username].PlayerKicked();

            _activePartiesDictionary[partyCode].TryRemove(username, out _);

            foreach (var player in _activePartiesDictionary[partyCode])
            {
                player.Value.PlayerLeft(_activePartiesDictionary[partyCode]);
            }
        }

        public async void NotifyStartGame(int partyCode)
        {
            _gameCards.TryAdd(partyCode, new Card[3]);

            for (int i = 0; i < _gameCards[partyCode].Length; i++)
            {
                _gameCards[partyCode][i] = new Card();
                _gameCards[partyCode][i].Number = "";
            }

            DealCards(partyCode);

            foreach (var player in _activePartiesDictionary[partyCode])
            {
                player.Value.GameStarted();
            }

            await Task.Delay(5000);
            _activePartiesDictionary.TryRemove(partyCode, out _);
        }
    }

    public partial class ServiceImplementation : IUserConnectionHandler
    {
        static ConcurrentDictionary<int, IUserConnectionHandlerCallback> _onlineUsers = new ConcurrentDictionary<int, IUserConnectionHandlerCallback>();

        public void NotifyLogIn(User user)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IUserConnectionHandlerCallback>();
            _onlineUsers.TryAdd(user.ID, callback);

            using (var databaseContext = new DuoContext())
            {
                var friendshipsList = databaseContext.Friendships
                    .Where(friendship => friendship.User2 == user.ID || friendship.User1 == user.ID)
                    .Include(friendships => friendships.Users).Include(friendships1 => friendships1.Users1)
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
            _onlineUsers.TryRemove(user.ID, out _);
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

        public bool IsUsernameInParty(int partyCode, string username)
        {
            return _activePartiesDictionary[partyCode].ContainsKey(username);
        }
    }

    public partial class ServiceImplementation : IMatchManager
    {
        private static ConcurrentDictionary<int, ConcurrentDictionary<string, int>> _matchResults = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();
        private static ConcurrentDictionary<int, ConcurrentDictionary<string, IMatchManagerCallback>> _playerCallbacks = new ConcurrentDictionary<int, ConcurrentDictionary<string, IMatchManagerCallback>>();
        private static ConcurrentDictionary<int, int> _currentTurn = new ConcurrentDictionary<int, int>();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

            if (!_currentTurn.ContainsKey(partyCode))
            {
                _currentTurn.TryAdd(partyCode, 0);
            }
        }

        public void SetGameScore(int partyCode, string username, int cardCount)
        {
            if (_matchResults.ContainsKey(partyCode))
            {
                _matchResults[partyCode].TryAdd(username, cardCount);
            }
            else
            {
                ConcurrentDictionary<string, int> playerScore = new ConcurrentDictionary<string, int>();
                playerScore.TryAdd(username, cardCount);

                _matchResults.TryAdd(partyCode, playerScore);
            }
        }

        public void KickPlayerFromGame(int partyCode, string username)
        {
            NotifyPlayerQuit(partyCode, username);
        }

        public async void EndGame(int partyCode)
        {
            await Task.Delay(1000);

            NotifyEndGameAsync(partyCode);
        }

        public void EndTurn(int partyCode)
        {
            List<string> playerList = new List<string>(_playerCallbacks[partyCode].Keys);
            _currentTurn[partyCode] = (_currentTurn[partyCode] + 1) % playerList.Count;

            NotifyEndTurn(partyCode);
        }

        public string GetCurrentTurn(int partyCode)
        {
            List<string> playerList = new List<string>(_playerCallbacks[partyCode].Keys);

            return playerList[_currentTurn[partyCode]];
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
            string currentCallback = "";

            try
            {
                foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
                {
                    currentCallback = player.Key;
                    player.Value.TurnFinished(playerList[_currentTurn[partyCode]]);
                }
            }
            catch 
            {
                NotifyPlayerQuit(partyCode, currentCallback);
            }
            
        }

        private async Task NotifyEndGameAsync(int partyCode)
        {
            string currentCallback = "";

            try
            {
                foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
                {
                    currentCallback = player.Key;
                    player.Value.GameOver();
                }    
            }
            catch
            {
                NotifyPlayerQuit(partyCode, currentCallback);
            }

            //Match data will be deleted so players start out with fresh data every match
            using (DuoContext databaseContext = new DuoContext())
            {
                string winner = _matchResults[partyCode].OrderBy(x => x.Value).First().Key;
                int winnerUserId = databaseContext.Users.Where(u => u.Username == winner).Select(u => u.UserID).FirstOrDefault();

                if (winnerUserId > 0)
                {
                    try
                    {
                        Users user = databaseContext.Users.Where(u => u.UserID == winnerUserId).FirstOrDefault();

                        if (user != null)
                        {
                            user.TotalWins++;

                            databaseContext.SaveChanges();
                        }
                    }
                    catch (DbUpdateException exception)
                    {
                        log.Error("An error happened while trying to save a match into the DB", exception);
                    }
                }
            }

            await Task.Delay(10000);
            _gameCards.TryRemove(partyCode, out _);
            _currentTurn.TryRemove(partyCode, out _);
            _playerCallbacks.TryRemove(partyCode, out _);
            _matchResults.TryRemove(partyCode, out _);
        }

        private async void NotifyPlayerQuit(int partyCode, string username)
        {
            List<string> playerList = new List<string>(_playerCallbacks[partyCode].Keys);
            string currentPlayerTurn = playerList[_currentTurn[partyCode]];

            if (currentPlayerTurn.Equals(username))
            {
                EndTurn(partyCode);
            }

            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                try
                {
                    if (_playerCallbacks[partyCode].Count > 1)
                    {
                        player.Value.PlayerLeftGame(username);
                    }
                    else
                    {
                        if (!player.Key.Equals(username))
                        {
                            EndGame(partyCode);
                        }
                    }
                }
                catch(TimeoutException ex)
                {
                    log.Error("Timeout error while kicking a player from a match", ex);
                }
            }

            _playerCallbacks[partyCode].TryRemove(username, out _);
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
            CardColors.RED
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
            if (_gameCards[partyCode][position].Number.Equals("") && !card.Number.Equals("#"))
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
