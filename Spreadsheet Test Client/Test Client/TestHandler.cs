using System;
using System.Collections.Generic;
using System.Timers;

namespace TestHandler
{
    public class TestHandler
    {
        // The number of tests that this program supprts
        private static int numTests = 26;

        // The IP and port of the server this program is testing
        private static string IP;
        private static string port;

        // Variables used in testing that the correct server messages are received
        private static string desiredCell;
        private static string desiredContent;
        private static int desiredID;

        // Bools used to test the server selection change functionality
        private static bool correctSelectionReceived = false;
        private static bool incorrectSelectionReceived = false;

        // Bools used to test the server cell change functionality
        private static bool correctChangeReceived = false;
        private static bool incorrectChangeReceived = false;

        // Bools used to test the server change rejection functionmality
        private static bool rejectionReceived = false;
        private static bool correctRejectionReceived = false;
        private static bool incorrectRejectionReceived = false;

        public static void Main(string[] args)
        {           
            if (args.Length == 0)
                Console.WriteLine(numTests.ToString());
            else
            {
                // Get IP and Port
                string[] input = args[1].Split(":");    // Second main Arg is IP:Port
                IP = input[0];
                port = input[1];


                // Run specefied Test
                if (args[0] == "1")                     // First main Arg is Test number
                    Test1();
                else if (args[0] == "2")
                    Test2();
                else if (args[0] == "3")
                    Test3();
                else if (args[0] == "4")
                    Test4();
                else if (args[0] == "5")
                    Test5();
                else if (args[0] == "6")
                    Test6();
                else if (args[0] == "7")
                    Test7();
                else if (args[0] == "8")
                    Test8();
                else if (args[0] == "9")
                    Test9();
                else if (args[0] == "10")
                    Test10();
                else if (args[0] == "11")
                    Test11();
                else if (args[0] == "12")
                    Test12();
                else if (args[0] == "13")
                    Test13();
                else if (args[0] == "14")
                    Test14();
                else if (args[0] == "15")
                    Test15();
                else if (args[0] == "16")
                    Test16();
                else if (args[0] == "17")
                    Test17();
                else if (args[0] == "18")
                    Test18();
                else if (args[0] == "19")
                    Test19();
                else if (args[0] == "20")
                    Test20();
                else if (args[0] == "21")
                    Test21();
                else if (args[0] == "22")
                    Test22();
                else if (args[0] == "23")
                    Test23();
                else if (args[0] == "24")
                    Test24();
                else if (args[0] == "25")
                    Test25();
                else if (args[0] == "26")
                    Test26();
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Default callback for a test which has timed out.
        /// </summary>
        /// <param name="source"> The timer which triggered this event </param>
        /// <param name="e"> Standard event args </param>
        private static void Timeout (Object source, ElapsedEventArgs e)
        {  Console.WriteLine("Test Failed : Timeout");  }

        /// <summary>
        /// Determines if the cell selection message that this client received was the one expected.
        /// </summary>
        /// <param name="cell"> The name of the cell selected </param>
        /// <param name="name"> The name of the client who selected the cell </param>
        /// <param name="ID"> The ID of the client who selected the cell </param>
        private static void CellSelectionHandler (string cell, string name, int ID)
        {
            if (cell == desiredCell && ID == desiredID)
                correctSelectionReceived = true;
            else
                incorrectSelectionReceived = true;
        }

        /// <summary>
        /// Determines if the cell change message that this client received was the one expected.
        /// </summary>
        /// <param name="cell"> The name of the cell changed </param>
        /// <param name="content"> The new content of the cell </param>
        private static void CellChangeHandler(string cell, string content)
        {
            if (cell == desiredCell && content == desiredContent)
                correctChangeReceived = true;
            else
                incorrectChangeReceived = true;
        }

        /// <summary>
        /// Determines if the invalid cell change message that this client received was the one expected.
        /// </summary>
        /// <param name="cell"> The name of the cell in which a change was rejected </param>
        /// <param name="message"> The rejection message from the server  </param>
        private static void ChangeRejectedHandler (string cell, string message)
        {
            rejectionReceived = true;

            if (cell == desiredCell)
                correctRejectionReceived = true;
            else
                incorrectRejectionReceived = true;
        }

        /// <summary>
        /// Tests that a client can connect to the server.
        /// </summary>
        public static void Test1 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Connection Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.Connect();

            while (time.Enabled)
            {
                if (client1.HasReceivedSpreadsheets())
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that a client can connect to a spreadsheet
        /// </summary>
        public static void Test2 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Spreadsheet Connection Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);
            client1.Connect();

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.ConnectToSpreadsheet("sheet");

            while (time.Enabled)
            {
                if (client1.HasConnectedToSheet())
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
            }

            time.Close();
        }
        
        /// <summary>
        /// Expands on Test1 to test that multiple clients can be connected to a server at once.
        /// </summary>
        public static void Test3 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Multi-Client Server Connection Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP, port);
            GhostClient client2 = new GhostClient(IP, port);
            GhostClient client3 = new GhostClient(IP, port);

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.Connect();
            client2.Connect();
            client3.Connect();

            while (time.Enabled)
            {
                if (client1.HasReceivedSpreadsheets() && client2.HasReceivedSpreadsheets() && client3.HasReceivedSpreadsheets())
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Expands on Test2 to test that multiple clients can be connected to a spreadsheet at once.
        /// </summary>
        public static void Test4 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Multi-Client Spreadsheet Connection Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP, port);
            GhostClient client2 = new GhostClient(IP, port);
            GhostClient client3 = new GhostClient(IP, port);
            
            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });
            GhostClient.ServerConnectionHandler client2Callback = ((bool error, List<String> ssNames) => {
                client2.ConnectToSpreadsheet("sheet");
            });
            GhostClient.ServerConnectionHandler client3Callback = ((bool error, List<String> ssNames) => {
                client3.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;
            client2.ConnectionAttempted += client2Callback;
            client3.ConnectionAttempted += client3Callback;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.Connect();
            client2.Connect();
            client3.Connect();

            while (time.Enabled)
            {
                if (client1.HasConnectedToSheet() && client2.HasConnectedToSheet() && client3.HasConnectedToSheet())
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that clients can connect to multiple different spreadsheets simultaneously
        /// </summary>
        public static void Test5 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Multi-Spreadsheet Connection Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);
            GhostClient client2 = new GhostClient(IP, port);
            client1.Connect();
            client2.Connect();

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.ConnectToSpreadsheet("sheet1");
            client2.ConnectToSpreadsheet("sheet2");

            while (time.Enabled)
            {
                if (client1.HasConnectedToSheet() && client2.HasConnectedToSheet())
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests basic cell selections by ensuring that the proper server message is received after the request is made
        /// </summary>
        public static void Test6 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Selection Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredID = client1.ID;
            client1.SelectionChanged += CellSelectionHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendSelectRequest("A1");

            while (time.Enabled)
            {
                if (correctSelectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectSelectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Expands on Test5 to test that the correct server response to a cell selection is received by multiple clients
        /// </summary>
        public static void Test7 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Multi-Client Basic Cell Selection Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP, port);
            GhostClient client2 = new GhostClient(IP, port);
            client1.Connect();
            client2.Connect();
            client1.ConnectToSpreadsheet("sheet");
            client2.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredID = client1.ID;
            client2.SelectionChanged += CellSelectionHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendSelectRequest("A1");

            while (time.Enabled)
            {
                if (correctSelectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectSelectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the correct change rejection is received when an invalid input is given to a cell selection request
        /// </summary>
        public static void Test8 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Selection Rejection Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "Incorrect Input";
            client1.ChangeRejected += ChangeRejectedHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendSelectRequest("Incorrect Input");

            while (time.Enabled)
            {
                if (correctRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests basic cell edits by ensuring that the proper server message is received after the request is made
        /// </summary>
        public static void Test9 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Edit Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "New Content";
            client1.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendEditRequest("A1", "New Content");

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");
                    
                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests multi-client cell edits by ensuring that the proper server message is received after the request is made
        /// </summary>
        public static void Test10 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Multi-Client Cell Edit Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP, port);
            GhostClient client2 = new GhostClient(IP, port);
            client1.Connect();
            client2.Connect();
            client1.ConnectToSpreadsheet("sheet");
            client2.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "New Content";
            client2.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendEditRequest("A1", "New Content");

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");
                    
                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the correct change rejection is received when an invalid input is given to a cell edit request
        /// </summary>
        public static void Test11 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Edit Rejection Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "Incorrect Input";
            client1.ChangeRejected += ChangeRejectedHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendEditRequest("Incorrect Input", "New Content");

            while (time.Enabled)
            {
                if (correctRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");
                    
                    return;
                }
                else if (incorrectRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server responds correctly to a valid undo request
        /// </summary>
        public static void Test12 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Undo Request Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "Value 1");
            client1.SendEditRequest("A1", "Value 2");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "Value 1";
            client1.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendUndoRequest();

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server responds correctly to a valid undo request by another client
        /// </summary>
        public static void Test13 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Multi-Client Undo Request Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP, port);
            GhostClient client2 = new GhostClient(IP, port);
            client1.Connect();
            client2.Connect();
            client1.ConnectToSpreadsheet("sheet");
            client2.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "Value 1");
            client2.SendEditRequest("A1", "Value 2");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "Value 1";
            client2.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendUndoRequest();

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server properly responds to multiple consecutive undo requests
        /// </summary>
        public static void Test14 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Multiple Undo Requests Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "Value 1");
            client1.SendEditRequest("A1", "Value 2");
            client1.SendEditRequest("B1", "Value 3");

            client1.SendUndoRequest();

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "Value 1";
            client1.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendUndoRequest();

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server responds correctly to a valid revert request
        /// </summary>
        public static void Test15 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Revert Request Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "Value 1");
            client1.SendEditRequest("A1", "Value 2");
            client1.SendEditRequest("B1", "Value 3");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "Value 1";
            client1.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendRevertRequest("A1");

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server responds correctly to a valid revert request from a different client.
        /// </summary>
        public static void Test16 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Multi-Client Revert Request Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP, port);
            GhostClient client2 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });
            GhostClient.ServerConnectionHandler client2Callback = ((bool error, List<String> ssNames) => {
                client2.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;
            client2.ConnectionAttempted += client2Callback;

            // Setup test
            client1.Connect();
            client2.Connect();
            client1.ConnectToSpreadsheet("sheet");
            client2.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "Value 1");
            client2.SendEditRequest("A1", "Value 2");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "Value 1";
            client1.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendRevertRequest("A1");

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server responds correctly to multiple consecutive revert requests
        /// </summary>
        public static void Test17 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Multiple Revert Requests Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "Value 1");
            client1.SendEditRequest("A1", "Value 2");
            client1.SendEditRequest("A1", "Value 3");
            client1.SendEditRequest("B1", "Value 4");

            client1.SendRevertRequest("A1");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "Value 1";
            client1.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendRevertRequest("A1");

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server responds correctly to an invalid undo request
        /// </summary>
        public static void Test18 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Invalid Undo Request Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            client1.ChangeRejected += ChangeRejectedHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendUndoRequest();

            while (time.Enabled)
            {
                if (rejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (rejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server rejects a revert request on a raw spreadsheet
        /// </summary>
        public static void Test19 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Invalid Revert Request Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "A1";
            client1.ChangeRejected += ChangeRejectedHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendRevertRequest("A1");

            while (time.Enabled)
            {
                if (correctRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server rejects a revert request trageted at an invalid cell
        /// </summary>
        public static void Test20 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Invalid Revert Target Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "Incorrect Input";
            client1.ChangeRejected += ChangeRejectedHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendRevertRequest("Incorrect Input");

            while (time.Enabled)
            {
                if (correctRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server correctly handles the interaction between the undo and revert functions
        /// </summary>
        public static void Test21 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Undoing Revert Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "Value 1");

            client1.SendRevertRequest("A1");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "Value 1";
            client1.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendUndoRequest();

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that a basic formula is handled correctly by the server
        /// </summary>
        public static void Test22 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Formula Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "2";
            client1.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendEditRequest("A1", "= 1 + 1");

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that basic cell dependency (via formula) is handled correctly by the server
        /// </summary>
        public static void Test23 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Dependency Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            //Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "Value 1");

            // Setup desired results and register handler
            desiredCell = "B1";
            desiredContent = "Value 1";
            client1.CellChanged += CellChangeHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendEditRequest("B1", "=A1");

            while (time.Enabled)
            {
                if (correctChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectChangeReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the server responds correctly to a formula which uses an invalid variable
        /// </summary>
        public static void Test24 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Invalid Formula Variable Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "New Content";
            client1.ChangeRejected += ChangeRejectedHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendEditRequest("A1", "=B123A");

            while (time.Enabled)
            {
                if (correctRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        public static void Test25 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Invalid Formula Variable Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "=B1");

            // Setup desired results and register handler
            desiredCell = "B1";
            client1.ChangeRejected += ChangeRejectedHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendEditRequest("B1", "=A1");

            while (time.Enabled)
            {
                if (correctRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }

        public static void Test26 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Invalid Formula Variable Test");

            // Setup ghost client
            GhostClient client1 = new GhostClient(IP, port);

            // Connection callbacks
            GhostClient.ServerConnectionHandler client1Callback = ((bool error, List<String> ssNames) => {
                client1.ConnectToSpreadsheet("sheet");
            });

            // Attach callbacks
            client1.ConnectionAttempted += client1Callback;

            // Setup test
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            client1.SendEditRequest("A1", "=B1");
            client1.SendEditRequest("A1", "New Content");
            client1.SendEditRequest("B1", "=A1");

            // Setup desired results and register handler
            desiredCell = "A1";
            client1.ChangeRejected += ChangeRejectedHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendRevertRequest("A1");

            while (time.Enabled)
            {
                if (correctRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Passed");

                    return;
                }
                else if (incorrectRejectionReceived)
                {
                    time.Stop();
                    time.Close();

                    Console.WriteLine("Test Failed");

                    return;
                }
            }

            time.Close();
        }
    }
}
