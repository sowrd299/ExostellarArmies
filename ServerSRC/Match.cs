using System.Collections.Generic;
using Game.Decks;
using Game;

namespace Server.Matches{

    // a class to represent and manage a match of the game being played on the server
    // using term "match" to distinguish from the game as a whole
    class Match{

        // the master gamestate
        private GameState gameState;

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
            gameState = new GameState();
            //setup the players
            //TODO: if don't get exactly 2 players for a 2 player game, flip out
            players = new PlayerManager[clients.Length];
            for(int i = 0; i < clients.Length; i++){
                players[i] = new PlayerManager(clients[i], new Player(), gameState, CheckEndTurn, EndMatch);
            }
        }

        // starts the match
        public void Start(ReturnCallback rc){
            foreach(PlayerManager pm in players){
                pm.Start();
            }
            returnCallback = rc; 
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
            lock(gameState){
                // actually calculate the outcome of the turn
                Delta[] turnDeltas = gameState.GetTurnDeltas();
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
                gameOver = gameState.Over; // do this now while we have the lock
            }
            // share the deltas and restart turns
            for(int i = 0; i < players.Length; i++){
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
