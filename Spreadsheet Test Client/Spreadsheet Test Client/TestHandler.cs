using System;
using System.Timers;

namespace TestHandler
{
    class TestHandler
    {
        private static int numTests = 3;

        private static string IP;

        static void Main(string[] args)
        {           
            if (args.Length == 0)
                Console.WriteLine(numTests.ToString());
            else
            {
                IP = args[1];                      // Second main Arg is IP

                if (args[0] == "1")                // First main Arg is Test number
                {  Test0();  }
                else if (args[0] == "2")
                {  Test1();  }
                else if (args[0] == "3")
                {  Test2();  }
            }
        }

        /// <summary>
        /// Default callback for a test which has timed out.
        /// </summary>
        /// <param name="source"> The timer which triggered this event </param>
        /// <param name="e"> Standard event args </param>
        private static void Timeout (Object source, ElapsedEventArgs e)
        {  Console.WriteLine("Test Failed : Timeout");  }

        /// <summary>
        /// Tests that a client can connect to the server.
        /// </summary>
        public static void Test0 ()
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
        public static void Test1 ()
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
        /// Expands on Test0 to test that multiple clients can be connected to a server at once.
        /// </summary>
        public static void Test2 ()
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
    }
}
