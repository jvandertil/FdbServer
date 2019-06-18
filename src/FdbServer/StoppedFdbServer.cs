using System;
using System.IO;

namespace FdbServer
{
    internal class StoppedFdbServer : FdbServerBase, IStoppedFdbServer
    {
        public StoppedFdbServer(FdbServerBase server) 
            : base(server)
        {
        }

        public void Destroy()
        {
            Exception caughtException = null;
            do
            {
                try
                {
                    Directory.Delete(HomeDirectory, true);
                }
                catch (Exception e)
                {
                    caughtException = e;
                }
            }
            while (caughtException != null);
        }
    }
}
