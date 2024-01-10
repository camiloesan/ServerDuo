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
        public void UnblockUserByBlockIdSuccessTest()
        {

            BlockManager.BlockUserByUsername(_user1, _user2);
            UserDTO user1Object = UsersManager.GetUserInfoByUsername(_user1);
            var blockList = BlockManager.GetBlockedUsersListByUserId(user1Object.ID);

            bool result = BlockManager.UnblockUserByBlockId(blockList.First().BlockID) == 1;
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void UnblockUserByBlockIdUserDoesNotExistTest()
        {
            bool result = BlockManager.UnblockUserByBlockId(0) == 1;

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void GetBlockedUsersListByIdHasOneTest()
        {
            BlockManager.BlockUserByUsername(_user1, _user2);
            UserDTO user1Object = UsersManager.GetUserInfoByUsername(_user1);
            var result = BlockManager.GetBlockedUsersListByUserId(user1Object.ID);

            foreach (var item in result)
            {
                BlockManager.UnblockUserByBlockId(item.BlockID);
            }

            int expected = 1;
            Assert.AreEqual(expected, result.Count());
        }

        [TestMethod()]
        public void IsUserBlockedOneWayTest()
        {
            BlockManager.BlockUserByUsername(_user1, _user2);
            bool result = BlockManager.IsUserBlocked(_user1, _user2);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsUserBlockedBothWaysTest()
        {
            BlockManager.BlockUserByUsername(_user1, _user2);
            bool result = BlockManager.IsUserBlocked(_user2, _user1);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUserBlockedDoesNotExistTest()
        {
            bool result = BlockManager.IsUserBlocked("joseAntonio", "janoRodriguez");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUserBlockedNotBlockedTest()
        {
            bool result = BlockManager.IsUserBlocked(_user2, _user1);
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
            BlockManager.BlockUserByUsername(user1 ,_user1);
            BlockManager.BlockUserByUsername(user2, _user1);
            int result = BlockManager.BlockUserByUsername(user3, _user1);
            Assert.AreEqual(2, result);
            UsersManager.DeleteUserFromDatabase(user1);
            UsersManager.DeleteUserFromDatabase(user2);
            UsersManager.DeleteUserFromDatabase(user3);
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
            BlockManager.BlockUserByUsername(user1, _user1);
            BlockManager.BlockUserByUsername(user2, _user1);
            BlockManager.BlockUserByUsername(user3, _user1);
            var userInfo = UsersManager.GetUserInfoByUsername(_user1);
            Assert.IsTrue(UsersManager.IsUserBanned(userInfo.ID));
            UsersManager.DeleteUserFromDatabase(user1);
            UsersManager.DeleteUserFromDatabase(user2);
            UsersManager.DeleteUserFromDatabase(user3);
        }
    }
}