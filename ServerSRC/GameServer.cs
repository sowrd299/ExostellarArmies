using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Xml;
using Server.Matches;

namespace Server{

    public class GameServer{

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
            //count backwards so can remove as go
            for(int i = clientSockets.Count-1; i >= 0; i--){
                XmlDocument msg = clientSockets[i].ReceiveXml();
                //handle socket death
                if(!clientSockets[i].Alive){
                    clientSockets.RemoveAt(i);
                    Console.WriteLine("A Client Disconnected :(");
                }
                //handle messages recieved
                if(msg != null){
                    string type = msg.DocumentElement.Attributes["type"].Value;
                    Console.WriteLine("Recieved message from new client {0} of type {1}", i, type);
                    //handle different types of messages
                    switch(type){
                        //go to match making
                        case "joinMatch":
                            matchMaker.Enqueueu(clientSockets[i], msg);
                            clientSockets.RemoveAt(i); //remove the socket so that two classes aren't trying to handle it
                            break;
                        default:
                            //if the client did not send an expected message type, send back an error
                            clientSockets[i].Send("<file type='error'><msg>Unexpected message type: "+type+"</msg></file>");
                            break; 
                    }
                    /* a simple test response back to the client
                    clientSockets[i].Send("<file type='acc'><ACC/></file>");
                    //*/
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

    }

}