using SFB.Game;
using SFB.Game.Decks;
using SFB.Game.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {
	public GameState gameState;
	Phase phase;
	Player chosen;
	bool hasPrintedPlayers;
	bool hasPrintedDetails;

	void Start() {
		List<int> test = new List<int>();

		DeckList dlC = new DeckList();
		dlC.AddCard(new UnitCard(1, "Carth1Cost", Faction.CARTH, 1, 1, 1), 4);
		dlC.AddCard(new UnitCard(2, "Carth2Cost#1", Faction.CARTH, 1, 1, 1), 3);
		dlC.AddCard(new UnitCard(2, "Carth2Cost#2", Faction.CARTH, 1, 1, 1), 3);
		dlC.AddCard(new UnitCard(3, "Carth3Cost", Faction.CARTH, 1, 1, 1), 3);
		dlC.AddCard(new UnitCard(4, "Carth4Cost", Faction.CARTH, 1, 1, 1), 2);
		dlC.AddCard(new UnitCard(5, "Carth5Cost", Faction.CARTH, 1, 1, 1), 1);

		DeckList dlJ = new DeckList();
		dlJ.AddCard(new UnitCard(1, "Jirnor1Cost", Faction.JIRNOR, 1, 1, 1), 4);
		dlJ.AddCard(new UnitCard(2, "Jirnor2Cost#1", Faction.JIRNOR, 1, 1, 1), 3);
		dlJ.AddCard(new UnitCard(2, "Jirnor2Cost#2", Faction.JIRNOR, 1, 1, 1), 3);
		dlJ.AddCard(new UnitCard(3, "Jirnor3Cost", Faction.JIRNOR, 1, 1, 1), 3);
		dlJ.AddCard(new UnitCard(4, "Jirnor4Cost", Faction.JIRNOR, 1, 1, 1), 2);
		dlJ.AddCard(new UnitCard(5, "Jirnor5Cost", Faction.JIRNOR, 1, 1, 1), 1);


		// Each deck has 16 cards in it
		DeckList[] deckLists = new DeckList[] { dlC, dlJ };


		gameState = new GameState(deckLists);
		phase = Phase.DRAW;
	}
	
	void Update() {
		switch(phase) {
			case Phase.DRAW:
				foreach(Player p in gameState.Players) {
					p.DrawCards();
					print(p + " has " + p.Hand + " and " + p.Deck.Count() + " cards in deck.");
				}
					
				phase = Phase.PLACEMENT;
				
				chosen = null;
				hasPrintedPlayers = false;
				hasPrintedDetails = true;

				break;
			case Phase.PLACEMENT:
				if(!hasPrintedPlayers)
					print("Press to act: 1. (" + gameState.Players[0] + ") / 2. (" + gameState.Players[1] + ")");

				break;
			case Phase.RANGED:
				phase = Phase.MELEE;
				break;
			case Phase.MELEE:
				phase = Phase.PLACEMENT;
				break;
		}
	}

}

enum Phase {
	DRAW, PLACEMENT, RANGED, MELEE
}