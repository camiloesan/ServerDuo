using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Collections.Concurrent;

namespace CommunicationService.Tests
{
    [TestClass()]
    public class CallbackImplementationTests
    {
        [TestMethod()]
        public void NotifyCreatePartyTest()
        {
            var callbackMock = new Mock<IPartyManagerCallback>();
            var callbackInstance = callbackMock.Object;
            var serviceInstance = new CallbackImplementation(callbackInstance);

            serviceInstance.NotifyCreateParty(1102, "camilo");

            callbackMock.Verify(callback =>
                callback.PartyCreated(It.IsAny<ConcurrentDictionary<string, IPartyManagerCallback>>()), Times.Once);
        }
    }
}