using System;
using System.Collections.Generic;
using System.Text;

namespace TestHandler {
    /// <summary>
    /// Defines a series of constants which represent states of server connection.
    /// These are used as values of SpreadsheetController.ConnectionState
    /// </summary>
    public struct ConnectionStates {

        /// <summary>
        /// Client is not connected to a server
        /// </summary>
        public const int Disconnected = 0;
        /// <summary>
        /// Client is connected and waiting for the server to send the spreadsheet list
        /// </summary>
        public const int WaitForSpreadsheets = 1;
        /// <summary>
        /// Client has received the spreadsheet list and is waiting for input to
        /// connect to a specific spreadsheet
        /// </summary>
        public const int WaitForSpreadsheetConnection = 2;
        /// <summary>
        /// Client has sent the spreadsheet name to connect to and is waiting for its ID
        /// </summary>
        public const int WaitForID = 3;
        /// <summary>
        /// Client is connected to the server and has entered the send/receive loop
        /// </summary>
        public const int Connected = 4;
        /// <summary>
        /// When the client is attempting to establish a connection to the server
        /// </summary>
        public const int WaitingForConnection = 5;
    }
}
