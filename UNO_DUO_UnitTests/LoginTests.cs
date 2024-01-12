using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClienteDuo.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using ClienteDuo.TestClasses;

namespace ClienteDuo.Pages.Tests
{
    [TestClass()]
    public class LoginTests
    {
        [TestMethod()]
        public void IsUserNotLoggedInTest()
        {
            bool result = UsersManager.IsUserLoggedIn("non-existant");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUserLoggedInTest()
        {
            UserConnectionHandlerClient userConnectionHandlerClient = new UserConnectionHandlerClient();
            UserDTO userDTO = new UserDTO();
            userDTO.UserName = "joseSalazar";
            userConnectionHandlerClient.NotifyLogIn(userDTO);
            
            bool result = UsersManager.IsUserLoggedIn(userDTO.UserName);
            userConnectionHandlerClient.LogOut(userDTO.UserName);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void NotifyLogOutTest()
        {
            UserConnectionHandlerClient userConnectionHandlerClient = new UserConnectionHandlerClient();
            UserDTO userDTO = new UserDTO();
            userDTO.UserName = "joseSalazar";
            userConnectionHandlerClient.NotifyLogIn(userDTO);
            userConnectionHandlerClient.LogOut(userDTO.UserName);

            bool result = UsersManager.IsUserLoggedIn(userDTO.UserName);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void NotifyLogOutUserNotLoggedTest()
        {
            UserConnectionHandlerClient userConnectionHandlerClient = new UserConnectionHandlerClient();
            UserDTO userDTO = new UserDTO();
            userDTO.UserName = "joseSalazar";
            userConnectionHandlerClient.LogOut(userDTO.UserName);

            bool result = UsersManager.IsUserLoggedIn(userDTO.UserName);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void AreCredentialsValidCorrectTest()
        {
            string username = "demonslayer77";
            string email = "dprk@gmail.com";
            string password = "Tokyo2023!";

            UsersManager.AddUserToDatabase(username, email, password);

            UserDTO result = UsersManager.AreCredentialsValid(username, password);

            UsersManagerClient usersManagerClient = new UsersManagerClient();
            usersManagerClient.DeleteUserFromDatabaseByUsername(username);

            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void AreCredentialsValidIncorrectTest()
        {
            string username = "demonslayer77";
            string password = "Tokyo2023!";

            UserDTO result = UsersManager.AreCredentialsValid(username, password);

            Assert.AreEqual(0, result.ID);
        }

        [TestMethod()]
        public void AreCredentialsValidBlankTest()
        {
            string username = "";
            string password = "";

            UserDTO result = UsersManager.AreCredentialsValid(username, password);

            Assert.AreEqual(0, result.ID);
        }
    }
}