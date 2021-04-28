using System;
using System.Timers;

namespace TestHandler
{
    class TestHandler
    {
        private static int numTests = 2;

        private static string IP;

        static void Main(string[] args)
        {           
            if (args.Length == 0)
                Console.WriteLine(numTests.ToString());
            else
            {
                IP = args[1];                      // Second main Arg is IP

                if (args[0] == "0")                // First main Arg is Test number
                {  Test0();  }
                else if (args[0] == "1")
                {  Test1();  }
            }
        }

        private static void Timeout (Object source, ElapsedEventArgs e)
        {  Console.WriteLine("Test Failed : Timeout");  }

        static void Test0 ()
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

        static void Test1 ()
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
    }
}
