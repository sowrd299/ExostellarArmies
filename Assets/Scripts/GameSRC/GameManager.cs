using SFB.Game.Content;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System;

namespace SFB.Game.Management{

    // a class to reprsent the state of the game at a give point in time
    // TODO: one big architecture problme I see, is that deltas and actions get tied to objects w/i the gamestate
    // ...before they are passed into the gamestate; in theory, they should have to go through the gamestate to get those refs;
    public class GameManager{

        // GAME PLAY CONSTANTS
        // the number of lanes in the game;
        const int NumLanes = 3;

        // THE GAMESTATE
        public bool Over {
            get{ // technically shouldn't do o(n) in a field, but it's a very small n
                foreach(Player p in players)
                    if(p.Lives == 0)
                        return true;
                return false;
            }
        }

        private Player[] players;
        // an array of all the players
        // MUST BE IN THE SAME ORDER AS THEIR DECKLISTS/IDS WERE PROVIDED
        public Player[] Players{
            get{
                return players;
            }
        }

        private Lane[] lanes;
        // an array of all the lanes
        public Lane[] Lanes{
            get{
                return lanes;
            }
        }

        // when hidden list is null, will init players with "hidden decks" of unkown cards
        //      functionality intended for clients
        // when ids is null, will init with newly generated ids
        //      functionality intended for servers
        // when both are null, will start a game with no players
        // MUST KEEP THE INDEXES OF THE OBJECTS GIVEN TO IT
        public GameManager(DeckList[] deckLists = null, XmlElement[] playerIds = null, XmlElement[] laneIds = null){
            // setup players
            int numPlayers = deckLists != null ? deckLists.Length : (playerIds != null ? playerIds.Length : 0);
            players = new Player[numPlayers];
            for(int i = 0; i < numPlayers; i++){
                DeckList hiddenList =  new DeckList();
                hiddenList.AddCard(new UnknownCard(), 20); // TODO: support decks of different sizes?
                DeckList list = deckLists != null ? deckLists[i] : hiddenList;
                players[i] = new Player("Player " + (i+1), i, list, playerIds != null ? playerIds[i] : null);
            }
            // setup lanes
            if(laneIds == null){ // ...from scratch
                lanes = new Lane[NumLanes];
                for(int i = 0; i < NumLanes; i++){
                    lanes[i] = new Lane();
                }
            }else{ // ...with pre-determined ids
                lanes = new Lane[laneIds.Length];
                for(int i = 0; i < laneIds.Length; i++){
                    int j = Int32.Parse(laneIds[i].Attributes["index"].Value);
                    lanes[j] = new Lane(laneIds[i].Attributes["id"].Value);
                }
            }
        }

        // returns if a move is legal for the given player to make
        public bool IsLegalAction(Player player, PlayerAction a){
            return a.IsLegalAction(player);
        }

        // returns the outcomes of a player taking a give action
        public Delta[] GetActionDeltas(Player player, PlayerAction a){
            // and old dummy implementation:
            // return new Delta[]{new Deck.RemoveFromDeckDelta(player.Deck, null, 0)}; // this implementation intrinsically throws errors
            // real implementation:
            return a.GetDeltas(player);
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

        // returns and XML representation of the ID of the lane in each index
        public XmlElement[] GetLaneIDs(XmlDocument doc){
            XmlElement[] r = new XmlElement[lanes.Length];
            for(int i = 0; i < lanes.Length; i++){
                r[i] = doc.CreateElement("laneIds");
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = lanes[i].ID;
                r[i].SetAttributeNode(id);
                XmlAttribute index = doc.CreateAttribute("index");
                index.Value = i.ToString();
                r[i].SetAttributeNode(index);
            }
            return r;
        }

		public void cleanUp() {
			foreach(Lane l in lanes) {
				// clean units
				foreach(Unit u in l.Units)
					if(u != null && u.HealthPoints <= 0)
						l.kill(u);

				// clean towers -> player lives
				for(int i = 0; i < l.Towers.Length; i++) {
					if(l.Towers[i].HP == 0) {
						players[i].takeDamage();
						l.Towers[i].revive();
						// extra deploy phase?
					}
				}
			}
		}

    }

}