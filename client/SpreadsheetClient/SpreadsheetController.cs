using System;
using System.Collections.Generic;
using System.Threading;
using NetworkUtil;
using Newtonsoft.Json;

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
        public delegate void DisconnectHandler(string message);
        public event DisconnectHandler Disconnected;

        /// <summary>
        /// Fires when the server indicates that a change request sent by this client is invalid
        /// </summary>
        /// <param name="cellName">Name of the cell that a change was requested for</param>
        /// <param name="serverMessage">Error message sent by server</param>
        public delegate void InvalidChangeHandler(string cellName, string serverMessage);
        public event InvalidChangeHandler ChangeRejected;

        /// <summary>
        /// Current state of this spreadsheet
        /// </summary>
        private SpreadsheetState State;

        /// <summary>
        /// Stores the current server connection, if it exists
        /// </summary>
        private SocketState Connection;

        /// <summary>
        /// Current state of the server connection.
        /// Defined by a series of constants in the class ConnectionState
        /// </summary>
        public int ConnectionState { get; private set; }

        /// <summary>
        /// ID of this client, will be -1 if not connected
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Username of this client
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Creates a new SpreadsheetController
        /// </summary>
        /// <param name="state">State object of current spreadsheet</param>
        public SpreadsheetController(SpreadsheetState state) {
            this.State = state;
            ConnectionState = ConnectionStates.Disconnected;
            ID = -1;
        }

        /// <summary>
        /// Connects this client to a server, using a separate thread to handle
        /// all connection tasks
        /// </summary>
        /// <param name="server">Hostname or IP of the server</param>
        public void ConnectToServer(string server, string username) {
            Username = username;
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
                ConnectionState = ConnectionStates.WaitForSpreadsheets;
                Connection = connection;
                Connection.OnNetworkAction = SpreadsheetListCallback;
                // Send username to server
                Networking.Send(Connection.TheSocket, Username + "\n");
                // Get spreadsheet list
                Networking.GetData(Connection);
            }
        }

        /// <summary>
        /// Callback after receiving list of valid spreadsheets from server
        /// </summary>
        /// <param name="ss">Connection to server</param>
        private void SpreadsheetListCallback(SocketState ss) {
            // Ensure we have received a complete list
            if (!ss.GetData().EndsWith("\n\n")) {
                Networking.GetData(ss);
                return;
            }

            // Parse spreadsheet list, trigger event
            string serverData = ss.GetData();
            ss.RemoveData(0, serverData.Length);
            List<string> spreadsheets = new List<string>(serverData.Split('\n'));
            spreadsheets.Remove("");
            ConnectionAttempted(false, spreadsheets);
            ConnectionState = ConnectionStates.WaitForSpreadsheetConnection;
        }

        /// <summary>
        /// Connects to a spreadsheet on the server.
        /// Should only be called after a connection to server has been established
        /// and a list of spreadsheets has been received.
        /// </summary>
        /// <exception cref="InvalidOperationException">If ConnectionState != ConnectionStates.WaitForSpreadsheetConnection</exception>
        /// <exception cref="ArgumentException">If name contains \n</exception>
        /// <param name="name">Name of spreadsheet to connect to</param>
        public void ConnectToSpreadsheet(string name) {
            // Validate input
            if (ConnectionState != ConnectionStates.WaitForSpreadsheetConnection)
                throw new InvalidOperationException("Cannot connect to a spreadsheet when the connection state is not WaitForSpreadsheetConnection");
            if (name.Contains("\n"))
                throw new ArgumentException("Param name cannot contain a newline character \\n");

            // Send connection message to server
            Networking.Send(Connection.TheSocket, name + "\n");
            ConnectionState = ConnectionStates.WaitForID;
            Connection.OnNetworkAction = WaitForIDCallback;
            Networking.GetData(Connection);
        }

        /// <summary>
        /// Networking callback while receiving data leading up to and including
        /// this client's ID. Used right after sending a spreadsheet name to the server
        /// </summary>
        /// <param name="ss">Connection</param>
        private void WaitForIDCallback(SocketState ss) {
            // See if we have tokens
            List<string> serverTokens = ParseServerTokens();
            if (serverTokens.Count == 0) {
                Networking.GetData(Connection);
                return;
            }

            bool receivedID = false;
            // See if we've received the ID, set if so
            if (!serverTokens[serverTokens.Count - 1].Contains("{")) {
                string ID = serverTokens[serverTokens.Count - 1];
                if (int.TryParse(ID, out int intID)) {
                    this.ID = intID;
                    receivedID = true;
                } else {
                    throw new InvalidOperationException("Unable to parse ID from the server");
                }
                serverTokens.RemoveAt(serverTokens.Count - 1);
            }

            // Process json
            foreach (string token in serverTokens)
                ProcessServerJson(token);

            // Move to next connection stage if ID received
            if (receivedID) {
                ConnectionState = ConnectionStates.Connected;
                Connection.OnNetworkAction = ReceiveLoop;
                IDReceived(ID);
            }

            // Get more data from server
            Networking.GetData(Connection);
        }

        /// <summary>
        /// Loop to receive information from the server
        /// </summary>
        /// <param name="ss">Connection</param>
        private void ReceiveLoop(SocketState ss) {
            // Get & parse tokens
            List<string> tokens = ParseServerTokens();
            foreach (string token in tokens)
                ProcessServerJson(token);

            // Check for error
            if (Connection.ErrorOccured) {
                Disconnected("Connection error occurred");
            }
            // Continue look
            Networking.GetData(Connection);
        }
        
        /// <summary>
        /// Parses all received tokens from the server into a list of strings.
        /// Uses this.Connection as the connection to the server, to parse strings from
        /// </summary>
        /// <returns>List of tokens. Will be empty if no tokens received</returns>
        private List<String> ParseServerTokens() {
            // Make sure we're connected
            if (Connection == null)
                throw new InvalidOperationException("Connection must be established to parse tokens");

            // Get data, make sure we have a complete token
            string serverData = Connection.GetData();
            if (!serverData.Contains("\n")) {
                return new List<string>();
            }

            // Parse tokens
            List<string> serverTokens = new List<string>(serverData.Split('\n'));
            Connection.RemoveData(0, serverData.LastIndexOf('\n'));

            // Remove incomplete token
            if (!serverData.EndsWith("\n"))
                serverTokens.RemoveAt(serverTokens.Count - 1);

            return serverTokens;
        }

        /// <summary>
        /// Processes a JSON message received from the server and triggers
        /// all necessary events
        /// </summary>
        /// <param name="json"></param>
        private void ProcessServerJson(string json) {
            // Try to deserialize object
            ServerMessage message;
            try {
                message = JsonConvert.DeserializeObject<ServerMessage>(json);
            } catch (JsonException) {
                return;
            }

            // Take action based on message
            switch (message.Type) {
                case "cellUpdated":
                    CellChanged(message.Cell, message.Contents);
                    break;
                case "cellSelected":
                    SelectionChanged(message.Cell, message.Selector, message.ID);
                    break;
                case "disconnected":
                    OtherClientDisconnected(message.ID);
                    break;
                case "requestError":
                    ChangeRejected(message.Cell, message.Message);
                    break;
                case "serverError":
                    Disconnected(message.Message);
                    break;
            }
        }

        /// <summary>
        /// Sends an edit request to the server
        /// </summary>
        /// <param name="cell">Name of the cell</param>
        /// <param name="contents">New contents of the cell</param>
        public void SendEditRequest(string cell, string contents) {
            Networking.Send(Connection.TheSocket,
                "{requestType: \"editCell\", cellName: \"" +
                cell +
                "\", contents: \"" +
                contents +
                "\"}"
                );
        }

        /// <summary>
        /// Sends a revert request to the server
        /// </summary>
        /// <param name="cell">Cell to revert</param>
        public void SendRevertRequest(string cell) {
            Networking.Send(Connection.TheSocket,
                "{requestType: \"revertCell\", cellName: \"" +
                cell +
                "\"}"
                );
        }

        /// <summary>
        /// Sends a select request to the server
        /// </summary>
        /// <param name="cell">Cell to select</param>
        public void SendSelectRequest(string cell) {
            Networking.Send(Connection.TheSocket,
                "{requestType: \"selectCell\", cellName: \"" +
                cell +
                "\"}"
                );
        }

        /// <summary>
        /// Sends an undo request to the server
        /// </summary>
        public void SendUndoRequest() {
            Networking.Send(Connection.TheSocket,
                "{requestType: \"undo\"}"
                );
        }
    }
}
