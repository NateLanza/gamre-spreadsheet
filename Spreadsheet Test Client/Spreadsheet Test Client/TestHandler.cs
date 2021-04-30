using System;
using System.Timers;

namespace TestHandler
{
    class TestHandler
    {
        private static int numTests = 7;

        private static string IP;

        // Variables used in testing that the correct server messages are received
        private static string desiredCell;
        private static string desiredContent;
        private static int desiredID;
        private static bool correctSelectionReceived = false;
        private static bool correctEditReceived = false;
        private static bool correctRejectionReceived = false;

        static void Main(string[] args)
        {           
            if (args.Length == 0)
                Console.WriteLine(numTests.ToString());
            else
            {
                IP = args[1];                      // Second main Arg is IP

                if (args[0] == "1")                // First main Arg is Test number
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
            }
        }

        /// <summary>
        /// Default callback for a test which has timed out.
        /// </summary>
        /// <param name="source"> The timer which triggered this event </param>
        /// <param name="e"> Standard event args </param>
        private static void Timeout (Object source, ElapsedEventArgs e)
        {  Console.WriteLine("Test Failed : Timeout");  }

        private static void CellSelectionHandler (string cell, string name, int ID)
        {
            if (cell == desiredCell && ID == desiredID)
                correctSelectionReceived = true;
        }

        private static void CellEditHandler(string cell, string content)
        {
            if (cell == desiredCell && content == desiredContent)
                correctEditReceived = true;
        }

        private static void ChangeRejectedHandler (string cell, string message)
        {
            if (cell == desiredCell)
                correctSelectionReceived = true;
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
            GhostClient client1 = new GhostClient(IP);

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
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
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
            GhostClient client1 = new GhostClient(IP);
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
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
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
            GhostClient client1 = new GhostClient(IP);
            GhostClient client2 = new GhostClient(IP);
            GhostClient client3 = new GhostClient(IP);

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
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
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
            GhostClient client1 = new GhostClient(IP);
            GhostClient client2 = new GhostClient(IP);
            GhostClient client3 = new GhostClient(IP);
            client1.Connect();
            client2.Connect();
            client3.Connect();

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;


            // BEGIN TEST
            time.Start();
            client1.ConnectToSpreadsheet("sheet");
            client2.ConnectToSpreadsheet("sheet");
            client3.ConnectToSpreadsheet("sheet");

            while (time.Enabled)
            {
                if (client1.HasConnectedToSheet() && client2.HasConnectedToSheet() && client3.HasConnectedToSheet())
                {
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests basic cell selections by ensuring that the proper server message is received after the request is made
        /// </summary>
        public static void Test5 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Selection Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP);
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
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Expands on Test5 to test that the correct server response to a cell selection is received by multiple clients
        /// </summary>
        public static void Test6()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Multi-Client Basic Cell Selection Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP);
            GhostClient client2 = new GhostClient(IP);
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
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the correct change rejection is received when an invalid input is given to a cell selection request
        /// </summary>
        public static void Test7 ()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Selection Rejection Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP);
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
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests basic cell edits by ensuring that the proper server message is received after the request is made
        /// </summary>
        public static void Test8()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Selection Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP);
            client1.Connect();
            client1.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "New Content";
            client1.CellChanged += CellEditHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendEditRequest("A1", "New Content");

            while (time.Enabled)
            {
                if (correctEditReceived)
                {
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests multi-client cell edits by ensuring that the proper server message is received after the request is made
        /// </summary>
        public static void Test9()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Selection Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP);
            GhostClient client2 = new GhostClient(IP);
            client1.Connect();
            client2.Connect();
            client1.ConnectToSpreadsheet("sheet");
            client2.ConnectToSpreadsheet("sheet");

            // Setup desired results and register handler
            desiredCell = "A1";
            desiredContent = "New Content";
            client2.CellChanged += CellEditHandler;

            // Setup timer
            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

            // BEGIN TEST
            time.Start();
            client1.SendEditRequest("A1", "New Content");

            while (time.Enabled)
            {
                if (correctEditReceived)
                {
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
                    return;
                }
            }

            time.Close();
        }

        /// <summary>
        /// Tests that the correct change rejection is received when an invalid input is given to a cell edit request
        /// </summary>
        public static void Test10()
        {
            // Output test description to console
            Console.WriteLine("Max runtime: 4 seconds");
            Console.WriteLine("Basic Cell Edit Rejection Test");

            // Setup ghost clients
            GhostClient client1 = new GhostClient(IP);
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
                    Console.WriteLine("Test Passed");
                    time.Stop();
                    time.Close();
                    return;
                }
            }

            time.Close();
        }
    }
}
