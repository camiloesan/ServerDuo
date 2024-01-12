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
using System.Configuration;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Data.Common;

namespace CommunicationService
{
    public partial class ServiceImplementation : IUsersManager
    {
        public int AddUserToDatabase(UserDTO user)
        {
            using (var databaseContext = new DuoContext())
            {
                User databaseUser = new User
                {
                    Username = user.UserName,
                    Email = user.Email,
                    TotalWins = 0,
                    PictureID = 0,
                    Password = user.Password
                };

                int result = 0;
                try
                {
                    databaseContext.Users.Add(databaseUser);
                    result = databaseContext.SaveChanges();
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return result;
            }
        }
        
        public bool IsEmailTaken(string email)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = true;
                try
                {
                    result = databaseContext.Users.Any(user => user.Email == email);
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return result;
            }
        }

        public UserDTO IsLoginValid(string username, string password)
        {
            using (var databaseContext = new DuoContext())
            {
                User databaseUser = new User();
                try
                {
                    databaseUser = databaseContext.Users
                        .FirstOrDefault(user => user.Username == username && user.Password == password);
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                UserDTO resultUser = new UserDTO();
                if (databaseUser != null)
                {
                    resultUser = new UserDTO
                    {
                        ID = databaseUser.UserID,
                        UserName = databaseUser.Username,
                        TotalWins = (int)databaseUser.TotalWins,
                        Email = databaseUser.Email,
                        PictureID = (int)databaseUser.PictureID
                    };
                }
                return resultUser;
            }
        }

        public bool IsUsernameTaken(string username)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = true;
                try
                {
                    result = databaseContext.Users.Any(user => username == user.Username);
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return result;
            }
        }
       
        public bool DeleteUserFromDatabaseByUsername(string username)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = true;
                try
                {
                    User userEntity = databaseContext.Users.First(user => user.Username == username);
                    databaseContext.Users.Remove(userEntity);
                    databaseContext.SaveChanges();
                }
                catch (EntityException ex)
                {
                    result = false;
                    log.Error(ex);
                }
                catch (SqlException ex)
                {
                    result = false;
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    result = false;
                    log.Error(ex);
                }

                return result;
            }
        }

        public bool IsUserAlreadyLoggedIn(string username)
        {
            return _onlineUsers.ContainsKey(username);
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
            string encryptedPassword = ConfigurationManager.AppSettings["EncryptedPassword"];
            string key = ConfigurationManager.AppSettings["Key"];
            string password = CryptoService.DecryptString(key, encryptedPassword);

            try
            {
                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(username, password);

                    MailMessage mailMessage = new MailMessage(from, to, subject, body);
                    smtpClient.Send(mailMessage);
                }
            }
            catch (SmtpException ex)
            {
                log.Error(ex);
                confirmationCode = -1;
            }

            return confirmationCode;
        }

        public int ModifyPasswordByEmail(string email, string newPassword)
        {
            using (var databaseContext = new DuoContext())
            {
                int result = 0;
                try
                {
                    User userToModify = databaseContext.Users
                        .First(user => user.Email == email);
                    userToModify.Password = newPassword;
                    result = databaseContext.SaveChanges();
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbUpdateException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return result;
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
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbException ex)
                {
                    log.Error(ex);
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

        public UserDTO GetUserInfoByUsername(string username)
        {
            using (var databaseContext = new DuoContext())
            {
                User databaseUser = null;
                UserDTO resultUser = null;
                try
                {
                    databaseUser = databaseContext.Users
                        .FirstOrDefault(user => user.Username == username);
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                if (databaseUser != null)
                {
                    resultUser = new UserDTO
                    {
                        ID = databaseUser.UserID,
                        UserName = databaseUser.Username,
                        TotalWins = (int)databaseUser.TotalWins,
                        PictureID = (int)databaseUser.PictureID
                    };
                }
                return resultUser;
            }
        }

        public int UpdateProfilePictureByUserId(int userId, int pictureId)
        {
            using (var databaseContext = new DuoContext())
            {
                int result = 0;
                try
                {
                    User userToModify = databaseContext.Users
                        .First(user => user.UserID == userId);
                    userToModify.PictureID = pictureId;
                    result = databaseContext.SaveChanges();
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                return result;
            }
        }

        public bool IsUserBanned(int userId)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = false;
                try
                {
                    result = databaseContext.BannedUsers
                        .Any(userBanned => userBanned.UserBannedID == userId);
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return result;
            }
        }
    }

    public partial class ServiceImplementation : IFriendsManager
    {
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
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (SqlException ex)
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
                        SenderUsername = friendRequestItem.User1.Username,
                        ReceiverID = (int)friendRequestItem.ReceiverID,
                        ReceiverUsername = friendRequestItem.User.Username
                    };
                    resultList.Add(friendRequest);
                }
                return resultList;
            }
        }

        public int SendFriendRequest(string usernameSender, string usernameReceiver)
        {
            using (var databaseContext = new DuoContext())
            {
                int senderId = 0;
                int receiverId = 0;
                try
                {
                    senderId = databaseContext.Users.First(user => user.Username == usernameSender).UserID;
                    receiverId = databaseContext.Users.First(user => user.Username == usernameReceiver).UserID;
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }


                int result = 0;
                if (senderId != 0 && receiverId != 0)
                {
                    FriendRequest friendRequest = new FriendRequest
                    {
                        SenderID = senderId,
                        ReceiverID = receiverId,
                    };

                    try
                    {
                        databaseContext.FriendRequests.Add(friendRequest);
                        result = databaseContext.SaveChanges();
                    }
                    catch (EntityException ex)
                    {
                        log.Error(ex);
                    }
                    catch (SqlException ex)
                    {
                        log.Error(ex);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                return result;
            }
        }

        public bool AcceptFriendRequest(FriendRequestDTO friendRequest)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = true;
                Friendship friendship = new Friendship
                {
                    SenderID = friendRequest.SenderID,
                    ReceiverID = friendRequest.ReceiverID,
                };

                try
                {
                    databaseContext.Friendships.Add(friendship);
                    FriendRequest friendRequestToDelete
                        = databaseContext.FriendRequests.Find(friendRequest.FriendRequestID);
                    databaseContext.FriendRequests.Remove(friendRequestToDelete);
                    databaseContext.SaveChanges();
                }
                catch (EntityException ex)
                {
                    result = false;
                    log.Error(ex);
                }
                catch (DbUpdateException ex)
                {
                    result = false;
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    result = false;
                    log.Error(ex);
                }
                return result;
            }
        }

        public bool RejectFriendRequest(int friendRequestId)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = true;
                try
                {
                    FriendRequest requestToDelete = databaseContext.FriendRequests.Find(friendRequestId);
                    databaseContext.FriendRequests.Remove(requestToDelete);
                    databaseContext.SaveChanges();
                }
                catch (EntityException ex)
                {
                    result = false;
                    log.Error(ex);
                }
                catch (DbUpdateException ex)
                {
                    result = false;
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    result = false;
                    log.Error(ex);
                }
                return result;
            }
        }

        public List<FriendshipDTO> GetOnlineFriends(int userId)
        {
            List<FriendshipDTO> friendList = GetFriendsList(userId);
            List<FriendshipDTO> onlineFriends = new List<FriendshipDTO>();

            foreach (var friend in friendList)
            {
                if (friend.Friend1ID != userId && _onlineUsers.ContainsKey(friend.Friend1Username))
                {
                    onlineFriends.Add(friend);
                }
                else if (friend.Friend2ID != userId && _onlineUsers.ContainsKey(friend.Friend2Username))
                {
                    onlineFriends.Add(friend);
                }
            }
            return onlineFriends;
        }

        public List<FriendshipDTO> GetFriendsList(int userId)
        {
            using (var databaseContext = new DuoContext())
            {
                List<Friendship> friendshipsList = new List<Friendship>();
                try
                {
                    friendshipsList = databaseContext.Friendships
                        .Where(friendship => friendship.SenderID == userId || friendship.ReceiverID == userId)
                        .ToList();
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

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

        public bool DeleteFriendshipById(int friendshipId)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = true;
                try
                {
                    Friendship friendshipEntity = databaseContext
                    .Friendships.FirstOrDefault(friendship => friendship.FriendshipID == friendshipId);
                    databaseContext.Friendships.Remove(friendshipEntity);
                    databaseContext.SaveChanges();
                }
                catch (EntityException ex)
                {
                    result = false;
                    log.Error(ex);
                }
                catch (SqlException ex)
                {
                    result = false;
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    result = false;
                    log.Error(ex);
                }

                return result;
            }
        }

        public bool IsFriendRequestAlreadyExistent(string usernameSender, string usernameReceiver)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = false;
                int senderId = 0;
                int receiverId = 0;
                try
                {
                    senderId = databaseContext.Users.First(user => user.Username == usernameSender).UserID;
                    receiverId = databaseContext.Users.First(user => user.Username == usernameReceiver).UserID;
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                if (senderId != 0 && receiverId != 0)
                {
                    try
                    {
                        result = databaseContext.FriendRequests
                            .Any(fet => (fet.SenderID == senderId && fet.ReceiverID == receiverId)
                                || (fet.SenderID == receiverId && fet.ReceiverID == senderId));
                    }
                    catch (EntityException ex)
                    {
                        log.Error(ex);
                    }
                    catch (DbException ex)
                    {
                        log.Error(ex);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                return result;
            }
        }

        public bool IsAlreadyFriend(string senderUsername, string receiverUsername)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = false;
                User userSender = new User();
                userSender.UserID = 0;
                User userReceiver = new User();
                userReceiver.UserID = 0;
                try
                {
                    userSender = databaseContext.Users
                        .First(user => user.Username == senderUsername);
                    userReceiver = databaseContext.Users
                        .First(user => user.Username == receiverUsername);
                    result = databaseContext.Friendships
                            .Any(friendship =>
                            (friendship.SenderID == userSender.UserID && friendship.ReceiverID == userReceiver.UserID)
                            || (friendship.SenderID == userReceiver.UserID && friendship.ReceiverID == userSender.UserID));
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return result;
            }
        }
    }

    public partial class ServiceImplementation : IBlockManager
    {
        public bool IsUserBlockedByUsername(string usernameBlocker, string usernameBlocked)
        {
            using (var databaseContext = new DuoContext())
            {
                bool result = false;
                try
                {
                    UserBlock userBlocks = new UserBlock
                    {
                        BlockerID = databaseContext.Users.First(user => user.Username == usernameBlocker).UserID,
                        BlockedID = databaseContext.Users.First(user => user.Username == usernameBlocked).UserID,
                    };

                    result = databaseContext.UserBlocks
                        .Any(block => block.BlockerID == userBlocks.BlockerID
                            && block.BlockedID == userBlocks.BlockedID);
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return result;
            }
        }

        public int BlockUserByUsername(string blockerUsername, string blockedUsername)
        {
            using (var databaseContext = new DuoContext())
            {
                int result = 0;
                try
                {
                    UserBlock userBlocks = new UserBlock
                    {
                        BlockerID = databaseContext.Users.First(user => user.Username == blockerUsername).UserID,
                        BlockedID = databaseContext.Users.First(user => user.Username == blockedUsername).UserID,
                    };
                    databaseContext.UserBlocks.Add(userBlocks);

                    if (databaseContext.UserBlocks.Count(blockedUser => blockedUser.BlockedID == userBlocks.BlockedID) >= 2)
                    {
                        BannedUser bannedUser = new BannedUser();
                        bannedUser.UserBannedID = userBlocks.BlockedID;
                        databaseContext.BannedUsers.Add(bannedUser);
                    }

                    result = databaseContext.SaveChanges();
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbUpdateException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return result;
            }
        }

        public int UnblockUserByBlockId(int blockId)
        {
            using (var databaseContext = new DuoContext())
            {
                int result = 0;
                try
                {
                    UserBlock userBlockedDatabase = databaseContext.UserBlocks
                        .First(block => block.UserBlockID == blockId);
                    databaseContext.UserBlocks.Remove(userBlockedDatabase);
                    result = databaseContext.SaveChanges();
                }
                catch (EntityException ex)
                {
                    log.Error(ex);
                }
                catch (DbUpdateException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                return result;
            }
        }

        public List<UserBlockedDTO> GetBlockedUsersListByUserId(int userId)
        {
            using (var databaseContext = new DuoContext())
            {
                List<UserBlockedDTO> resultList = new List<UserBlockedDTO>();

                List<UserBlock> blockedUsersList = new List<UserBlock>();

                try
                {
                    blockedUsersList = databaseContext.UserBlocks
                        .Where(blocks => blocks.BlockerID == userId)
                        .ToList();
                }
                catch (EntityException ex)
                {
                    log.Error(ex.Message);
                }
                catch (DbException ex)
                {
                    log.Error(ex);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

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
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ServiceImplementation : ILobbyManager
    {
        static ConcurrentDictionary<int, ConcurrentDictionary<string, ILobbyManagerCallback>> _activeLobbiesDictionary = new ConcurrentDictionary<int, ConcurrentDictionary<string, ILobbyManagerCallback>>();

        private void DisconnectUser(int lobbyCode, string username)
        {
            UserDTO user = GetUserInfoByUsername(username);
            if (username.Contains("guest"))
            {
                NotifyKickPlayer(lobbyCode, username, "timeout");
            }
            else if (_onlineUsers[username])
            {
                NotifyCloseLobby(lobbyCode, username, "host has left");
                _onlineUsers.TryRemove(user.UserName, out _);
            }
            else 
            {
                NotifyKickPlayer(lobbyCode, username, "timeout");
                _onlineUsers.TryRemove(user.UserName, out _);
            }
        }

        public void NotifyCreateLobby(int lobbyCode, string hostUsername)
        {
            var callback = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();

            var lobbyContextsDictionary = new ConcurrentDictionary<string, ILobbyManagerCallback>();
            lobbyContextsDictionary.TryAdd(hostUsername, callback);
            _activeLobbiesDictionary.TryAdd(lobbyCode, lobbyContextsDictionary);
            _onlineUsers[hostUsername] = true;
            try
            {
                callback.LobbyCreated(lobbyContextsDictionary);
            }
            catch (CommunicationException)
            {
                DisconnectUser(lobbyCode, hostUsername);
            }
            catch (TimeoutException)
            {
                DisconnectUser(lobbyCode, hostUsername);
            }
        }

        public void NotifyJoinLobby(int lobbyCode, string username)
        {
            if (_activeLobbiesDictionary.ContainsKey(lobbyCode))
            {
                var callback = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
                _activeLobbiesDictionary[lobbyCode].TryAdd(username, callback);
                foreach (var player in _activeLobbiesDictionary[lobbyCode])
                {
                    try
                    {                    
                        player.Value.PlayerJoined(_activeLobbiesDictionary[lobbyCode]);
                    }
                    catch (CommunicationException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                    catch (TimeoutException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                }
            }
        }

        public void NotifyLeaveLobby(int lobbyCode, string username)
        {
            if (_activeLobbiesDictionary.ContainsKey(lobbyCode))
            {
                _activeLobbiesDictionary[lobbyCode].TryRemove(username, out _);
                foreach (var player in _activeLobbiesDictionary[lobbyCode])
                {
                    try
                    {
                        player.Value.PlayerLeft(_activeLobbiesDictionary[lobbyCode]);
                    }
                    catch (CommunicationException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                    catch (TimeoutException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                }
            }
        }

        public void NotifyCloseLobby(int lobbyCode, string hostName, string reason)
        {
            if (_activeLobbiesDictionary.ContainsKey(lobbyCode)) {
                _activeLobbiesDictionary[lobbyCode].TryRemove(hostName, out _);
                foreach (var player in _activeLobbiesDictionary[lobbyCode])
                {
                    try
                    {
                        player.Value.PlayerKicked(reason);
                    }
                    catch (CommunicationException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                    catch (TimeoutException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                }
                _activeLobbiesDictionary.TryRemove(lobbyCode, out _);
            }
        }

        public void NotifySendMessage(int lobbyCode, string message)
        {
            if (_activeLobbiesDictionary.ContainsKey(lobbyCode))
            {
                foreach (var player in _activeLobbiesDictionary[lobbyCode])
                {
                    try
                    {
                        player.Value.MessageReceived(message);
                    }
                    catch (CommunicationException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                    catch (TimeoutException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                }
            }

        }

        public void NotifyKickPlayer(int lobbyCode, string username, string reason)
        {
            if (_activeLobbiesDictionary.ContainsKey(lobbyCode))
            {
                try
                {
                    _activeLobbiesDictionary[lobbyCode][username].PlayerKicked(reason);
                }
                catch (CommunicationException ex)
                {
                    log.Error(ex);
                }
                catch (TimeoutException ex)
                {
                    log.Error(ex);
                }

                _activeLobbiesDictionary[lobbyCode].TryRemove(username, out _);

                foreach (var player in _activeLobbiesDictionary[lobbyCode])
                {
                    try
                    {
                        player.Value.PlayerLeft(_activeLobbiesDictionary[lobbyCode]);
                    }
                    catch (CommunicationException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                    catch (TimeoutException)
                    {
                        DisconnectUser(lobbyCode, player.Key);
                    }
                }
            }
        }

        public async void NotifyStartGame(int lobbyCode)
        {
            if (_activeLobbiesDictionary.ContainsKey(lobbyCode))
            {
                _gameCards.TryAdd(lobbyCode, new Card[3]);

                for (int i = 0; i < _gameCards[lobbyCode].Length; i++)
                {
                    _gameCards[lobbyCode][i] = new Card();
                    _gameCards[lobbyCode][i].Number = "";
                }

                DealCards(lobbyCode);

                foreach (var player in _activeLobbiesDictionary[lobbyCode])
                {
                    player.Value.GameStarted();
                }

                await Task.Delay(5000);
                _activeLobbiesDictionary.TryRemove(lobbyCode, out _);
            }
        }
    }

    public partial class ServiceImplementation : IUserConnectionHandler
    {
        static readonly ConcurrentDictionary<string, bool> _onlineUsers = new ConcurrentDictionary<string, bool>();

        public void NotifyLogIn(UserDTO user)
        {
            _onlineUsers.TryAdd(user.UserName, false);
        }

        public void NotifyLogOut(UserDTO user, bool isHost)
        {
            if (isHost)
            {
                NotifyCloseLobby(user.PartyCode, user.UserName, "");
            }
            else if (user.PartyCode != 0)
            {
                NotifyLeaveLobby(user.PartyCode, user.UserName);
            }
            _onlineUsers.TryRemove(user.UserName, out _);
        }

        public void NotifyGuestLeft(int partyCode, string username)
        {
            NotifyLeaveLobby(partyCode, username);
        }

        public void LogOut(string username)
        {
            _onlineUsers.TryRemove(username, out _);
        }
    }

    public partial class ServiceImplementation : ILobbyValidator
    {
        public bool IsLobbyExistent(int lobbyCode)
        {
            return _activeLobbiesDictionary.ContainsKey(lobbyCode);
        }

        public bool IsSpaceAvailable(int lobbyCode)
        {
            return _activeLobbiesDictionary[lobbyCode].Count < 4;
        }

        public bool IsUsernameInLobby(int lobbyCode, string username)
        {
            return _activeLobbiesDictionary[lobbyCode].ContainsKey(username);
        }

        public List<string> GetPlayersInLobby(int lobbyCode)
        {
            return _activeLobbiesDictionary[lobbyCode].Keys.ToList();
        }
    }

    public partial class ServiceImplementation : IMatchManager
    {
        private static ConcurrentDictionary<int, ConcurrentDictionary<string, int>> _matchResults = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();
        private static ConcurrentDictionary<int, ConcurrentDictionary<string, IMatchManagerCallback>> _playerCallbacks = new ConcurrentDictionary<int, ConcurrentDictionary<string, IMatchManagerCallback>>();
        private static ConcurrentDictionary<int, int> _currentTurn = new ConcurrentDictionary<int, int>();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Add user callback to the service so they can be called back
        /// </summary>
        /// <param name="partyCode">Code of the match</param>
        /// <param name="username">Name of the user subscribing to the service</param>
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

        public ConcurrentDictionary<string, int> GetMatchResults(int partyCode)
        {
            return _matchResults[partyCode];
        }

        private void NotifyEndTurn(int partyCode)
        {
            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                try
                {
                    player.Value.TurnFinished(GetCurrentTurn(partyCode));
                }
                catch (CommunicationException)
                {
                    _playerCallbacks[partyCode].TryRemove(player.Key, out _);
                    _onlineUsers.TryRemove(player.Key, out _);
                    NotifyPlayerQuit(partyCode, player.Key, "Communication error");
                }
                catch (TimeoutException)
                {
                    _playerCallbacks[partyCode].TryRemove(player.Key, out _);
                    _onlineUsers.TryRemove(player.Key, out _);
                    NotifyPlayerQuit(partyCode, player.Key, "Timeout");
                }
            }
        }

        private async Task NotifyEndGame(int partyCode)
        {
            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                try
                {
                    player.Value.GameOver();
                }
                catch (CommunicationException)
                {
                    _playerCallbacks[partyCode].TryRemove(player.Key, out _);
                    _onlineUsers.TryRemove(player.Key, out _);
                }
                catch (TimeoutException)
                {
                    _playerCallbacks[partyCode].TryRemove(player.Key, out _);
                    _onlineUsers.TryRemove(player.Key, out _);
                }
            }

            SaveMatchResult(partyCode);

            await Task.Delay(10000);
            //Match data will be deleted so players start out with fresh data every match
            _gameCards.TryRemove(partyCode, out _);
            _currentTurn.TryRemove(partyCode, out _);
            _playerCallbacks.TryRemove(partyCode, out _);
            _matchResults.TryRemove(partyCode, out _);
        }

        private void SaveMatchResult(int partyCode)
        {
            if (_matchResults.ContainsKey(partyCode))
            {
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

        public void DealCards(int partyCode)
        {
            if (!_gameCards.ContainsKey(partyCode))
            {
                _gameCards.TryAdd(partyCode, new Card[3]);

                for (int i = 0; i < _gameCards[partyCode].Length; i++)
                {
                    _gameCards[partyCode][i] = new Card();
                    _gameCards[partyCode][i].Number = "";
                }
            }

            for (int i = 0; i < _gameCards[partyCode].Length - 1; i++)
            {
                if (_gameCards[partyCode][i].Number.Equals(""))
                {
                    do
                    {
                        _gameCards[partyCode][i].Number = _numberGenerator.Next(1, 11).ToString();
                    } while (_gameCards[partyCode][i].Number.Equals("2"));

                    _gameCards[partyCode][i].Color = _cardColors[_numberGenerator.Next(0, 4)];
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

        public Card[] GetCards(int partyCode)
        {
            return _gameCards[partyCode];
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
    public partial class ServiceImplementation : IMatchPlayerManager
    {
        public List<string> GetPlayerList(int partyCode)
        {
            return new List<string>(_playerCallbacks[partyCode].Keys);
        }

        public void ExitMatch(int partyCode, string username)
        {
            if (_playerCallbacks[partyCode].ContainsKey(username))
            {
                _playerCallbacks[partyCode].TryRemove(username, out _);
            }

            NotifyPlayerQuit(partyCode, username, "User clicked exit button");
        }

        public void KickPlayerFromGame(int partyCode, string username, string reason)
        {
            NotifyPlayerQuit(partyCode, username, reason);
        }

        private async void NotifyPlayerQuit(int partyCode, string username, string reason)
        {
            foreach (KeyValuePair<string, IMatchManagerCallback> player in _playerCallbacks[partyCode])
            {
                try
                {
                    player.Value.PlayerLeftGame(username, reason);
                }
                catch (CommunicationException)
                {
                    _playerCallbacks[partyCode].TryRemove(player.Key, out _);
                    _onlineUsers.TryRemove(player.Key, out _);
                }
                catch (TimeoutException)
                {
                    _playerCallbacks[partyCode].TryRemove(player.Key, out _);
                    _onlineUsers.TryRemove(player.Key, out _);
                }
            }

            if (_playerCallbacks[partyCode].ContainsKey(username))
            {
                _playerCallbacks[partyCode].TryRemove(username, out _);
            }

            if (_playerCallbacks[partyCode].Count > 1)
            {
                if (GetCurrentTurn(partyCode).Equals(username))
                {
                    EndTurn(partyCode);
                }
            }
            else
            {
                EndGame(partyCode);
            }
        }
    }
}
