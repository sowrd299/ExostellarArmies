using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Xml;

namespace Server{

    public class GameServer{

        public const int Port = 4011;
        public const string eof = "</file>";

        private IPHostEntry ipEntry;
        private IPAddress ipAddr;

        //socket managers
        private NewClientManager ncl;
        private List<SocketManager> clientSockets;

        //connected sockets

        public GameServer(){
            //get the IP address
            ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = ipEntry.AddressList[0];
            //setup socket managers
            ncl = new NewClientManager(ipAddr, Port);
            clientSockets = new List<SocketManager>();
        }

        //to be called once per mainloop
        public void Update(){
            //handle new connections
            Socket s = ncl.Accept();
            if(s != null){
                Console.WriteLine("A Client Connected!");
                clientSockets.Add(new SocketManager(s, eof));
            }
            //read messages from clients
            for(int i = 0; i < clientSockets.Count; i++){
                XmlDocument msg = clientSockets[i].ReceiveXml();
                if(msg != null){
                    Console.WriteLine("Recieved message from client {0}: {1}", i.ToString(), msg.DocumentElement.Attributes["type"]);
                    clientSockets[i].Send("<file type='acc'><ACC/></file>");
                }
            }
        }

    }

}