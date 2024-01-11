using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationService
{
    [ServiceContract]
    public class Card
    {
        /// <summary>
        /// Constructor with default values for a DOS card with color black
        /// </summary>
        public Card()
        {
            Number = "2";
            Color = "#000000";
        }

        /// <summary>
        /// Auxiliary constructor with set data
        /// </summary>
        /// <param name="number">Value for the Number property</param>
        /// <param name="color">Value for the Color property</param>
        public Card(string number, string color)
        {
            Number = number;
            Color = color;
        }

        /// <summary>
        /// Number of the card with ranges [0-9] and a Wildcard '#'
        /// </summary>
        [DataMember]
        public string Number { get; set; }

        /// <summary>
        /// Color of the card, must be a CardColors string (Black must only be used when the card is a Number 2)
        /// </summary>
        [DataMember]
        public string Color { get; set; }

        /// <summary>
        /// Compares color and number of a Card to check if they are equal
        /// </summary>
        /// <param name="obj"> Card object to be compared</param>
        /// <returns>Result of number and color comparison</returns>
        [OperationContract]
        public override bool Equals(object obj)
        {
            bool result = false;
            var card = obj as Card;

            if (card != null)
            {
                result = card.Number.Equals(Number) && card.Color.Equals(Color);
            }

            return result;
        }
    }
}
