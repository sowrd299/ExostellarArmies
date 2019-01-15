using System.Collections.Generic;
using Game.Decks;

namespace Server.Matches{

    // a class to represent and manage a match of the game being played on the server
    // using term "match" to distinguish from the game as a whole
    class Match{

        // an array of all the player's in the game
        PlayerManager[] players;

        public Match(SocketManager[] clients, DeckList[] decks){
            //setup the players
            //TODO: if don't get exactly 2 players for a 2 player game, flip out
            players = new PlayerManager[clients.Length];
            for(int i = 0; i < clients.Length; i++){
                players[i] = new PlayerManager(clients[i], decks[i]);
            }
        }

        // starts the match
        public void Start(){
            foreach(PlayerManager pm in players){
                pm.Start();
            }
        }

        public void Update(){
            bool turnLockedIn = true; // tracks if all players are read to advance
            foreach(PlayerManager pm in players){
                pm.Update();
                turnLockedIn &= pm.TurnLockedIn;
            }
            if(turnLockedIn){
                // TODO: calculate the turn outcome
                // TODO: update player's and pm's with new game start
                foreach(PlayerManager pm in players){
                    pm.StartTurn();
                }
            }
        }

    }

}
