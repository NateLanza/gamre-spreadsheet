using System;
using System.Collections.Generic;
using System.Threading;
using NetworkUtil;

namespace SS {

    /// <summary>
    /// Controls the spreadsheet, handling server connections
    /// and passing information to the SpreadsheetState
    /// TODO: Make thread safe
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
        /// Fires when this client disconnects from the server
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
        /// ID of this client, will be -1 if not connected
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Creates a new SpreadsheetController
        /// </summary>
        /// <param name="state">State object of current spreadsheet</param>
        public SpreadsheetController(SpreadsheetState state) {
            this.State = state;
            Connected = false;
            ID = -1;
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
                Connection.OnNetworkAction = SpreadsheetListCallback;
                Networking.GetData(Connection);
            }
        }

        /// <summary>
        /// Callback after receiving list of valid spreadsheets from server
        /// </summary>
        /// <param name="ss">Connection to server</param>
        private void SpreadsheetListCallback(SocketState ss) {
            // Ensure we have received a complete list
            if (!ss.GetData().Contains("\n\n")) {
                Networking.GetData(ss);
                return;
            }

            // Parse spreadsheet list, trigger event
            string serverData = ss.GetData();
            ss.RemoveData(0, serverData.Length);
            List<string> spreadsheets = new List<string>(serverData.Split('\n'));
            ConnectionAttempted(false, spreadsheets);
        }

        /// <summary>
        /// Connects to a spreadsheet on the server.
        /// Should only be called after a connection to server has been established
        /// and a list of spreadsheets has been received.
        /// </summary>
        /// <exception cref="InvalidOperationException">If a connection to the server has not been established</exception>
        /// <param name="name"></param>
        public void ConnectToSpreadsheet(string name) {
        }

        public void SendEditRequest(string cell, string contents) {
            if (!Connected || ID == -1 || Connection.ErrorOccured)
                return;
        }
    }
}
