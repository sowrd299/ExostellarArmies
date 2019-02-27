using SFB.Game.Content;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System;
using System.Linq;

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
                players[i] = new Player("Player " + (i+1), list, playerIds != null ? playerIds[i] : null);
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

        public void ApplyDelta(Delta d){
            d.Apply();
        }

        // returns if a move is legal for the given player to make
        public bool IsLegalAction(Player player, PlayerAction a){
            // the action is a legal action for them to play, 
            // and they may still play actions
            return a.IsLegalAction(player) && player.DeployPhases > 0;
        }

        // THE FOLLOWING METHODS IMPLEMENT IN THE GAME LOOP
        // EACH RETURNING THE DELTAS FROM EACH STEP OF THE TURN
        //   (to be applied, sent, etc. before calling the next turn's)
        // You may call them out of order; this will simulate skipping all the phases
        //   whose deltas haven't been applied yet

        // get deltas for the absolute start of the turn
        public Delta[] GetStartTurnDeltas(){
            List<Delta> deltas = new List<Delta>();
            foreach(Player p in players){
                foreach(Delta d in p.GetDeployPhaseDeltas()){
                    deltas.Add(d);
                }
            }
            return deltas.ToArray();
        }

        // get Deltas for the start of each deploy phase
        public Delta[] GetStartDeployDeltas(){
			List<Delta> deltas = new List<Delta>();
			foreach(Player p in players) {
				foreach(Delta d in p.GetDrawDeltas()) {
					deltas.Add(d);
				}
			}
			return deltas.ToArray();
		}

		public Delta[] GetRangedCombatDeltas() {
			return CombatManager.getRangedDeltas(lanes);
		}

		public Delta[] GetMeleeCombatDeltas() {
			return CombatManager.getMeleeDeltas(lanes);
		}

		// returns the outcomes of a player taking a given action
		public Delta[] GetActionDeltas(Player player, PlayerAction a){
            // and old dummy implementation:
            // return new Delta[]{new Deck.RemoveFromDeckDelta(player.Deck, null, 0)}; // this implementation intrinsically throws errors
            // real implementation:
            return a.GetDeltas(player);
        }

        // Get deltas for after a deployment phase ends
        public Delta[] GetEndDeployDeltas(){
            List<Delta> deltas = new List<Delta>();
            foreach(Player p in players){
                foreach(Delta d in p.GetPostDeployPhaseDeltas()){
                    deltas.Add(d);
                }
            }

			// TODO: activate deploy affects here, probably
			foreach(Lane l in lanes)
				for(int play = 0; play < l.Units.GetLength(0); play++) // player
					for(int pos = 0; pos < l.Units.GetLength(1); pos++) // front/back row
                        if(l.Units[play, pos] != null) // do not call on empty spaces
                            deltas.AddRange(l.Units[play, pos].onEachDeployPhase(play, lanes, players));
		
            return deltas.ToArray();
        }

        // returns whether or there are deploy phases to continue doing
        // if returns true, continue to combat
        // if not, go back to GetStartDeployDeltas...
        public bool DeployPhasesOver(){
            bool r = true;
            foreach(Player p in players){
                r &= p.DeployPhases <= 0;
            }
            return r;
        }

        // return the results if the turn were to end right then
        // more specifically, returns the combine results of every phase
        //  from the currently ending deployment phase up to the draw phase
        //  before the next deploy phase
        // TODO: is this REALLY the best way to do this...
        // probably should depricate this...
        public Delta[] GetTurnDeltas(){
            List<Delta> deltas = new List<Delta>();
            // card draws; at some point change this over to the draw phase
            // only draw for players who have deployment phases left
            foreach(Player p in players.Where((x) => {return x.DeployPhases > 0;})){ 
                foreach(Delta d in p.GetDrawDeltas()){
                    deltas.Add(d);
                }
            }
            // cleanup and return
            return deltas.ToArray();
        }


		public void DrawPhase() {
			foreach(Player p in players) {
				Delta[] ds = p.GetDrawDeltas();

				//send network??

				foreach(Delta d in ds)
					ApplyDelta(d);
			}
		}

		public void CombatPhase() {
			Delta[] rds = CombatManager.getRangedDeltas(lanes);
			Delta[] mds = CombatManager.getMeleeDeltas(lanes);

			//send network??

			foreach(Delta d in rds)
				ApplyDelta(d);
			cleanUp();

			foreach(Delta d in mds)
				ApplyDelta(d);
			cleanUp();
		}


        // VARIOUS ADMIN METHODS

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

        // to be called after every phase
		public Delta[] cleanUp() {
			List<Delta> deltas = new List<Delta>();
			foreach(Lane l in lanes) {
				// clean units
				for(int play = 0; play < l.Units.GetLength(0); play++)
					for(int pos = 0; pos < l.Units.GetLength(1); pos++) {
						Unit u = l.Units[play, pos];
						if(u != null && u.HealthPoints <= 0) {
							deltas.AddRange(u.onDeath(play, lanes, players));
							l.kill(play, pos);
						}
					}

				// clean towers -> player lives
				for(int i = 0; i < l.Towers.Length; i++) {
					if(l.Towers[i].HP <= 0) {
						players[i].takeDamage();
						l.Towers[i].revive();
                        players[i].AddDeployPhase();
					}
				}
			}
			return deltas.ToArray();
		}

    }

}