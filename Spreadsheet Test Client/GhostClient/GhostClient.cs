using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace TestHandler
{
    public class GhostClient
    {
        private string IP;

        private List<string> spreadsheets;

        private bool spreadsheetsReceived = false;
        private bool connectedToSheet = false;

        private object ConnectionThreadKey;

        public int ConnectionState { get; private set; }

        private SocketState Connection;

        private int ID;

        public GhostClient(string _IP)
        {
            IP = _IP;
        }

        public bool HasReceivedSpreadsheets ()
        {  return spreadsheetsReceived;  }

        public bool HasConnectedToSheet ()
        { return connectedToSheet;   }

        ////////////////////////////////////////////////////////////////////////////////
        /// CONNECTION SEQUENCE BEGINS:                                              ///
        /// Connect --> ServerConnectionCallback --> SpreadsheetListCallback ///
        ////////////////////////////////////////////////////////////////////////////////

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
                Networking.ConnectToServer(ServerConnectionCallback, IP, 1100);
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

        /*----------------------------------------------------------------------------------------------------------------*/

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
                    Disconnected("Connection error occurred");           //Event!!
                }
            }
            // Continue look
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
                    CellChanged(message.Cell, message.Contents);                       //Event!!
                    break;
                case "cellSelected":
                    SelectionChanged(message.Cell, message.Selector, message.ID);      //Event!!
                    break;
                case "disconnected":
                    OtherClientDisconnected(message.ID);                               //Event!!
                    break;
                case "requestError":
                    ChangeRejected(message.Cell, message.Message);                     //Event!!
                    break;
                case "serverError":
                    Disconnected(message.Message);                                     //Event!!
                    break;
            }
        }

        ////////////////////////////////////////////
        /// SPREADSHEET CONNECTION SEQUENCE ENDS ///
        ////////////////////////////////////////////

    }
}
