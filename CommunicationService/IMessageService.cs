using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationService
{
    [ServiceContract(CallbackContract = typeof(IMessageServiceCallback))]
    internal interface IMessageService
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(string message);
    }

    [ServiceContract]
    internal interface IMessageServiceCallback
    {
        [OperationContract]
        void MessageReceived(string messageSent);
    }
}
