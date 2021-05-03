using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SS {

    /// <summary>
    /// Represents a message received from the server
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class ServerMessage {

        /// <summary>
        /// Type of this ServerMessage.
        /// Can be: cellUpdated, cellSelected, disconnected, requestError, serverError
        /// </summary>
        [JsonProperty(PropertyName = "messageType")]
        public string Type { get; private set; }

        /// <summary>
        /// Name of the cell in this ServerMessage
        /// Will not be set for Type == disconnected || serverError
        /// </summary>
        [JsonProperty(PropertyName = "cellName")]
        public string Cell { get; private set; }

        /// <summary>
        /// Contents of the cell, used in a cellUpdated message
        /// </summary>
        [JsonProperty(PropertyName = "contents")]
        public string Contents { get; private set; }

        /// <summary>
        /// ID of the user who caused this message.
        /// Used for cellSelected and disconnected messages
        /// </summary>
        [JsonProperty(PropertyName = "selector")]
        public int ID { get; private set; }

        /// <summary>
        /// Routes "user" JSON fields to the ID field
        /// Never contains a value, setting sets the ID field
        /// </summary>
        [JsonProperty(PropertyName = "user")]
        private int ID2 { 
            get { throw new NotImplementedException("Property ID2 does not contain a value"); } 
            set { ID = value; } 
        }

        /// <summary>
        /// Name of the user who selected a cell
        /// Used by cellSelected messages
        /// </summary>
        [JsonProperty(PropertyName = "selectorName")]
        public string Selector { get; private set; }

        /// <summary>
        /// Text of message from server
        /// Used by requestError and serverError
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; private set; }

    }
}
