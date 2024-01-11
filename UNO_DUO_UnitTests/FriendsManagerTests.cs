using ClienteDuo.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ClienteDuo.DataService;
using System.Linq;

namespace UNO_DUO_UnitTests
{
    [TestClass]
    public class FriendsManagerTests
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
            FriendsManager.SendFriendRequest(_user1, _user2);
            UserDTO user2Object = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = FriendsManager.GetFriendRequestsByUserId(user2Object.ID);


            bool result = FriendsManager.AcceptFriendRequest(requestList.First());

            var friends = FriendsManager.GetFriendsListByUserId(user2Object.ID);
            foreach (var item in friends)
            {
                FriendsManager.DeleteFriendshipById(item.FriendshipID);
            }

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void AcceptFriendRequestNotExistsTest()
        {
            FriendRequestDTO friendRequestDTO = new FriendRequestDTO();
            friendRequestDTO.SenderID = 0;
            bool result = FriendsManager.AcceptFriendRequest(friendRequestDTO);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void DeclineFriendRequestSuccessTest()
        {
            FriendsManager.SendFriendRequest(_user1, _user2);
            UserDTO user2Object = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = FriendsManager.GetFriendRequestsByUserId(user2Object.ID);

            bool result = FriendsManager.DeclineFriendRequest(requestList.First());

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void DeclineFriendRequestNotExistsTest()
        {
            FriendsManager.SendFriendRequest(_user1, _user2);

            FriendRequestDTO friendRequestDTO = new FriendRequestDTO();
            friendRequestDTO.SenderID = 0;
            bool result = FriendsManager.DeclineFriendRequest(friendRequestDTO);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void GetFriendRequestsByUserIdHasTwoTest()
        {
            string user3 = "roberto";
            string email3 = "roberto@gmail.com";
            string password3 = "panda!78990";
            UsersManager.AddUserToDatabase(user3, email3, password3);
            FriendsManager.SendFriendRequest(_user1, _user2);
            FriendsManager.SendFriendRequest(user3, _user2);

            UserDTO user2Object = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = FriendsManager.GetFriendRequestsByUserId(user2Object.ID);
            int actualSize = requestList.Count();

            foreach (var item in requestList)
            {
                FriendsManager.DeclineFriendRequest(item);
            }

            UsersManager.DeleteUserFromDatabase(user3);

            int expected = 2;

            Assert.AreEqual(expected, actualSize);
        }

        [TestMethod()]
        public void GetFriendRequestsByUserIdHasNoneTest()
        {
            UserDTO user2Object = UsersManager.GetUserInfoByUsername(_user1);
            var requestList = FriendsManager.GetFriendRequestsByUserId(user2Object.ID);

            int expected = 0;
            Assert.AreEqual(expected, requestList.Count());
        }


        [TestMethod()]
        public void IsAlreadyFriendLeftToRightTest()
        {
            FriendsManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = FriendsManager.GetFriendRequestsByUserId(userObject.ID);
            FriendsManager.AcceptFriendRequest(requestList.First());
            bool result = FriendsManager.IsAlreadyFriend(_user1, _user2);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsAlreadyFriendRightToLeftTest()
        {
            FriendsManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = FriendsManager.GetFriendRequestsByUserId(userObject.ID);
            FriendsManager.AcceptFriendRequest(requestList.First());
            bool result = FriendsManager.IsAlreadyFriend(_user2, _user1);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsAlreadyFriendNotFriendTest()
        {
            bool result = FriendsManager.IsAlreadyFriend(_user2, _user1);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsAlreadyFriendDoesNotExist()
        {
            bool result = FriendsManager.IsAlreadyFriend(_user2, "reikInolvidable");

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsFriendRequestAlreadySentTrueTest()
        {
            FriendsManager.SendFriendRequest(_user1, _user2);

            bool result = FriendsManager.IsFriendRequestAlreadySent(_user1, _user2);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsFriendRequestAlreadySentFalseTest()
        {
            bool result = FriendsManager.IsFriendRequestAlreadySent(_user1, _user2);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsFriendRequestAlreadySentUserDoesNotExistTest()
        {
            bool result = FriendsManager.IsFriendRequestAlreadySent(_user1, "cristinaRomo");

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void GetOnlineFriendsHasNoneTest()
        {
            FriendsManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = FriendsManager.GetFriendRequestsByUserId(userObject.ID);
            FriendsManager.AcceptFriendRequest(requestList.First());

            var result = FriendsManager.GetOnlineFriends(userObject.ID);

            int expected = 0;
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod()]
        public void GetFriendsListHasOneTest()
        {
            FriendsManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = FriendsManager.GetFriendRequestsByUserId(userObject.ID);
            FriendsManager.AcceptFriendRequest(requestList.First());

            var result = FriendsManager.GetFriendsListByUserId(userObject.ID);

            int expected = 1;
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod()]
        public void GetFriendsListHasZeroTest()
        {
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var result = FriendsManager.GetFriendsListByUserId(userObject.ID);

            int expected = 0;
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod()]
        public void DeleteFriendshipByIdSuccessTest()
        {
            FriendsManager.SendFriendRequest(_user1, _user2);
            UserDTO userObject = UsersManager.GetUserInfoByUsername(_user2);
            var requestList = FriendsManager.GetFriendRequestsByUserId(userObject.ID);
            FriendsManager.AcceptFriendRequest(requestList.First());
            var friendsList = FriendsManager.GetFriendsListByUserId(userObject.ID);
            bool result = FriendsManager.DeleteFriendshipById(friendsList.First().FriendshipID);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void DeleteFriendshipByIdDoesNotExistTest()
        {
            bool result = FriendsManager.DeleteFriendshipById(0);

            Assert.IsFalse(result);
        }

    }
}
