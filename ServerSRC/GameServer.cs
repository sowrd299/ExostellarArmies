using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using SFB.Net.Server.Matches;

namespace SFB.Net.Server{

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

        public GameServer()
        {
            //get the IP address
            ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            int index_print = 0;
            foreach(var entry in ipEntry.AddressList){
                Console.WriteLine(index_print.ToString() + ": " + entry.ToString());
                ++index_print;
            }
            int index_read = 0;
            string index_read_str;
            do {
                Console.WriteLine("Please choose an IP address to use (not all IP addresses will work; please use the IP address for your LAN): ");
                index_read_str = Console.ReadLine();
            } while(!Int32.TryParse(index_read_str, out index_read) || index_read >= ipEntry.AddressList.Length); // repeat until get a valid int
            
            ipAddr = ipEntry.AddressList[index_read];
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
        public void StartAsyncAccept(){
            ncl.AsyncAccept(endAsyncAccept);
        }

        private void endAsyncAccept(Socket s){
            SocketManager sm = AddClient(s);
            StartAsyncReceive(sm);
            // loop accepting
            // TODO: probably should be toggleable
            StartAsyncAccept();
        }

        // accepts new connections in a synchronous, non-blocking way
        public void SyncAccept(){
            Socket s = ncl.Accept();
            if(s != null){
                AddClient(s);
            }
        }

        protected override void HandleSocketDeath(SocketManager socket){
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
                    HandleSocket(sm);
                    //is its own for-loop to deal with weirdness from removing at two different points
                    if(!sm.Alive){
                        removedSockets.Add(sm);
                    }
                }
                // finish the above removals
                clearRemovedSockets();
            }
        }

        public override void HandleMessage(XmlDocument msg, SocketManager from){
            string type = msg.DocumentElement.Attributes["type"].Value;
            //Console.WriteLine("While in 'Main Menu', Recieved message from client of type {0}", type);
            //handle different types of messages
            switch(type){
                //go to match making
                case "joinMatch":
                    matchMaker.Enqueueu(from, msg);
                    Console.WriteLine("A client has entered the lobby. Lobby size is {0}.", matchMaker.NumQueuedClients);
                    lock(removedSockets){
                        removedSockets.Add(from); //remove the socket so that two classes aren't trying to handle it
                    }
                    MakeMatch();
                    //Console.WriteLine("...I guess they are waiting for a while. Okay.");
                    break;
                default:
                    base.HandleMessage(msg, from);
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
            Console.WriteLine("Match making finished...");
            if(newMatch != null){
                Console.WriteLine("Starting game!");
                newMatch.AsyncStart(ReturnClients);
            }
            return newMatch;
        }

        // returns the given sockets to the management of the game server "main menu"
        // TODO: this is just a really dumb way to do this
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