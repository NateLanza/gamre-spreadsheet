using System;
using System.Timers;

namespace TestHandler
{
    class TestHandler
    {
        private static int numTests = 1;

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
                { /*run test 1*/ }
            }
        }

        private static void Timeout (Object source, ElapsedEventArgs e)
        {  Console.WriteLine("Test Failed : Timeout");  }

        static void Test0 ()
        {
            Console.WriteLine("Max runtime: 4 seconds");

            Console.WriteLine("Basic Connection Test");

            GhostClient client1 = new GhostClient(IP);

            Timer time = new Timer(4000);
            time.Elapsed += Timeout;

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
    }
}
