using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ClienteDuo.DataService;

namespace ClienteDuo.Utilities.Tests
{
    [TestClass()]
    public class UsersManagerTests
    {
        readonly string _user1 = "marcial";
        readonly string _user2 = "casemiro";
        readonly string _email1 = "marcial@gmail.com";
        readonly string _email2 = "casemiro@gmail.com";
        readonly string _password1 = "liranroll!33";
        readonly string _password2 = "panda!78990";

        [TestInitialize]
        public void Init()
        {
            UsersManager.AddUserToDatabase(_user1, _email1, _password1);
            UsersManager.AddUserToDatabase(_user2, _email2, _password2);
        }

        [TestCleanup]
        public void Cleanup()
        {
            UsersManager.DeleteUserFromDatabase(_user1);
            UsersManager.DeleteUserFromDatabase(_user2);
        }

        [TestMethod()]
        public void AcceptFriendRequestSuccessTest()
        {
            UsersManager.SendFriendRequest(_user1, _user2);
            UserDTO user2Object = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = UsersManager.GetFriendRequestsByUserId(user2Object.ID);

            bool result = UsersManager.AcceptFriendRequest(requestList.First());

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void AcceptFriendRequestNotExistsTest()
        {
            FriendRequestDTO friendRequestDTO = new FriendRequestDTO();
            friendRequestDTO.SenderID = 0;
            bool result = UsersManager.AcceptFriendRequest(friendRequestDTO);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void DeclineFriendRequestSuccessTest()
        {
            UsersManager.SendFriendRequest(_user1, _user2);
            UserDTO user2Object = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = UsersManager.GetFriendRequestsByUserId(user2Object.ID);

            bool result = UsersManager.DeclineFriendRequest(requestList.First());

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void DeclineFriendRequestNotExistsTest()
        {
            UsersManager.SendFriendRequest(_user1, _user2);

            FriendRequestDTO friendRequestDTO = new FriendRequestDTO();
            friendRequestDTO.SenderID = 0;
            bool result = UsersManager.DeclineFriendRequest(friendRequestDTO);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void GetFriendRequestsByUserIdHasTwoTest()
        {
            string user3 = "roberto";
            string email3 = "roberto@gmail.com";
            string password3 = "panda!78990";
            UsersManager.AddUserToDatabase(user3, email3, password3);

            UsersManager.SendFriendRequest(_user1, _user2);
            UsersManager.SendFriendRequest(user3, _user2);
            UserDTO user2Object = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = UsersManager.GetFriendRequestsByUserId(user2Object.ID);

            UsersManager.DeleteUserFromDatabase(user3);

            int expected = 2;

            Assert.AreEqual(expected, requestList.Count());
        }

        [TestMethod()]
        public void GetFriendRequestsByUserIdHasNoneTest()
        {
            UserDTO user2Object = UsersManager.GetUserInfoByUsername(_user1);
            var requestList = UsersManager.GetFriendRequestsByUserId(user2Object.ID);

            int expected = 0;
            Assert.AreEqual(expected, requestList.Count());
        }

        [TestMethod()]
        public void UnblockUserByBlockIdSuccessTest()
        {

            UsersManager.BlockUserByUsername(_user1, _user2);
            UserDTO user1Object = UsersManager.GetUserInfoByUsername(_user1);
            var blockList = UsersManager.GetBlockedUsersListByUserId(user1Object.ID);

            bool result = UsersManager.UnblockUserByBlockId(blockList.First().BlockID) == 1;
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void UnblockUserByBlockIdUserDoesNotExistTest()
        {
            bool result = UsersManager.UnblockUserByBlockId(0) == 1;

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void GetBlockedUsersListByIdHasOneTest()
        {
            UsersManager.BlockUserByUsername(_user1, _user2);
            UserDTO user1Object = UsersManager.GetUserInfoByUsername(_user1);
            var result = UsersManager.GetBlockedUsersListByUserId(user1Object.ID);

            int expected = 1;
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod()]
        public void IsUserBlockedOneWayTest()
        {
            UsersManager.BlockUserByUsername(_user1, _user2);
            bool result = UsersManager.IsUserBlocked(_user1, _user2);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsUserBlockedBothWaysTest()
        {
            UsersManager.BlockUserByUsername(_user1, _user2);
            bool result = UsersManager.IsUserBlocked(_user2, _user1);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUserBlockedDoesNotExistTest()
        {
            bool result = UsersManager.IsUserBlocked("joseAntonio", "janoRodriguez");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUserBlockedNotBlockedTest()
        {
            bool result = UsersManager.IsUserBlocked(_user2, _user1);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void GetUserInfoByUsernameNotNullTest()
        {
            var result = UsersManager.GetUserInfoByUsername(_user1);

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetUserInfoByUsernameDoesNotExistTest()
        {
            var result = UsersManager.GetUserInfoByUsername("persianaAmericana");

            Assert.IsNull(result);
        }

        [TestMethod()]
        public void IsAlreadyFriendLeftToRightTest()
        {
            UsersManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = UsersManager.GetFriendRequestsByUserId(userObject.ID);
            UsersManager.AcceptFriendRequest(requestList.First());
            bool result = UsersManager.IsAlreadyFriend(_user1, _user2);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsAlreadyFriendRightToLeftTest()
        {
            UsersManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = UsersManager.GetFriendRequestsByUserId(userObject.ID);
            UsersManager.AcceptFriendRequest(requestList.First());
            bool result = UsersManager.IsAlreadyFriend(_user2, _user1);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsAlreadyFriendNotFriendTest()
        {
            bool result = UsersManager.IsAlreadyFriend(_user2, _user1);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsAlreadyFriendDoesNotExist()
        {
            bool result = UsersManager.IsAlreadyFriend(_user2, "reikInolvidable");

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsFriendRequestAlreadySentTrueTest()
        {
            UsersManager.SendFriendRequest(_user1, _user2);

            bool result = UsersManager.IsFriendRequestAlreadySent(_user1, _user2);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsFriendRequestAlreadySentFalseTest()
        {
            bool result = UsersManager.IsFriendRequestAlreadySent(_user1, _user2);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsFriendRequestAlreadySentUserDoesNotExistTest()
        {
            bool result = UsersManager.IsFriendRequestAlreadySent(_user1, "cristinaRomo");

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void GetOnlineFriendsHasNoneTest()
        {
            UsersManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = UsersManager.GetFriendRequestsByUserId(userObject.ID);
            UsersManager.AcceptFriendRequest(requestList.First());

            var result = UsersManager.GetOnlineFriends(userObject.ID);

            int expected = 0;
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod()]
        public void GetFriendsListHasOneTest()
        {
            UsersManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = UsersManager.GetFriendRequestsByUserId(userObject.ID);
            UsersManager.AcceptFriendRequest(requestList.First());

            var result = UsersManager.GetFriendsListByUserId(userObject.ID);

            int expected = 1;
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod()]
        public void GetFriendsListHasZeroTest()
        {
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var result = UsersManager.GetFriendsListByUserId(userObject.ID);

            int expected = 0;
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod()]
        public void DeleteFriendshipByIdSuccessTest()
        {
            UsersManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = UsersManager.GetFriendRequestsByUserId(userObject.ID);
            UsersManager.AcceptFriendRequest(requestList.First());
            var friendsList = UsersManager.GetFriendsListByUserId(userObject.ID);
            bool result = UsersManager.DeleteFriendshipById(friendsList.First().FriendshipID);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void DeleteFriendshipByIdDoesNotExistTest()
        {
            bool result = UsersManager.DeleteFriendshipById(0);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ModifyPasswordByEmailDoesNotExistTest()
        {
            bool result = UsersManager.ModifyPasswordByEmail("eldorado@gmail.com", _password2) == 1;
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ModifyPasswordByEmailSuccessTest()
        {
            bool result = UsersManager.ModifyPasswordByEmail(_email1, _password2) == 1;
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void SendConfirmationCodeEmailExistsTest()
        {
            string email = "camiloesan@gmail.com";
            int result = UsersManager.SendConfirmationCode(email, "en");
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void UserBanTest()
        {
            string user1 = "pepe";
            string user2 = "antonio";
            string user3 = "jesus";
            UsersManager.AddUserToDatabase(user1, "degos@gmail.com", "12345");
            UsersManager.AddUserToDatabase(user2, "degas@gmail.com", "12345");
            UsersManager.AddUserToDatabase(user3, "degbs@gmail.com", "12345");
            UsersManager.BlockUserByUsername(user1 ,_user1);
            UsersManager.BlockUserByUsername(user2, _user1);
            int result = UsersManager.BlockUserByUsername(user3, _user1);
            Assert.AreEqual(2, result);
        }

        [TestMethod()]
        public void IsUserBannedTest()
        {
            string user1 = "pepe";
            string user2 = "antonio";
            string user3 = "jesus";
            UsersManager.AddUserToDatabase(user1, "degos@gmail.com", "12345");
            UsersManager.AddUserToDatabase(user2, "degas@gmail.com", "12345");
            UsersManager.AddUserToDatabase(user3, "degbs@gmail.com", "12345");
            UsersManager.BlockUserByUsername(user1, _user1);
            UsersManager.BlockUserByUsername(user2, _user1);
            UsersManager.BlockUserByUsername(user3, _user1);
            var userInfo = UsersManager.GetUserInfoByUsername(_user1);
            Assert.IsTrue(UsersManager.IsUserBanned(userInfo.ID));
        }
    }
}