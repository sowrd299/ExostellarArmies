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
            // a disposable Xml document for creating nodes in
            // TODO: this is really bad practice
            XmlDocument doc = new XmlDocument();
            // get all the enemy player ID data each player needs
            List<XmlElement>[] playerIds = new List<XmlElement>[players.Length];
            // build all the arrays
            for(int i = 0; i < players.Length; i++){
                playerIds[i] = new List<XmlElement>();
            }
            // TODO: this whole ID collecting section might be better served in the game manager, I'm not sure
            // populate all the arrays with player id's
            // ...from other players
            for(int i = 0; i < players.Length; i++){
                XmlElement e = players[i].GetPlayerIDs(doc);
                for(int j = (i+1)%players.Length; j != i; j = (j+1)%players.Length){
                    playerIds[j].Add(e);
                }
            }
            // send all the players all the ids and get them started
            XmlElement[] laneIds = gameManager.GetLaneIDs(doc);
            for(int i = 0; i < players.Length; i++){
                players[i].Start(playerIds[i].ToArray(), laneIds, i);
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
        // to be called after ending deployment
        // NOTE: technically, multiple deployment phases withing one turn are implemented
        // as multiple turns, with many phases skipped
        public void EndTurn(){
            bool gameOver;
            List<Delta>[] turnDeltaLists = new List<Delta>[players.Length];
            lock(gameManager){
                // calculate the outcome of the deploy phase
                // use a list to collect the deltas, to send them later
                List<Delta> turnDeltas = new List<Delta>(); 
                // cleanup the deploy phase
                foreach(Delta d in gameManager.GetEndDeployDeltas()){
                    turnDeltas.Add(d);
                    gameManager.ApplyDelta(d);
                }
				foreach(Delta d in gameManager.GetCleanUpDeltas()) {
					turnDeltas.Add(d);
					gameManager.ApplyDelta(d);
				}
				// if no more deployment phases, do the rest of this turn into the start of the next
				if(gameManager.DeployPhasesOver()){
                    // ranged combat
                    foreach(Delta d in gameManager.GetRangedCombatDeltas()){
                        turnDeltas.Add(d);
                        gameManager.ApplyDelta(d);
                    }
                    foreach(Delta d in gameManager.GetCleanUpDeltas()){
                        turnDeltas.Add(d);
                        gameManager.ApplyDelta(d);
                    }
                    // melee combat
                    foreach(Delta d in gameManager.GetMeleeCombatDeltas()){
                        turnDeltas.Add(d);
                        gameManager.ApplyDelta(d);
                    }
                    foreach(Delta d in gameManager.GetCleanUpDeltas()){
                        turnDeltas.Add(d);
                        gameManager.ApplyDelta(d);
                    }
					
					// tower damage
					foreach(Delta d in gameManager.GetTowerDamageDeltas()) {
						turnDeltas.Add(d);
						gameManager.ApplyDelta(d);
					}
					foreach(Delta d in gameManager.GetCleanUpDeltas()) {
						turnDeltas.Add(d);
						gameManager.ApplyDelta(d);
					}

					// start of the next turn
					foreach(Delta d in gameManager.GetStartTurnDeltas()){
                        turnDeltas.Add(d);
                        gameManager.ApplyDelta(d);
                    }
                }
                // get the start of the next deploy phase
                foreach(Delta d in gameManager.GetStartDeployDeltas()){
                    turnDeltas.Add(d);
                    gameManager.ApplyDelta(d);
                }
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
