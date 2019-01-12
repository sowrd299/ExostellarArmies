using System;
using System.Xml;
using System.Collections.Generic;

namespace Server{

    // a class to make matches between waiting clients
    // and puts them into a game
    // responsible for knowing who wants to join what
    // and for deciding which game they will join
    class MatchMaker{

        private Queue<MatchMakingInfo> waitingClients; //a queue of clients waiting to join games

        public MatchMaker(){
            waitingClients = new Queue<MatchMakingInfo>();
        }

        // adds the given client to the match-making queue
        public void Enqueueu(SocketManager socket, XmlDocument enqueueRequest){
            waitingClients.Enqueue(new MatchMakingInfo{Socket = socket, EnqueueRequest = enqueueRequest});
        }

        // if possible, make the next game
        // only make one match at a time, to avoid too much blocking
        // returns the new game object
        public Match MakeMatch(){
            const int playerCount = 2; //this is here so that different matches can have difterent player counts
            if(waitingClients.Count >= playerCount){ 
                List<SocketManager> clients = new List<SocketManager>();
                for(int i = 0; i < playerCount; i++){
                    // currently using a simplistic "every two consecutive requests get paired"
                    // algorithm for who goes into the match
                    clients.Add(waitingClients.Dequeue().Socket);
                }
                return new Match(clients);
            }
            return null;
        }


        // an inner class to store all relivant data for one client
        private class MatchMakingInfo{

            public SocketManager Socket;
            public XmlDocument EnqueueRequest; //save the XML message to enqueue for now, incase add important data to it

        }

    }

}