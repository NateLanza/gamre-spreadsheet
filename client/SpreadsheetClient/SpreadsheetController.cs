using System.Collections.Generic;
using System.Threading;
using NetworkUtil;

namespace SS {

    /// <summary>
    /// Controls the spreadsheet, handling server connections
    /// and passing information to the SpreadsheetState
    /// </summary>
    public class SpreadsheetController {

        // ===== Events =====
        /// <summary>
        /// Fires when a cell change is received from the server
        /// </summary>
        /// <param name="cell">Name of cell</param>
        /// <param name="contents">New contents of cell</param>
        public delegate void CellChangeHandler(string cell, string contents);
        public event CellChangeHandler CellChanged;

        /// <summary>
        /// Fires when a cell selection change is received from the server
        /// </summary>
        /// <param name="cell">Name of cell</param>
        /// <param name="name">Name of client selecting the cell</param>
        /// <param name="id">ID of client selecting the cell</param>
        public delegate void SelectionChangeHandler(string cell, string name, int id);
        public event SelectionChangeHandler SelectionChanged;

        /// <summary>
        /// Fires when a successful connection to the server is established and
        /// a list of spreadsheets is received from the server, 
        /// or if an attempted connection to the server fails
        /// </summary>
        /// <param name="error">True if an error occurred and no connection was established</param>
        /// <param name="spreadsheets">List of spreadsheets that can be opened. Will be null if error == true</param>
        public delegate void ServerConnectionHandler(bool error, List<string> spreadsheets);
        public event ServerConnectionHandler ConnectionAttempted;

        /// <summary>
        /// Fires when this client receives its unique ID from the server
        /// </summary>
        /// <param name="ID">ID of this client</param>
        public delegate void IDReceivedHandler(int ID);
        public event IDReceivedHandler IDReceived;

        /// <summary>
        /// Fires when another client connected to the server (not this client) disconnects
        /// </summary>
        /// <param name="ID">ID of the disconnected client</param>
        public delegate void ClientDisconnectHandler(int ID);
        public event ClientDisconnectHandler OtherClientDisconnected;

        /// <summary>
        /// Fires when this client disconnects from the server unexpectedly
        /// </summary>
        public delegate void DisconnectHandler();
        public event DisconnectHandler Disconnected;

        /// <summary>
        /// Fires when the server indicates that a change request sent by this client is invalid
        /// </summary>
        /// <param name="cellName">Name of the cell that a change was requested for</param>
        /// <param name="serverMessage">Error message sent by server</param>
        public delegate void InvalidChangeHandler(string cellName, string serverMessage);
        public event InvalidChangeHandler ChangeRejected;

        /// <summary>
        /// Fires if the server sends a serverError message and disconnects this client
        /// </summary>
        /// <param name="message">Message sent by the server</param>
        public delegate void ServerShutdownHandler(string message);
        public event ServerShutdownHandler ServerClosed;


        /// <summary>
        /// Current state of this spreadsheet
        /// </summary>
        private SpreadsheetState State;

        /// <summary>
        /// Stores the current server connection, if it exists
        /// </summary>
        private SocketState Connection;

        /// <summary>
        /// Whether this client is connected to a server
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Creates a new SpreadsheetController
        /// </summary>
        /// <param name="state">State object of current spreadsheet</param>
        public SpreadsheetController(SpreadsheetState state) {
            this.State = state;
            Connected = false;
        }

        /// <summary>
        /// Connects this client to a server, using a separate thread to handle
        /// all connection tasks
        /// </summary>
        /// <param name="server">Hostname or IP of the server</param>
        public void ConnectToServer(string server) {
            Thread t = new Thread(() => {
                Networking.ConnectToServer(ServerConnectionCallback, server, 1100);
            });
            t.Start();
        }

        /// <summary>
        /// Networking callback after attempting to connect to a server
        /// </summary>
        /// <param name="connection">Socketstate created by connection attempt</param>
        private void ServerConnectionCallback(SocketState connection) {
            if (connection.ErrorOccured) {
                ConnectionAttempted(true, null);
            } else {
                Connected = true;
                Connection = connection;
            }
        }
    }
}
