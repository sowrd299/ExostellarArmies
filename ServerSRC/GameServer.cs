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
        private HashSet<SocketManager> clientSockets; //currently, this stores clients "at the main menu"
        private HashSet<SocketManager> removedSockets; //sockets from client sockets to be removed

        // logic (and socket) managers
        private MatchMaker matchMaker;

        public GameServer(){
            //get the IP address
            ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = ipEntry.AddressList[0];
            //setup socket managers
            ncl = new NewClientManager(ipAddr, Port);
            clientSockets = new HashSet<SocketManager>();
            removedSockets = new HashSet<SocketManager>();
            matchMaker = new MatchMaker();
        }

        //to be called once per mainloop
        //TODO: ... except not really, that contutes a perfomant but power-hunger busy wait
        public void Update(){
            //handle new connections
            SyncAccept();
            //handle new messages
            SyncReceive();
            //start new games/matches
            MakeMatch();
        }

        public SocketManager AddClient(Socket s){
            Console.WriteLine("A Client Connected!");
            SocketManager sm = new SocketManager(s, eof);
            clientSockets.Add(sm);
            return sm;
        }

        // accepts new connections in a synchronous, non-blocking way
        public void SyncAccept(){
            Socket s = ncl.Accept();
            if(s != null){
                AddClient(s);
            }
        }

        // recieves messages from attached sockets in a synchronous, non-blocking way
        public void SyncReceive(){
            foreach(SocketManager sm in clientSockets){
                //read messages from clients
                //handle messages recieved (possibly)
                handleSocket(sm);
                //handle socket death
                //is its own for-loop to deal with weirdness from removing at two different points
                if(!sm.Alive){
                    removedSockets.Add(sm);
                }
            }
            // finish the above removals
            clearRemovedSockets();
        }

        public override void handleMessage(XmlDocument msg, SocketManager from){
            string type = msg.DocumentElement.Attributes["type"].Value;
            Console.WriteLine("Recieved message from new client of type {0}", type);
            //handle different types of messages
            switch(type){
                //go to match making
                case "joinMatch":
                    matchMaker.Enqueueu(from, msg);
                    removedSockets.Add(from); //remove the socket so that two classes aren't trying to handle it
                    break;
                default:
                    base.handleMessage(msg, from);
                    break;
            }
        }

        // removes sockets marked for removal
        private void clearRemovedSockets(){
            foreach(SocketManager sm in removedSockets){
                clientSockets.Remove(sm);
            }
            removedSockets.Clear();
        }

        // starts a new match, if it can
        internal Match MakeMatch(){
            Match newMatch = matchMaker.MakeMatch();
            if(newMatch != null){
                Console.WriteLine("Starting game!");
                // TODO: multithread! (maybe?)
                newMatch.Start();
            }
            return newMatch;
        }

    }

}