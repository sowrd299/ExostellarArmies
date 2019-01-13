using System;
using System.Xml;
using System.Collections.Generic;
using Server.Matches;
using Game.Decks;

namespace Server{

    // a class to make matches between waiting clients
    // and puts them into a game
    // responsible for knowing who wants to join what
    // and for deciding which game they will join

    //TODO: only put connected clients into games
    class MatchMaker{

        private Queue<MatchMakingInfo> waitingClients; //a queue of clients waiting to join games
        private DeckListManager deckListManager; // load and annylize player's deck lists

        public MatchMaker(){
            waitingClients = new Queue<MatchMakingInfo>();
            deckListManager = new DeckListManager();
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
                SocketManager[] clients = new SocketManager[playerCount];
                DeckList[] decks = new DeckList[playerCount]; // the deck lists the player's are using, in order
                for(int i = 0; i < playerCount; i++){
                    // currently using a simplistic "every two consecutive requests get paired"
                    // algorithm for who goes into the match
                    MatchMakingInfo client = waitingClients.Dequeue();
                    //Console.WriteLine("Adding player...");
                    clients[i] = client.Socket;
                    //Console.WriteLine("Loading deck...");
                    string deckID = client.EnqueueRequest.DocumentElement["deck"].Attributes["id"].Value;
                    decks[i] = deckListManager.LoadFromID(deckID);
                    //Console.WriteLine("Player {0} ready!", i);
                }
                return new Match(clients,decks);
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