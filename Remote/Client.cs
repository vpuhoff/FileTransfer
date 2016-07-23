using System;
using System.Runtime.Remoting.Channels.Tcp;
using static System.Runtime.Remoting.Channels.ChannelServices;

namespace Remote
{
   public class Client
    {
        public ISharedTypeInterface RemoteObject;
        public Client(string host = "localhost",int port =9998)
        {
            var tcpChannel = new TcpChannel();
            var needreg = true;
            foreach (var item in RegisteredChannels )
            {
                if (item.ChannelName==tcpChannel.ChannelName )
                {
                    needreg = false;
                }
            }
            if (needreg )
            {
                RegisterChannel(tcpChannel,true);
            }
            

            Type requiredType = typeof(ISharedTypeInterface);

            RemoteObject = (ISharedTypeInterface)Activator.GetObject(requiredType,
            "tcp://"+ host+":"+port+"/RemoteService");
            Console.WriteLine(RemoteObject.GetRemoteStatus("Ticket No: 3344"));
        }
    }
}
