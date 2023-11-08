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
                User userData = new User();
                var databaseUser = databaseContext.Users.FirstOrDefault(user => user.Username == username && user.Password == password);

                if (databaseUser != null)
                {
                    userData.ID = databaseUser.UserID;
                    userData.UserName = databaseUser.Username;
                    userData.Email = databaseUser.Email;
                }
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

        public bool SendFriendRequest(int senderID, int receiverID)
        {
            using (var databaseContext = new DuoContext())
            {
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

                Console.WriteLine(friendshipsList.Count);

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

            foreach (KeyValuePair<string, IPartyManagerCallback> keyValuePair in partyMap)
            {
                keyValuePair.Value.GameStarted();
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
        static ConcurrentDictionary<int, ConcurrentDictionary<string, int>> _playerScores = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();
        static ConcurrentDictionary<int, ConcurrentDictionary<string, IMatchManagerCallback>> _playerCallbacks = new ConcurrentDictionary<int, ConcurrentDictionary<string, IMatchManagerCallback>>();
        static int currentTurn;

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

            if (_playerScores.ContainsKey(partyCode))
            {
                _playerScores[partyCode].TryAdd(username, 0);
            }
            else
            {
                ConcurrentDictionary<string, int> score = new ConcurrentDictionary<string, int>();
                score.TryAdd(username, 0);

                _playerScores.TryAdd(partyCode, score);
            }

            
        }

        public void EndGame(int partyCode)
        {
            _gameCards.TryRemove(partyCode, out _);
            _playerScores.TryRemove(partyCode, out _);

            NotifyEndGame(partyCode);
        }

        public void EndRound(int partyCode)
        {
            NotifyEndRound(partyCode);
        }

        public void EndTurn(int partyCode)
        {
            List<string> playerList = new List<string>(_playerScores[partyCode].Keys);
            currentTurn = (currentTurn + 1) % playerList.Count;

            NotifyEndTurn(partyCode);
        }

        public string GetCurrentTurn(int partyCode)
        {
            List<string> playerList = new List<string>(_playerScores[partyCode].Keys);
            currentTurn = (currentTurn + 1) % playerList.Count;

            return playerList[currentTurn];
        }

        public ConcurrentDictionary<string, int> GetPlayerScores(int partyCode)
        {
            if (_playerScores.TryGetValue(partyCode, out ConcurrentDictionary<string, int> playerScores))
            {
                return playerScores;
            }
            else
            {
                return new ConcurrentDictionary<string, int>();
            }
        }

        void NotifyEndTurn(int partyCode)
        {
            List<string> playerList = new List<string>(_playerScores[partyCode].Keys);

            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                player.Value.TurnFinished(playerList[currentTurn]);
            }
        }

        void NotifyEndGame(int partyCode)
        {
            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                player.Value.GameOver();
            }
        }

        void NotifyEndRound(int partyCode)
        {
            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                player.Value.RoundOver();
            }
        }
    }

    public partial class ServiceImplementation : ICardManager
    {
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
        static ConcurrentDictionary<int, Card[]> _gameCards = new ConcurrentDictionary<int, Card[]>();
        static Random _numberGenerator = new Random();

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

        public void PlayCard(int gameId, int position)
        {
            _gameCards[gameId][position].Number = "";

            NotifyPlayedCard(gameId);
        }

        public void NotifyPlayedCard(int gameId)
        {
            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[gameId])
            {
                player.Value.UpdateTableCards();
            }
        }
    }
}
