using System.Collections.Generic;
using System.Xml; // it's only used once, and I don't really like that
using System;
using SFB.Game.Content;
using SFB.Game.Management;

namespace SFB.Net.Server.Matches{

    // a class to represent and manage a match of the game being played on the server
    // using term "match" to distinguish from the game as a whole
    class Match{

        // the master gamestate
        private GameManager gameManager;

        // an array of all the player's in the game
        private PlayerManager[] players;

        private ReturnCallback returnCallback;

        public bool TurnLockedIn{
            get{
                bool turnLockedIn = true; // tracks if all players are read to advance
                foreach(PlayerManager pm in players){
                    turnLockedIn &= pm.TurnLockedIn;
                }
                return turnLockedIn;
            }
        }

        public Match(SocketManager[] clients, DeckList[] decks){
            gameManager = new GameManager(decks);
            //setup the players
            //TODO: if don't get exactly 2 players for a 2 player game, flip out
            players = new PlayerManager[clients.Length];
            for(int i = 0; i < clients.Length; i++){
                players[i] = new PlayerManager(clients[i],
                        gameManager.Players[i], gameManager,
                        CheckEndTurn, EndMatch);
            }
        }

        // starts the match
        public void Start(ReturnCallback rc){
            // get all the enemy player ID data each player needs
            List<XmlElement>[] playerIds = new List<XmlElement>[players.Length];
            // build all the arrays
            for(int i = 0; i < players.Length; i++){
                playerIds[i] = new List<XmlElement>();
            }
            // populate all the arrays with player id's
            // ...from other players
            for(int i = 0; i < players.Length; i++){
                XmlDocument doc = new XmlDocument();
                XmlElement e = players[i].GetPlayerIDs(doc);
                for(int j = (i+1)%players.Length; j != i; j = (j+1)%players.Length){
                    playerIds[j].Add(e);
                }
            }
            // send all the players all the ids and get them started
            for(int i = 0; i < players.Length; i++){
                players[i].Start(playerIds[i].ToArray());
            }
            returnCallback = rc; 
            // start the first turn;
            EndTurn();
        }

        // starts the match with asynchronous receiving
        public void AsynchStart(ReturnCallback rc){
            Start(rc);
            foreach(PlayerManager pm in players){
                pm.StartAsyncReceive();
            }
        }

        // to be called once per frame in synchronous mode
        public void Update(){
            foreach(PlayerManager pm in players){
                pm.Update();
            }
            CheckEndTurn();
        }

        // checks if the turn is over, and it is,
        // proceeds to the next turn
        // mostly here as a freestanding method so I can call it back
        public void CheckEndTurn(){
            if(TurnLockedIn){
                EndTurn();
            }
        }
        
        // TODO: avoid weird race conditions when a user resumes making actions
        public void EndTurn(){
            bool gameOver;
            List<Delta>[] turnDeltaLists = new List<Delta>[players.Length];
            lock(gameManager){
                // actually calculate the outcome of the turn
                Delta[] turnDeltas = gameManager.GetTurnDeltas();
                // figure out which deltas everyone needs
                for(int i = 0; i < players.Length; i++){ // for each player...
                    turnDeltaLists[i] = new List<Delta>();
                    for(int j = (i+1)%players.Length; j != i; j = (j+1)%players.Length){ // for each other player...
                        foreach(Delta d in players[j].TurnDeltas){ // share turn deltas...
                            turnDeltaLists[i].Add(d);
                        }
                    }
                    // add in the shared deltas
                    foreach(Delta d in turnDeltas){
                        turnDeltaLists[i].Add(d);
                    }
                }
                gameOver = gameManager.Over; // do this now while we have the lock
            }
            // share the deltas and restart turns
            for(int i = 0; i < players.Length; i++){
                Console.WriteLine("Giving {0} {1} enemy deltas; they produced {2} deltas this turn.",
                        players[i].Name, turnDeltaLists[i].Count, players[i].TurnDeltas.Length);
                players[i].StartTurn(turnDeltaLists[i].ToArray());
            }
            if(gameOver){
                EndMatch();
            }
        }

        public void EndMatch(){
            SocketManager[] sockets = new SocketManager[players.Length];
            for(int i = 0; i < players.Length; i++){
                players[i].EndMatch();
                sockets[i] = players[i].Socket;
            }
            returnCallback(sockets);
        }

        public delegate void ReturnCallback(SocketManager[] clients);

    }

}
