using System;

namespace TestHandler
{
    public class ClientHandler
    {
        private GhostClient client1;
        private GhostClient client2;
        private GhostClient client3;
        private GhostClient client4;

        public ClientHandler (string IP)
        {
            client1 = new GhostClient(IP);
            client2 = new GhostClient(IP);
            client3 = new GhostClient(IP);
            client4 = new GhostClient(IP);
        }
    }
}
