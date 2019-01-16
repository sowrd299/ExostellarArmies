using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Xml;
using Server.Matches;

namespace Server{

    // the central manager for activities on the server
    // doubles as both a master of sockets and a main menu
    // TODO: ...these two rolls should probably be forked
    // ... but we are well bellow the scale where this becomes an issue
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
            lock(clientSockets){
                clientSockets.Add(sm);
            }
            return sm;
        }

        // accepts new connections asynchronously
        // will continue to accept new connections ad infinitum
        public void StartAsynchAccept(){
            ncl.AsynchAccept(endAsynchAccept);
        }

        private void endAsynchAccept(Socket s){
            SocketManager sm = AddClient(s);
            StartAsyncReceive(sm);
            // loop accepting
            // TODO: probably should be toggleable
            StartAsynchAccept();
        }

        // accepts new connections in a synchronous, non-blocking way
        public void SyncAccept(){
            Socket s = ncl.Accept();
            if(s != null){
                AddClient(s);
            }
        }

        /* MOVED TO MESSAGE HANDLER
        // begins accepting new messages asynchronously
        // will continue to accept new messages ad infinitum
        // TODO: add this to Message Handler
        public void StartAsyncReceive(SocketManager sm){
            sm.AsynchReceiveXml(endAsynchReceive);
        }

        private void endAsynchReceive(XmlDocument msg, SocketManager from){
            handleMessage(msg, from);
            clearRemovedSockets(); // doing this is less efficient but more thread safe than checking removedSockets
            // resuming listening if the socket hasn't been sent somewhere else
            // TODO: this is a super weird way to handle the case where it moves on
            if(clientSockets.Contains(from)){
                StartAsyncReceive(from);
            }
        }
        // */

        protected override void handleSocketDeath(SocketManager socket){
            lock(clientSockets){
                clientSockets.Remove(socket);
            }
        }

        // recieves messages from attached sockets in a synchronous, non-blocking way
        public void SyncReceive(){
            lock(clientSockets){
                foreach(SocketManager sm in clientSockets){
                    //read messages from clients
                    //handle messages recieved (possibly)
                    //handle socket death
                    handleSocket(sm);
                    //is its own for-loop to deal with weirdness from removing at two different points
                    if(!sm.Alive){
                        removedSockets.Add(sm);
                    }
                }
                // finish the above removals
                clearRemovedSockets();
            }
        }

        public override void handleMessage(XmlDocument msg, SocketManager from){
            string type = msg.DocumentElement.Attributes["type"].Value;
            Console.WriteLine("While in 'Main Menu', Recieved message from client of type {0}", type);
            //handle different types of messages
            switch(type){
                //go to match making
                case "joinMatch":
                    matchMaker.Enqueueu(from, msg);
                    lock(removedSockets){
                        removedSockets.Add(from); //remove the socket so that two classes aren't trying to handle it
                    }
                    MakeMatch();
                    break;
                default:
                    base.handleMessage(msg, from);
                    break;
            }
        }

        // removes sockets marked for removal
        private void clearRemovedSockets(){
            lock(removedSockets){
                lock(clientSockets){
                    foreach(SocketManager sm in removedSockets){
                        clientSockets.Remove(sm);
                    }
                }
                removedSockets.Clear();
            }
        }

        // starts a new match, if it can
        internal Match MakeMatch(){
            Match newMatch = matchMaker.MakeMatch();
            if(newMatch != null){
                Console.WriteLine("Starting game!");
                // TODO: multithread! (maybe?)
                newMatch.AsynchStart(ReturnClients);
            }
            return newMatch;
        }

        // returns the given sockets to the management of the game server "main menu"
        // TODO: this is just a really dump way to do this
        internal void ReturnClients(SocketManager[] client){
            lock(clientSockets){
                // for each given client, record that it is here and start receiving from it
                foreach(SocketManager sm in client){
                    if(sm.Alive){ // TODO: this is a really course way to handle the possibility of being returned dead sockets
                        clientSockets.Add(sm);
                        StartAsyncReceive(sm);
                    }
                }
            }
            Console.WriteLine("Game Over Da!");
        }

    }

}