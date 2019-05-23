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
    public class GameManager
	{
        // GAME PLAY CONSTANTS

        // THE GAMESTATE
        public bool Over {
            get{ // technically shouldn't do o(n) in a field, but it's a very small n
                foreach(Player p in GameState.Players)
                    if(p.Lives == 0)
                        return true;
                return false;
            }
        }

        public GameState GameState { get; private set; }
		public Player[] Players { get { return GameState.Players; } }
		public Lane[] Lanes { get { return GameState.Lanes; } }

		/*
        // WHERE ALL PENDING PENDING INPUT REQUESTS GO
        private List<InputRequest> pendingInputRequests;
        public InputRequest[] PendingInputRequests{
            get{ return pendingInputRequests.ToArray(); }
        }
		*/

		

        // when hidden list is null, will init players with "hidden decks" of unkown cards
        //      functionality intended for clients
        // when ids is null, will init with newly generated ids
        //      functionality intended for servers
        // when both are null, will start a game with no players
        // MUST KEEP THE INDEXES OF THE OBJECTS GIVEN TO IT
        public GameManager(DeckList[] deckLists = null, XmlElement[] serializedPlayers = null, XmlElement[] serializedLanes = null){
			GameState = new GameState(deckLists, serializedPlayers, serializedLanes);
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
		// add deploy phases
        public Delta[] GetStartTurnDeltas(){
            List<Delta> deltas = new List<Delta>();
            foreach(Player p in GameState.Players){
                foreach(Delta d in p.GetDeployPhaseDeltas()){
                    deltas.Add(d);
                }
            }
            return deltas.ToArray();
        }

        // get Deltas for the start of each deploy phase
        public Delta[] GetStartDeployDeltas(){
			List<Delta> deltas = new List<Delta>();
			foreach(Player p in GameState.Players) {
				foreach(Delta d in p.GetDrawDeltas()) {
					deltas.Add(d);
				}
                deltas.Add(p.ManaPool.GetAddDeltas(6 - p.Lives)[0]);
            }
			return deltas.ToArray();
		}

		public Delta[] GetRangedCombatDeltas() {
			return CombatManager.GetRangedDeltas(GameState.Lanes);
		}

		public Delta[] GetMeleeCombatDeltas() {
			return CombatManager.GetMeleeDeltas(GameState.Lanes);
		}

		public Delta[] GetTowerDamageDeltas() {
			return CombatManager.GetTowerDeltas(GameState.Lanes);
		}


		// returns the outcomes of a player taking a given action
		public Delta[] GetActionDeltas(Player player, PlayerAction a){
            // and old dummy implementation:
            // return new Delta[]{new Deck.RemoveFromDeckDelta(player.Deck, null, 0)}; // this implementation intrinsically throws errors
            // real implementation:
            return a.GetDeltas(player, this.GameState);
        }


        // hand an answered input request
        public Delta[] FinishInputRequest(Player player, InputRequest req){
            if(!req.Made){ // deal with unfinished req; this honestly should probably just error
                return new Delta[]{};
            }
            Delta[] deltas = req.GetDeltas();
            req.Finish();
            return deltas;
        }
		
        // Get deltas for after a deployment phase ends
		// Decrease # of deploy phases, activate deploy effects
        public Delta[] GetEndDeployDeltas()
		{
			// decrease each player's # of deploy phases
            List<Delta> deltas = new List<Delta>();
            foreach(Player p in GameState.Players) {
                foreach(Delta d in p.GetPostDeployPhaseDeltas()){
                    deltas.Add(d);
                }
            }

			// activate deploy affects
			for(int l = 0; l < GameState.Lanes.Length; l++) {
				Lane lane = GameState.Lanes[l];
				
				for(int side = 0; side < lane.Units.GetLength(0); side++) // sideIndex
					for(int pos = 0; pos < lane.Units.GetLength(1); pos++) // front/back row
						if(lane.Units[side, pos] != null) // do not call on empty spaces
							deltas.AddRange(lane.Units[side, pos].OnEachDeployPhase(l, side, pos, GameState));
			}
			GameState.UseAddRecurringDeployDeltas(deltas);

            return deltas.ToArray();
        }

        // returns whether or there are deploy phases to continue doing
        // if returns true, continue to combat
        // if not, go back to GetStartDeployDeltas...
        public bool DeployPhasesOver()
		{
            bool r = true;
            foreach(Player p in GameState.Players) {
                r &= p.DeployPhases <= 0;
            }
            return r;
        }

		// VARIOUS ADMIN METHODS

		// returns and XML representation of the ID of the lane in each index
		public XmlElement[] GetLaneIDs(XmlDocument doc)
		{
			return GameState.Lanes.Select((lane, index) => lane.ToXml(doc, index)).ToArray();
		}

		// to be called after every phase
		public Delta[] GetCleanUpDeltas()
		{
			List<Delta> deltas = new List<Delta>();

			// clean units
			for(int l = 0; l < GameState.Lanes.Length; l++) {
				Lane lane = GameState.Lanes[l];
				for(int side = 0; side < lane.Units.GetLength(0); side++)
					for(int pos = 0; pos < lane.Units.GetLength(1); pos++) {
						Unit u = lane.Units[side, pos];
						if(u != null && u.HealthPoints <= 0) {
							deltas.AddRange(u.OnDeath(l, side, pos, GameState));
							deltas.AddRange(lane.GetDeathDeltas(side, pos));
							deltas.AddRange(Players[side].Discard.GetDiscardDeltas(new Card[] {u.Card}));
						}
					}
			}
			GameState.UseAddDeathDeltas(deltas);

			// fill front if empty
			foreach(Lane lane in GameState.Lanes) {
				for(int side = 0; side < lane.Units.GetLength(0); side++)
					if(lane.NeedFillFront(side))
						deltas.AddRange(lane.GetInLaneSwapDeltas(side));
			}

			GameState.UseAddBoardUpdateDeltas(deltas);

			// clean towers -> player lives
			for(int l = 0; l < GameState.Lanes.Length; l++) {
				Lane lane = GameState.Lanes[l];
				for(int i = 0; i < lane.Towers.Length; i++) {
					if(lane.Towers[i].HP <= 0) {
						deltas.AddRange(GameState.Players[i].LivesPool.GetSubtractDeltas(1));
						deltas.AddRange(lane.Towers[i].GetReviveDeltas());
						deltas.AddRange(GameState.Players[i].DeployPhasesPool.GetAddDeltas(1));
					}
				}
			}

			return deltas.ToArray();
		}
    }
}