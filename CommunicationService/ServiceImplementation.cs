using Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace CommunicationService
{
    public partial class ServiceImplementation : IUsersManager
    {
        public bool AddUserToDatabase(UserDTO user)
        {
            using (var databaseContext = new DuoContext())
            {
                var databaseUser = new User
                {
                    Username = user.UserName,
                    Email = user.Email,
                    TotalWins = 0,
                    Password = user.Password
                };

                try
                {
                    databaseContext.Users.Add(databaseUser);
                    databaseContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    return false;
                }
                return true;
            }
        }

        public List<FriendRequestDTO> GetFriendRequestsList(int userId)
        {
            using (var databaseContext = new DuoContext())
            {
                List<FriendRequest> friendRequests = new List<FriendRequest>();
                try
                {
                    friendRequests = databaseContext.FriendRequests
                        .Where(friendRequest => friendRequest.ReceiverID == userId)
                        .ToList();
                } 
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                List<FriendRequestDTO> resultList = new List<FriendRequestDTO>();
                foreach (var friendRequestItem in friendRequests)
                {
                    FriendRequestDTO friendRequest = new FriendRequestDTO
                    {
                        FriendRequestID = friendRequestItem.RequestID,
                        SenderID = (int)friendRequestItem.SenderID,
                        SenderUsername = friendRequestItem.User1.Username, //could be upside down
                        ReceiverID = (int)friendRequestItem.ReceiverID,
                        ReceiverUsername = friendRequestItem.User.Username
                    };
                    resultList.Add(friendRequest);
                }
                return resultList;
            }
        }

        public List<string> GetOnlineFriends(string username)
        {
            //TODO
            return null;
        }

        public bool IsEmailTaken(string email)
        {
            using (var databaseContext = new DuoContext())
            {
                return databaseContext.Users.Any(user => user.Email == email);
            }
        }

        public UserDTO IsLoginValid(string username, string password)
        {
            using (var databaseContext = new DuoContext())
            {
                var databaseUser = new User();
                try
                {
                    databaseUser = databaseContext.Users
                        .First(user => user.Username == username && user.Password == password);
                }
                catch
                {
                    return null;
                }

                var resultUser = new UserDTO
                {
                    ID = databaseUser.UserID,
                    UserName = databaseUser.Username,
                    TotalWins = (int)databaseUser.TotalWins,
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

                var friendRequest = new FriendRequest
                {
                    SenderID = senderId,
                    ReceiverID = receiverId,
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

        public bool AcceptFriendRequest(FriendRequestDTO friendRequest)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendship = new Friendship
                {
                    SenderID = friendRequest.SenderID,
                    ReceiverID = friendRequest.ReceiverID,
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

        public List<FriendshipDTO> GetFriendsList(int userId)
        {
            using (var databaseContext = new DuoContext())
            {
                var friendshipsList = databaseContext.Friendships
                    .Where(friendship => friendship.SenderID == userId || friendship.ReceiverID == userId)
                    .ToList();

                List<FriendshipDTO> resultList = new List<FriendshipDTO>();
                foreach (var friendshipItem in friendshipsList)
                {
                    FriendshipDTO friendship = new FriendshipDTO()
                    {
                        FriendshipID = friendshipItem.FriendshipID,
                        Friend1ID = (int)friendshipItem.SenderID,
                        Friend1Username = friendshipItem.User.Username,
                        Friend2ID = (int)friendshipItem.ReceiverID,
                        Friend2Username = friendshipItem.User1.Username
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
                    (friendRequest.SenderID == senderId && friendRequest.ReceiverID == receiverId)
                    || (friendRequest.SenderID == receiverId && friendRequest.ReceiverID == senderId));
                return friendRequestEntity != null;
            }
        }

        public bool IsAlreadyFriend(string senderUsername, string receiverUsername)
        {
            using (var databaseContext = new DuoContext())
            {
                User userSender;
                User userReceiver;
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
                    (friendship.SenderID == userSender.UserID && friendship.ReceiverID == userReceiver.UserID)
                    || (friendship.SenderID == userReceiver.UserID && friendship.ReceiverID == userSender.UserID));
                return friendshipEntity != null;
            }
        }

        public bool IsUserAlreadyLoggedIn(int userId)
        {
            return _onlineUsers.ContainsKey(userId);
        }

        public int SendConfirmationCode(string email, string lang)
        {
            Random randomCode = new Random();
            int confirmationCode = randomCode.Next(1000, 10000);
            string to = email;
            string subject;
            string body;

            switch (lang)
            {
                case "es":
                    subject = "Petición de reestablecimiento de contraseña";
                    body = "Hemos recivido una petición para cambiar la contraseña, si lo hiciste, aquí está el código que necesitas ingresar: \n\n" + confirmationCode;
                    break;
                case "en":
                    subject = "Password reset request";
                    body = "We have received a request to change your password, if you did it, here's the code you need to enter: \n\n" + confirmationCode;
                    break;
                case "fr":
                    subject = "Demande de réinitialisation du mot de passe";
                    body = "Nous avons reçu une demande de changement de mot de passe, si vous l'avez fait, voici le code que vous devez saisir: \n\n" + confirmationCode;
                    break;
                default:
                    subject = "Password reset request";
                    body = "We have received a request to change your password, if you did it, here's the code you need to enter: \n\n" + confirmationCode;
                    break;
            }

            string from = "duogamefei@gmail.com";
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string username = "duogamefei@gmail.com";
            string password = "rfis qkfp zmub hcft";

            try
            {
                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(username, password);

                    MailMessage mailMessage = new MailMessage(from, to, subject, body);
                    client.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return -1;
            }

            return confirmationCode;
        }

        public bool IsUserBlockedByUsername(string usernameBlocker, string usernameBlocked)
        {
            using (var databaseContext = new DuoContext())
            {
                try
                {
                    UserBlock userBlocks = new UserBlock
                    {
                        BlockerID = databaseContext.Users.First(user => user.Username == usernameBlocker).UserID,
                        BlockedID = databaseContext.Users.First(user => user.Username == usernameBlocked).UserID,
                    };

                    var userBlockEntity = databaseContext.UserBlocks
                        .First(block => block.BlockerID == userBlocks.BlockerID 
                        && block.BlockedID == userBlocks.BlockedID);
                }
                catch
                {
                    //log
                    return false;
                }
            }
            return true;
        }

        public bool ModifyPasswordByEmail(string email, string newPassword) //not secure ?
        {
            using (var databaseContext = new DuoContext())
            {
                try
                {
                    User userToModify = databaseContext.Users
                        .First(user => user.Email == email);
                    userToModify.Password = newPassword;
                    databaseContext.SaveChanges();
                }
                catch
                {
                    //log
                    return false;
                }
            }
            return true;
        }

        public bool BlockUserByUsername(string blockerUsername, string blockedUsername)
        {
            using (var databaseContext = new DuoContext())
            {
                try
                {
                    UserBlock userBlocks = new UserBlock
                    {
                        BlockerID = databaseContext.Users.First(user => user.Username == blockerUsername).UserID,
                        BlockedID = databaseContext.Users.First(user => user.Username == blockedUsername).UserID,
                    };

                    databaseContext.UserBlocks.Add(userBlocks);
                    databaseContext.SaveChanges();
                }
                catch
                {
                    //log
                    return false;
                }

                return true;
            }
        }

        public bool UnblockUserByBlockId(int blockId)
        {
            using (var databaseContext = new DuoContext())
            {
                try
                {
                    UserBlock userBlockedDatabase = databaseContext.UserBlocks
                        .First(block => block.UserBlockID == blockId);
                    databaseContext.UserBlocks.Remove(userBlockedDatabase);
                    databaseContext.SaveChanges();
                }
                catch
                {
                    //log
                    return false;
                }

                return true;
            }
        }

        public List<UserBlockedDTO> GetBlockedUsersListByUserId(int userId)
        {
            using (var databaseContext = new DuoContext())
            {
                List<UserBlockedDTO> resultList = new List<UserBlockedDTO>();

                var blockedUsersList = databaseContext.UserBlocks
                    .Where(blocks => blocks.BlockerID == userId)
                    .ToList();
                
                foreach (var blockedUserItem in blockedUsersList)
                {
                    UserBlockedDTO userBlocked = new UserBlockedDTO
                    {
                        BlockID = blockedUserItem.UserBlockID,
                        BlockedID = (int)blockedUserItem.BlockedID,
                        BlockedUsername = blockedUserItem.User1.Username
                    };

                    resultList.Add(userBlocked);
                }
                return resultList;
            }
        }

        public List<UserDTO> GetTopTenWinners()
        {
            using (var databaseContext = new DuoContext())
            {
                List<UserDTO> resultList = new List<UserDTO>();

                List<User> topTenWinners = new List<User>();
                try
                {
                    topTenWinners = databaseContext.Users
                        .OrderByDescending(user => user.TotalWins)
                        .Take(10)
                        .ToList();
                } 
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                
                foreach (var user in topTenWinners)
                {
                    UserDTO userDTO = new UserDTO
                    {
                        UserName = user.Username,
                        TotalWins = (int)user.TotalWins
                    };

                    resultList.Add(userDTO);
                }

                return resultList;
            }
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

        public void NotifyLogIn(UserDTO user)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IUserConnectionHandlerCallback>();
            _onlineUsers.TryAdd(user.ID, callback);

            using (var databaseContext = new DuoContext())
            {
                var friendshipsList = databaseContext.Friendships
                    .Where(friendship => friendship.ReceiverID == user.ID || friendship.SenderID == user.ID)
                    .ToList();
                foreach (var friend in friendshipsList)
                {
                    if (friend.SenderID == user.ID)
                    {
                        if (_onlineUsers.ContainsKey((int)friend.ReceiverID))
                        {
                            _onlineUsers[friend.User1.UserID].UserLogged(friend.User.Username);
                        }
                    }
                    else if (friend.ReceiverID == user.ID)
                    {
                        if (_onlineUsers.ContainsKey((int)friend.SenderID))
                        {
                            _onlineUsers[friend.User.UserID].UserLogged(friend.User1.Username);
                        }
                    }
                }
            }
        }

        public void NotifyLogOut(UserDTO user)
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

        public List<string> GetPlayersInParty(int partyCode)
        {
            return _activePartiesDictionary[partyCode].Keys.ToList();
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

        public void ExitMatch(int partyCode, string username)
        {
            List<string> playerList = GetPlayerList(partyCode);
            string currentPlayerTurn = playerList[_currentTurn[partyCode]];

            if (currentPlayerTurn.Equals(username))
            {
                EndTurn(partyCode);
            }

            _playerCallbacks[partyCode].TryRemove(username, out _);

            NotifyPlayerQuit(partyCode, username, "User clicked exit button");
        }

        public void KickPlayerFromGame(int partyCode, string username, string reason)
        {
            NotifyPlayerQuit(partyCode, username, reason);
        }

        public async void EndGame(int partyCode)
        {
            await NotifyEndGame(partyCode);
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
                NotifyPlayerQuit(partyCode, currentCallback, "Timeout");
            }
            
        }

        private async Task NotifyEndGame(int partyCode)
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
                NotifyPlayerQuit(partyCode, currentCallback, "Timeout");
            }

            
            using (DuoContext databaseContext = new DuoContext())
            {
                string winner = _matchResults[partyCode].OrderBy(x => x.Value).First().Key;
                int winnerUserId = databaseContext.Users.Where(u => u.Username == winner).Select(u => u.UserID).FirstOrDefault();

                if (winnerUserId > 0)
                {
                    try
                    {
                        User user = databaseContext.Users.Where(u => u.UserID == winnerUserId).FirstOrDefault();

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
            //Match data will be deleted so players start out with fresh data every match
            _gameCards.TryRemove(partyCode, out _);
            _currentTurn.TryRemove(partyCode, out _);
            _playerCallbacks.TryRemove(partyCode, out _);
            _matchResults.TryRemove(partyCode, out _);
        }

        private async void NotifyPlayerQuit(int partyCode, string username, string reason)
        {
            List<string> playerList = GetPlayerList(partyCode);
            string currentPlayerTurn = playerList[_currentTurn[partyCode]];

            if (currentPlayerTurn.Equals(username))
            {
                EndTurn(partyCode);
            }

            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                try
                {
                    player.Value.PlayerLeftGame(username, reason);
                }
                catch(TimeoutException ex)
                {
                    log.Error("Timeout error while kicking a player from a match", ex);
                }
            }

            if (_playerCallbacks[partyCode].ContainsKey(username))
            {
                _playerCallbacks[partyCode].TryRemove(username, out _);
            }

            if (_playerCallbacks[partyCode].Count <= 1)
            {
                EndGame(partyCode);
            }
        }
    }

    public partial class ServiceImplementation : ICardManager
    {
        //Lists are for random card generation using weights
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
