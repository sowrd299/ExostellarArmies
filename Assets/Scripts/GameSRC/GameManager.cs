using SFB.Game.Content;
using System.Collections.Generic;
using System.Xml;

namespace SFB.Game.Management{

    // a class to reprsent the state of the game at a give point in time
    public class GameManager{

        public bool over() {
			foreach(Player p in players)
				if(p.Lives == 0)
					return true;
			return false;
        }

        Player[] players;
        // an array of all the players
        // MUST BE IN THE SAME ORDER AS THEIR DECKLISTS/IDS WERE PROVIDED
        public Player[] Players{
            get{
                return players;
            }
        }

        // when hidden list is null, will init players with "hidden decks" of unkown cards
        //      functionality intended for clients
        // when ids is null, will init with newly generated ids
        //      functionality intended for servers
        // when both are null, will start a game with no players
        // MUST KEEP THE INDEXES OF THE OBJECTS GIVEN TO IT
        public GameManager(DeckList[] deckLists = null, XmlElement[] ids = null){
            int numPlayers = deckLists != null ? deckLists.Length : (ids != null ? ids.Length : 0);
            players = new Player[numPlayers];
            for(int i = 0; i < numPlayers; i++){
                DeckList hiddenList =  new DeckList();
                hiddenList.AddCard(new UnknownCard(), 20); // TODO: support decks of different sizes?
                DeckList list = deckLists != null ? deckLists[i] : hiddenList;
                players[i] = new Player("Player " + (i+1), list, ids != null ? ids[i] : null);
            }
        }

        // returns if a move is legal for the given player to make
        public bool IsLegalAction(Player player, Action a){
            // dummy implementation
            return true;
        }

        // returns the outcomes of a player taking a give action
        public Delta[] GetActionDeltas(Player player, Action a){
            // dummy implementation
            // return new Delta[]{new Deck.RemoveFromDeckDelta(player.Deck, null, 0)}; // this implementation intrinsically throws errors
            return new Delta[]{};
        }

        // return the resaults if the turn were to end right then
        public Delta[] GetTurnDeltas(){
            List<Delta> deltas = new List<Delta>();
            // card draws
            foreach(Player p in players){
                foreach(Delta d in p.GetDrawDeltas()){
                    deltas.Add(d);
                }
            }
            // cleanup and return
            return deltas.ToArray();
        }

        public void ApplyDelta(Delta d){
            d.Apply();
        }

    }

}