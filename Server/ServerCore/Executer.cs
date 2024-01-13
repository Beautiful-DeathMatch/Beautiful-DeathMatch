using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Linq;

namespace ServerCore
{
    public class Executer
    {
        private IPEndPoint myIPEndPoint = null;
        private int myPortNumber;

        private Listener mylistener = new Listener(ProtocolType.Tcp, 10, 100);

        private JobTimer jobTimer = new JobTimer();
        private int flushWaitTime = 250;

        private Func<EndPoint, Session> sessionFactory = null;

        public Executer(Func<EndPoint, Session> sessionFactory, int portNumber)
        {
            this.sessionFactory = sessionFactory;
            this.myPortNumber = portNumber;
        }

        private void FlushMyRoom()
        {
            RoomManager.Instance.Flush();

            jobTimer.Push(FlushMyRoom, flushWaitTime);
        }

        public void Execute()
        {
            myIPEndPoint = GetMyEndPoint(myPortNumber);
            if (myIPEndPoint == null)
                return;

            mylistener.Start(myIPEndPoint, sessionFactory);
            jobTimer.Start(FlushMyRoom);

            while (true)
            {
                jobTimer.Tick();
            }
        }

        private IPEndPoint GetMyEndPoint(int portNumber)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddress = ipHost.AddressList.FirstOrDefault();

            return new IPEndPoint(ipAddress, portNumber);
        }
    }
}
