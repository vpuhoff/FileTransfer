using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using static System.Runtime.Remoting.Channels.ChannelServices;
using static System.Runtime.Remoting.RemotingConfiguration;

namespace Remote
{
    public class Server
    {
        public Server(int port = 9998)
        {
            Console.WriteLine("Remote Server started...");

            var tcpChannel = new TcpChannel(port);
            RegisterChannel(tcpChannel,true );

            var commonInterfaceType = Type.GetType("Remote.SharedType");

            RegisterWellKnownServiceType(commonInterfaceType,"RemoteService", WellKnownObjectMode.SingleCall);

            Console.WriteLine("Press ENTER to quitnn");
            Console.ReadLine();
        }
    }

    
}
