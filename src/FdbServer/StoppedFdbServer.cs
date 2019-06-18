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
            Exception caughtException;
            do
            {
                try
                {
                    Directory.Delete(HomeDirectory, true);
                    caughtException = null;
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
