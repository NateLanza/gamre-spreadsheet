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

        private object ConnectionThreadKey;

        public int ConnectionState { get; private set; }

        private SocketState Connection;

        public GhostClient(string _IP)
        {
            IP = _IP;
        }

        public bool HasReceivedSpreadsheets ()
        {  return spreadsheetsReceived;  }

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

        ///////////////////////////////////////////////
        /// SPREADSHEET CONNECTION SEQUENCE BEGINS: ///
        /// ConnectToSpreadsheet                    ///
        ///////////////////////////////////////////////


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

    }
}
