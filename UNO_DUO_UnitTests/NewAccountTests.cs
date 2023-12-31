using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClienteDuo.DataService;
using ClienteDuo.Utilities;

namespace ClienteDuo.Pages.Tests
{
    [TestClass()]
    public class NewAccountTests
    {
        readonly string _initializedUsername = "demonslayer77";
        readonly string _initializedEmail = "dprk@gmail.com";
        readonly string _initializedPassword = "Tokyo2023!";

        [TestInitialize]
        public void Init()
        {
            UsersManager.AddUserToDatabase(_initializedUsername, _initializedEmail, _initializedPassword);
        }

        [TestCleanup]
        public void Cleanup()
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            usersManagerClient.DeleteUserFromDatabaseByUsername(_initializedUsername);
        }

        [TestMethod()]
        public void IsUsernameValidCorrectTest()
        {
            NewAccount newAccount = new NewAccount();
            string username = "sofia";
            bool result = newAccount.IsUsernameValid(username);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsUsernameValidMinLengthTest()
        {
            NewAccount newAccount = new NewAccount();
            string username = "ab";
            bool result = newAccount.IsUsernameValid(username);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUsernameValidMaxLengthTest()
        {
            NewAccount newAccount = new NewAccount();
            string username = "Uunumpentium12232maxpro";
            bool result = newAccount.IsUsernameValid(username);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUsernameValidNonAlphanumericCharactersTest()
        {
            NewAccount newAccount = new NewAccount();
            string username = "jesusSol!s";
            bool result = newAccount.IsUsernameValid(username);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void UsernameIsNotBeAvailableTest()
        {
            Assert.IsTrue(UsersManager.IsUsernameTaken(_initializedUsername));
        }

        [TestMethod()]
        public void UsernameIsAvailableTest()
        {
            Assert.IsFalse(UsersManager.IsUsernameTaken("jorgeAntonio23"));
        }

        [TestMethod()]
        public void EmailIsNotAvailableTest()
        {
            Assert.IsTrue(UsersManager.IsEmailTaken(_initializedEmail));
        }

        [TestMethod()]
        public void EmailIsAvailableTest()
        {
            Assert.IsFalse(UsersManager.IsEmailTaken("roland@gmail.com"));
        }

        [TestMethod()]
        public void PasswordIsNotMatchTest()
        {
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsPasswordMatch(_initializedPassword, "p455w0rd"));
        }

        [TestMethod()]
        public void PasswordIsMatchTest()
        {
            NewAccount newAccount = new NewAccount();
            Assert.IsTrue(newAccount.IsPasswordMatch(_initializedPassword, _initializedPassword));
        }

        [TestMethod()]
        public void IsEmailValidTooLargeTest()
        {
            string email = "demonslayer77groyperchuddie@gmail.com";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsEmailValid(email));
        }

        [TestMethod()]
        public void IsEmailValidCorrectTest()
        {
            string email = "taylosft@gmail.com";
            NewAccount newAccount = new NewAccount();
            Assert.IsTrue(newAccount.IsEmailValid(email));
        }

        [TestMethod()]
        public void IsPasswordSecureIncorrectTooSmallTest()
        {
            string password = "smoke";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsPasswordSecure(password));
        }

        [TestMethod()]
        public void IsPasswordSecureIncorrectWithoutNumberTest()
        {
            string password = "Xalapadosmil";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsPasswordSecure(password));
        }

        [TestMethod()]
        public void IsPasswordSecureIncorrectWithoutSpecialCharsTest()
        {
            string password = "Xalapa20000";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsPasswordSecure(password));
        }

        [TestMethod()]
        public void IsPasswordSecureIncorrectTooLargeTest()
        {
            string password = "Xalapa!!1234567891011123";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsPasswordSecure(password));
        }

        [TestMethod()]
        public void IsPasswordSecureCorrectStructureTest()
        {
            string password = "xalapA200!??";
            NewAccount newAccount = new NewAccount();
            Assert.IsTrue(newAccount.IsPasswordSecure(password));
        }

        [TestMethod()]
        public void UsernameContainsGuestKeywordTest()
        {
            string username = "guestTaylor";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsUsernameValid(username));
        }

        [TestMethod()]
        public void EmailInvalidLengthTest()
        {
            string email = "....1231231123123123@aol.es.com.mx";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsEmailValid(email));
        }

        [TestMethod()]
        public void EmailInvalidFormatTest()
        {
            string email = "robertoGonzales";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsEmailValid(email));
        }

        [TestMethod()]
        public void EmailDoenNotContainDomainTest()
        {
            string email = "robertoGonzales@";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsEmailValid(email));
        }

        [TestMethod()]
        public void EmailCorrectlyFormattedTest()
        {
            string email = "camiloesan@gmail.com";
            NewAccount newAccount = new NewAccount();
            Assert.IsTrue(newAccount.IsEmailValid(email));
        }

        [TestMethod()]
        public void AddUserAlreadyExistsTest()
        {
            int result = UsersManager.AddUserToDatabase(_initializedUsername, _initializedEmail, _initializedPassword);
            int expected = 0;
            Assert.AreEqual(expected, result);
        }
    }
}