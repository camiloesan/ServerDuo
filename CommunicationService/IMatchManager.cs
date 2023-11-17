using System.Runtime.Serialization;
using System.ServiceModel;

namespace CommunicationService
{
    [ServiceContract]
    public interface IMatchManager
    {
        [OperationContract]
        Card DrawCard();

        [OperationContract]
        Card[] GetTableCards();

        [OperationContract(IsOneWay = true)]
        void DealTableCards();

        [OperationContract(IsOneWay = true)]
        void EndTurn();

        [OperationContract(IsOneWay = true)]
        void EndRound();

        [OperationContract]
        void EndGame();

        [OperationContract]
        void InitializeData();

        [OperationContract(IsOneWay = true)]
        void PlayCard(int position);
    }

    [DataContract]
    public class Card
    {
        public Card()
        {
            Number = "2";
            Color = "#000000";
        }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Color { get; set; }
    }
}
