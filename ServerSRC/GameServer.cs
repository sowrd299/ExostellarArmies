using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Xml;
using Server.Matches;

namespace Server{

    public class GameServer : MessageHandler{

        public const int Port = 4011;
        public const string eof = "</file>";

        private IPHostEntry ipEntry;
        private IPAddress ipAddr;

        // socket managers
        private NewClientManager ncl;
        private List<SocketManager> clientSockets; //currently, this stores clients "at the main menu"

        // logic (and socket) managers
        private MatchMaker matchMaker;

        public GameServer(){
            //get the IP address
            ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = ipEntry.AddressList[0];
            //setup socket managers
            ncl = new NewClientManager(ipAddr, Port);
            clientSockets = new List<SocketManager>();
            matchMaker = new MatchMaker();
        }

        //to be called once per mainloop
        //TODO: ... except not really, that contutes a perfomant but power-hunger busy wait
        public void Update(){
            //handle new connections
            Socket s = ncl.Accept();
            if(s != null){
                Console.WriteLine("A Client Connected!");
                clientSockets.Add(new SocketManager(s, eof));
            }
            //read messages from clients
            //handle messages recieved (possibly)
            for(int i = clientSockets.Count-1; i >= 0; i--){
                handleSocket(clientSockets[i]);
            }
            //handle socket death
            //is its own for-loop to deal with weirdness from removing at two different points
            for(int i = clientSockets.Count-1; i >= 0; i--){
                if(i < clientSockets.Count && !clientSockets[i].Alive){
                    clientSockets.RemoveAt(i);
                    Console.WriteLine("A Client Disconnected :(");
                }
            }
            //start new games/matches
            Match newMatch = matchMaker.MakeMatch();
            if(newMatch != null){
                Console.WriteLine("Starting game!");
                // TODO: multithread! (maybe?)
                newMatch.Start();
            }
        }

        // NOTE: removes items from the list :S yeesh that's bad code
        public override void handleMessage(XmlDocument msg, SocketManager from){
            string type = msg.DocumentElement.Attributes["type"].Value;
            Console.WriteLine("Recieved message from new client of type {0}", type);
            //handle different types of messages
            switch(type){
                //go to match making
                case "joinMatch":
                    matchMaker.Enqueueu(from, msg);
                    clientSockets.Remove(from); //remove the socket so that two classes aren't trying to handle it
                    break;
                default:
                    base.handleMessage(msg, from);
                    break;
            }
        }

    }

}