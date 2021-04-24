using System;

namespace TestHandler
{
    class TestHandler
    {
        private static int numTests = 1;

        private static ClientHandler c;

        static void Main(string[] args)
        {           
            if (args.Length == 0)
                Console.WriteLine(numTests.ToString());
            else
            {
                c = new ClientHandler(args[1]);    // Second main Arg is IP

                if (args[1] == "1")                // First main arg is Test number
                { /*run test 1*/ }
            }
        }

        static void Test1 ()
        {
            Console.WriteLine("Max runtime");

            Console.WriteLine("Test Name");

            /*
            *    RUN THE TEST YAAAAAAY
            */

            if (true /*test passes*/)
                Console.WriteLine("Test Passed");
            else
                Console.WriteLine("Test Failed");
        }
    }
}
