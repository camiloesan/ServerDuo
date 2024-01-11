using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationService
{
    /// <summary>
    /// Stores color hexes to avoid code duplication
    /// </summary>
    [DataContract]
    public class CardColors
    {
        /// <summary>
        /// Hex for color blue used in cards
        /// </summary>
        [DataMember]
        public static readonly string BLUE = "#0000FF";

        /// <summary>
        /// Hex for color yellow used in cards
        /// </summary>
        [DataMember]
        public static readonly string YELLOW = "#FFFF00";

        /// <summary>
        /// Hex for color green used in cards
        /// </summary>
        [DataMember]
        public static readonly string GREEN = "#008000";

        /// <summary>
        /// Hex for color red used in cards
        /// </summary>
        [DataMember]
        public static readonly string RED = "#FF0000";

        /// <summary>
        /// Hex for color black used in cards with the number 2 only
        /// </summary>
        [DataMember]
        public static readonly string BLACK = "#000000";
    }
}
