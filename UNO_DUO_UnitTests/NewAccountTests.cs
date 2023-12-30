using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClienteDuo.DataService;

namespace ClienteDuo.Tests
{
    [TestClass()]
    public class NewAccountTests
    {
        string initializedUsername = "demonslayer77";
        string initializedEmail = "dprk@gmail.com";
        string initializedPassword = "Tokyo2023!";

        [TestInitialize]
        public void Init()
        {
            NewAccount newAccount = new NewAccount();
            newAccount.AddUserToDatabase(initializedUsername, initializedEmail, initializedPassword);
        }

        [TestCleanup]
        public void Cleanup()
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            usersManagerClient.DeleteUserFromDatabaseByUsername(initializedUsername);
        }

        [TestMethod()]
        public void UsernameIsNotBeAvailableTest()
        {
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsUsernameTaken(initializedUsername));
        }

        [TestMethod()]
        public void UsernameIsAvailableTest()
        {
            NewAccount newAccount = new NewAccount();
            Assert.IsTrue(newAccount.IsUsernameTaken("jorgeAntonio23"));
        }

        [TestMethod()]
        public void EmailIsNotAvailableTest()
        {
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsEmailTaken(initializedEmail));
        }

        [TestMethod()]
        public void EmailIsAvailableTest()
        {
            NewAccount newAccount = new NewAccount();
            Assert.IsTrue(newAccount.IsEmailTaken("roland@gmail.com"));
        }

        [TestMethod()]
        public void PasswordIsNotMatchTest()
        {
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsPasswordMatch(initializedPassword, "p455w0rd"));
        }

        [TestMethod()]
        public void PasswordIsMatchTest()
        {
            NewAccount newAccount = new NewAccount();
            Assert.IsTrue(newAccount.IsPasswordMatch(initializedPassword, initializedPassword));
        }

        [TestMethod()]
        public void UsernameFieldTooLargeTest()
        {
            string username = "demonslayer77groyperchuddiewojakbasedadonislaptophuaweisamsung8889";
            string email = "taylorswift55@gmail.com";
            NewAccount newAccount = new NewAccount();
            //Assert.IsFalse(newAccount.AreFieldsLengthValid(username, email));
        }

        [TestMethod()]
        public void EmailFieldTooLargeTest()
        {
            string username = "taylorswift";
            string email = "demonslayer77groyperchuddiewojakbasedadonislaptophuaweisamsung8889@gmail.com";
            NewAccount newAccount = new NewAccount();
            //Assert.IsFalse(newAccount.AreFieldsLengthValid(username, email));
        }

        [TestMethod()]
        public void CorrectFieldLengthTest()
        {
            string username = "taylorswift";
            string email = "taylosft@gmail.com";
            NewAccount newAccount = new NewAccount();
            //Assert.IsTrue(newAccount.AreFieldsLengthValid(username, email));
        }

        [TestMethod()]
        public void InsecurePasswordTest()
        {
            string password = "smoke";
            NewAccount newAccount = new NewAccount();
            Assert.IsFalse(newAccount.IsPasswordSecure(password));
        }

        public void SecurePasswordTest()
        {
            string password = "SmokeKing122!";
            NewAccount newAccount = new NewAccount();
            Assert.IsTrue(newAccount.IsPasswordSecure(password));
        }

        [TestMethod()]
        public void UsernameContainsGuestKeywordTest()
        {
            string username = "guestTaylor";
            NewAccount newAccount = new NewAccount();
            //Assert.IsTrue(newAccount.UsernameContainsGuestKeyword(username));
        }

        [TestMethod()]
        public void UsernameDoesNotContainGuestKeywordTest()
        {
            string username = "jesusSolis";
            NewAccount newAccount = new NewAccount();
            //Assert.IsFalse(newAccount.UsernameContainsGuestKeyword(username));
        }

        [TestMethod()]
        public void EmailContainsUnsupportedCharactersTest()
        {
            string email = "....@aol.es.com.mx";
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
    }
}