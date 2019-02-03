﻿using SFB.Game;
using SFB.Game.Decks;
using SFB.Game.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {
	public GameManager gameManager;
	Phase phase;
	Player chosen;
	bool hasPrinted;
	Player selectedPlayer;

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


		gameManager = new GameManager(deckLists);
		phase = Phase.DRAW;
	}
	
	void Update() {
		switch(phase) {
			case Phase.DRAW:
				print("");

				foreach(Player p in gameManager.Players)
					p.DrawCards();				
					
				phase = Phase.PLACEMENT;
				
				chosen = null;
				hasPrinted = false;
				selectedPlayer = null;

				break;
			case Phase.PLACEMENT:
				if(!hasPrinted && selectedPlayer == null) {
					foreach(Player p in gameManager.Players)
						print(p + " has " + p.Hand + " and " + p.Deck.Count + " cards in deck.");
					print("Press to select player: (1. " + gameManager.Players[0] + ") / (2. " + gameManager.Players[1] + ") / (0. when you're done)");
					hasPrinted = true;
				} else if(!hasPrinted && selectedPlayer != null) {
					string s = selectedPlayer + " can use ";
					char[] inputs = new char[] {'Q', 'W', 'E', 'R'};
					for(int i = 0; i < selectedPlayer.Hand.Count; i++)
						s += "(" + inputs[i] + ". " + selectedPlayer.Hand[i] + ") ";
					print(s + "(P. deselect player)");
					hasPrinted = true;
				}

				if(selectedPlayer == null) {
					if(Input.GetKeyDown(KeyCode.Alpha1)) {
						selectedPlayer = gameManager.Players[0];
						hasPrinted = false;
					} else if(Input.GetKeyDown(KeyCode.Alpha2)) {
						selectedPlayer = gameManager.Players[1];
						hasPrinted = false;
					} else if(Input.GetKeyDown(KeyCode.Alpha0)) {
						phase = Phase.RANGED;
					}
				} else {
					if(selectedPlayer.Hand.Count > 0 && Input.GetKeyDown(KeyCode.Q)) {
						selectedPlayer.UseCard(0);
						hasPrinted = false;
					} else if(selectedPlayer.Hand.Count > 1 && Input.GetKeyDown(KeyCode.W)) {
						selectedPlayer.UseCard(1);
						hasPrinted = false;
					} else if(selectedPlayer.Hand.Count > 2 && Input.GetKeyDown(KeyCode.E)) {
						selectedPlayer.UseCard(2);
						hasPrinted = false;
					} else if(selectedPlayer.Hand.Count > 3 && Input.GetKeyDown(KeyCode.R)) {
						selectedPlayer.UseCard(3);
						hasPrinted = false;
					}
					if(Input.GetKeyDown(KeyCode.P)) {
						selectedPlayer = null;
						hasPrinted = false;
					}
				}

				break;
			case Phase.RANGED:
				phase = Phase.MELEE;
				break;
			case Phase.MELEE:
				phase = Phase.DRAW;
				break;
		}
	}

}

enum Phase {
	DRAW, PLACEMENT, RANGED, MELEE
}