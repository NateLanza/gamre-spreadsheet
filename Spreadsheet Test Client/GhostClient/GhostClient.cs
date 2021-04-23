using System;

namespace TestHandler
{
    public class GhostClient
    {
        private ServerMessage serverMessage;

        public GhostClient ()
        {
        }

        public Boolean Connect()
        {
            return true;
        }

        public Boolean SendServerMessage ()
        {
            return true;
        }

        public ServerMessage GetServerMessage ()
        {
            return serverMessage;
        }
    }
}
