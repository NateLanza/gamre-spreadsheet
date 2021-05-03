using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace TestHandler
{
    public class GhostClient
    {

        //=================================================== Events =================================================//

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
        /// Fires when this client receives its unique ID from the server
        /// </summary>
        /// <param name="ID">ID of this client</param>
        public delegate void IDReceivedHandler(int ID);
        public event IDReceivedHandler IDReceived;

        /// <summary>
        /// Fires when the server indicates that a change request sent by this client is invalid
        /// </summary>
        /// <param name="cellName">Name of the cell that a change was requested for</param>
        /// <param name="serverMessage">Error message sent by server</param>
        public delegate void InvalidChangeHandler(string cellName, string serverMessage);
        public event InvalidChangeHandler ChangeRejected;

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
        /// Fires when a successful connection to the server is established and
        /// a list of spreadsheets is received from the server, 
        /// or if an attempted connection to the server fails
        /// </summary>
        /// <param name="error">True if an error occurred and no connection was established</param>
        /// <param name="spreadsheets">List of spreadsheets that can be opened. Will be null if error == true</param>
        public delegate void ServerConnectionHandler(bool error, List<string> spreadsheets);
        public event ServerConnectionHandler ConnectionAttempted;

        //===========================================================================================================//

        private string IP;
        private int port;

        private List<string> spreadsheets;

        private bool spreadsheetsReceived = false;
        private bool connectedToSheet = false;

        private object ConnectionThreadKey = new object();

        public int ConnectionState { get; private set; }

        private SocketState Connection;

        public int ID;

        public GhostClient(string _IP, string _port)
        {
            IP = _IP;
            port = Int32.Parse(_port);
        }

        public bool HasReceivedSpreadsheets ()
        {  return spreadsheetsReceived;  }

        public bool HasConnectedToSheet ()
        { return connectedToSheet;   }

        ////////////////////////////////////////////////////////////////////////
        /// CONNECTION SEQUENCE BEGINS:                                      ///
        /// Connect --> ServerConnectionCallback --> SpreadsheetListCallback ///
        ////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Connects the ghost client to the specified server with ServerConnectionCallback
        /// as the registered callback
        /// </summary>
        /// <param name="server"> the address of the server to be connected to </param>
        public void Connect()
        {
            lock (ConnectionThreadKey)
            {
                ConnectionState = ConnectionStates.WaitingForConnection;
            }
            Thread t = new Thread(() => {
                Networking.ConnectToServer(ServerConnectionCallback, IP, port);
            });
            t.Start();
        }

        /// <summary>
        /// Networking callback after attempting to connect to a server
        /// </summary>
        /// <param name="connection">Socketstate created by connection attempt</param>
        private void ServerConnectionCallback(SocketState connection)
        {
            lock (ConnectionThreadKey)
            {
                // Make sure we're in the right state
                if (ConnectionState != ConnectionStates.WaitingForConnection)
                    return;

                // Check for valid connection
                if (connection.ErrorOccured)
                    ConnectionState = ConnectionStates.Disconnected;
                else
                {
                    ConnectionState = ConnectionStates.WaitForSpreadsheets;
                    Connection = connection;
                    Connection.OnNetworkAction = SpreadsheetListCallback;

                    // Send username to server
                    Networking.Send(Connection.TheSocket, "GhostClient\n");
                    // Get spreadsheet list
                    Networking.GetData(Connection);
                }
            }
        }

        /// <summary>
        /// Callback after receiving list of valid spreadsheets from server
        /// </summary>
        /// <param name="ss">Connection to server</param>
        private void SpreadsheetListCallback(SocketState ss)
        {
            // Validate connection state
            lock (ConnectionThreadKey)
            {
                if (ConnectionState != ConnectionStates.WaitForSpreadsheets)
                    return;
            }

            // Ensure we have received a complete list
            if (!ss.GetData().EndsWith("\n\n"))
            {
                Networking.GetData(ss);
                return;
            }

            // Parse spreadsheet list,
            string serverData = ss.GetData();
            ss.RemoveData(0, serverData.Length);
            spreadsheets = new List<string>(serverData.Split('\n'));
            spreadsheets.Remove("");

            // Boolean set for tester purposes
            spreadsheetsReceived = true;

            // Check for race conditions, trigger event
            lock (ConnectionThreadKey)
            {
                if (ConnectionState != ConnectionStates.WaitForSpreadsheets)
                    return;

                ConnectionState = ConnectionStates.WaitForSpreadsheetConnection;
            }
        }

        ////////////////////////////////
        /// CONNECTION SEQUENCE ENDS ///
        ////////////////////////////////



        //////////////////////////////////////////////////////////////////
        /// SPREADSHEET CONNECTION SEQUENCE BEGINS:                    ///
        /// ConnectToSpreadsheet --> WaitForIDCallback --> ReceiveLoop ///
        //////////////////////////////////////////////////////////////////


        /// <summary>
        /// Connects to a spreadsheet on the server.
        /// Should only be called after a connection to server has been established
        /// and a list of spreadsheets has been received.
        /// </summary>
        /// <exception cref="InvalidOperationException">If ConnectionState != ConnectionStates.WaitForSpreadsheetConnection</exception>
        /// <exception cref="ArgumentException">If name contains \n</exception>
        /// <param name="name">Name of spreadsheet to connect to</param>
        public void ConnectToSpreadsheet(string name)
        {
            // Validate connection state
            lock (ConnectionThreadKey)
            {
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
        }

        /// <summary>
        /// Networking callback while receiving data leading up to and including
        /// this client's ID. Used right after sending a spreadsheet name to the server
        /// </summary>
        /// <param name="ss">Connection</param>
        private void WaitForIDCallback(SocketState ss)
        {
            // Validate state
            lock (ConnectionThreadKey)
            {
                if (ConnectionState != ConnectionStates.WaitForID)
                    return;
            }

            // See if we have tokens
            List<string> serverTokens = ParseServerTokens();
            if (serverTokens.Count == 0)
            {
                Networking.GetData(Connection);
                return;
            }

            bool receivedID = false;
            // See if we've received the ID, set if so
            if (!serverTokens[serverTokens.Count - 1].Contains("{"))
            {
                string ID = serverTokens[serverTokens.Count - 1];
                if (int.TryParse(ID, out int intID))
                {
                    this.ID = intID;
                    receivedID = true;
                }
                else
                {
                    throw new InvalidOperationException("Unable to parse ID from the server");
                }
                serverTokens.RemoveAt(serverTokens.Count - 1);
            }

            // Process json
            foreach (string token in serverTokens)
                ProcessServerJson(token);

            // Move to next connection stage if ID received
            if (receivedID)
            {
                lock (ConnectionThreadKey)
                {
                    connectedToSheet = true;

                    ConnectionState = ConnectionStates.Connected;
                    Connection.OnNetworkAction = ReceiveLoop;
                    IDReceived(ID);                                      //Event!!
                }
            }

            // Get more data from server
            Networking.GetData(Connection);
        }

        /// <summary>
        /// Loop to receive information from the server
        /// </summary>
        /// <param name="ss">Connection</param>
        private void ReceiveLoop(SocketState ss)
        {
            // Get & parse tokens
            List<string> tokens = ParseServerTokens();
            foreach (string token in tokens)
                ProcessServerJson(token);

            // Check for error
            lock (ConnectionThreadKey)
            {
                if (Connection.ErrorOccured)
                {
                    Disconnected("Connection error occurred");
                }
            }

            // Continue loop
            Networking.GetData(Connection);
        }

        private List<String> ParseServerTokens()
        {
            lock (ConnectionThreadKey)
            {
                // Make sure we're connected
                if (Connection == null || ConnectionState == ConnectionStates.Disconnected || ConnectionState == ConnectionStates.WaitingForConnection)
                    throw new InvalidOperationException("Connection must be established to parse tokens");

                // Get data, make sure we have a complete token
                string serverData = Connection.GetData();
                if (!serverData.Contains("\n"))
                {
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
        }

        /// <summary>
        /// Processes a JSON message received from the server and triggers
        /// all necessary events
        /// </summary>
        /// <param name="json"></param>
        private void ProcessServerJson(string json)
        {
            // Try to deserialize object
            ServerMessage message;
            try
            {
                message = JsonConvert.DeserializeObject<ServerMessage>(json);
            }
            catch (JsonException)
            {
                return;
            }

            // Take action based on message
            switch (message.Type)
            {
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

        ////////////////////////////////////////////
        /// SPREADSHEET CONNECTION SEQUENCE ENDS ///
        ////////////////////////////////////////////



        //////////////////////////////////
        /// SENDING CLIENT MESSAGES:   ///
        /// Edit, Revert, Select, Undo ///
        //////////////////////////////////
        
        /// <summary>
        /// Sends an edit request to the server
        /// </summary>
        /// <param name="cell">Name of the cell</param>
        /// <param name="contents">New contents of the cell</param>
        public void SendEditRequest(string cell, string contents)
        {
            // Validate current connection state
            lock (ConnectionThreadKey)
            {
                if (ConnectionState != ConnectionStates.Connected)
                    return;
            }

            Networking.Send(Connection.TheSocket,
                "{\"requestType\": \"editCell\", \"cellName\": \"" +
                cell +
                "\", \"contents\": \"" +
                contents +
                "\"}"
                );
        }

        /// <summary>
        /// Sends a revert request to the server
        /// </summary>
        /// <param name="cell">Cell to revert</param>
        public void SendRevertRequest(string cell)
        {
            // Validate current connection state
            lock (ConnectionThreadKey)
            {
                if (ConnectionState != ConnectionStates.Connected)
                    return;
            }

            Networking.Send(Connection.TheSocket,
                "{\"requestType\": \"revertCell\", \"cellName\": \"" +
                cell +
                "\"}"
                );
        }

        /// <summary>
        /// Sends a select request to the server
        /// </summary>
        /// <param name="cell">Cell to select</param>
        public void SendSelectRequest(string cell)
        {
            // Validate current connection state
            lock (ConnectionThreadKey)
            {
                if (ConnectionState != ConnectionStates.Connected)
                    return;
            }

            Networking.Send(Connection.TheSocket,
                "{\"requestType\": \"selectCell\", \"cellName\": \"" +
                cell +
                "\"}"
                );
        }

        /// <summary>
        /// Sends an undo request to the server
        /// </summary>
        public void SendUndoRequest()
        {
            // Validate current connection state
            lock (ConnectionThreadKey)
            {
                if (ConnectionState != ConnectionStates.Connected)
                    return;
            }

            Networking.Send(Connection.TheSocket,
                "{\"requestType\": \"undo\"}"
                );
        }

        ///////////////////////////////////
        /// SENDING CLIENT MESSAGES END ///
        ///////////////////////////////////
    }
}
