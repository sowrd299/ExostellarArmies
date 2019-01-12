using System;
using System.Net;
using System.Net.Sockets;

namespace Server{

    public class GameServer{

        public const int Port = 4011;

        private IPHostEntry ipEntry;
        private IPAddress ipAddr;

        //socket managers
        private NewConListener ncl;

        //connected sockets

        public GameServer(){
            //get the IP address
            ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = ipEntry.AddressList[0];
            //setup socket managers
            ncl = new NewConListener(ipAddr, Port);
        }

        //to be called once per mainloop
        public void Update(){
            //handle new connections
            Socket s = ncl.Accept();
            if(s != null){
                //TODO: testing
                Console.WriteLine("A Client Connected!");
            }
        }

    }

}