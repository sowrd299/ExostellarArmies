using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Server{

    public class GameServer{

        public const int Port = 4011;

        private IPHostEntry ipEntry;
        private IPAddress ipAddr;

        //socket managers
        private NewConListener ncl;
        private List<ClientSocketManager> clientSockets;

        //connected sockets

        public GameServer(){
            //get the IP address
            ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = ipEntry.AddressList[0];
            //setup socket managers
            ncl = new NewConListener(ipAddr, Port);
            clientSockets = new List<ClientSocketManager>();
        }

        //to be called once per mainloop
        public void Update(){
            //handle new connections
            Socket s = ncl.Accept();
            if(s != null){
                Console.WriteLine("A Client Connected!");
                clientSockets.Add(new ClientSocketManager(s));
            }
            //read messages from clients
            for(int i = 0; i < clientSockets.Count; i++){
                string msg = clientSockets[i].Recieve();
                if(msg != null && msg != ""){
                    Console.WriteLine("Recieved message from client {0}: {1}", i.ToString(), msg);
                }
            }
        }

    }

}