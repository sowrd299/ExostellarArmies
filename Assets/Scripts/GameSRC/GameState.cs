using SFB.Game.Content;
using System.Xml;
using System;
using System.Collections.Generic;

namespace SFB.Game.Management
{
	public class GameState
	{
		// the number of lanes in the game;
		public const int NUM_LANES = 3;

		// an array of all the players
		// MUST BE IN THE SAME ORDER AS THEIR DECKLISTS/IDS WERE PROVIDED
		public Player[] Players { get; private set; }
		
		// an array of all the lanes
		public Lane[] Lanes { get; private set; }

		public event Ability.AddDelta AddBoardUpdateDeltas;

		public void UseAddBoardUpdateDeltas(List<Delta> deltas) {
			AddBoardUpdateDeltas?.Invoke(deltas, null);
		}

		public GameState(DeckList[] deckLists, XmlElement[] serializedPlayers, XmlElement[] serializedLanes)
		{
			// setup players
			int numPlayers = deckLists != null ? deckLists.Length : (serializedPlayers != null ? serializedPlayers.Length : 0);
			Players = new Player[numPlayers];
			for(int i = 0; i < numPlayers; i++) {
				DeckList hiddenList = new DeckList();
				hiddenList.AddCard(new UnknownCard(), 20); // TODO: support decks of different sizes?
				DeckList list = deckLists != null ? deckLists[i] : hiddenList;

				Players[i] = new Player(list, serializedPlayers?[i]);
			}
			// setup lanes
			if(serializedLanes == null) { // ...from scratch
				Lanes = new Lane[NUM_LANES];
				for(int i = 0; i < NUM_LANES; i++) {
					Lanes[i] = new Lane();
				}
			} else { // ...with pre-determined ids
				Lanes = new Lane[serializedLanes.Length];
				for(int i = 0; i < serializedLanes.Length; i++) {
					int j = Int32.Parse(serializedLanes[i].Attributes["index"].Value);
					Lanes[j] = new Lane(serializedLanes[i]);
				}
			}
		}

		public int[] GetLaneSidePosOf(Unit u) {
			for(int l = 0; l < Lanes.Length; l++)
				for(int side = 0; side < 2; side++)
					for(int pos = 0; pos < 2; pos++)
						if(Lanes[l].Units[side, pos] == u)
							return new int[] { l, side, pos };
			return null;
		}

		public GameStateLocation WithLocation(int lane, int side, int pos)
		{
			return new GameStateLocation(this, lane, side, pos);
		}
	}
}