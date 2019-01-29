using SFB.Game;
using SFB.Game.Decks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {
		DeckList dl = new DeckList();
		dl.AddCard(new Card(1, "test1", Faction.NONE), 1);
		dl.AddCard(new Card(1, "test2", Faction.NONE), 2);
		dl.AddCard(new Card(1, "test3", Faction.NONE), 1);
		dl.AddCard(new Card(1, "test4", Faction.NONE), 1);
		dl.AddCard(new Card(1, "test5", Faction.NONE), 3);
		dl.AddCard(new Card(1, "test6", Faction.NONE), 1);
		Deck d = new Deck();
		d.LoadCards(dl);
		d.Shuffle();
		print(d);

		Hand h = new Hand();
		h.DrawFrom(d);
		print(h);
		print(d);
		h.DrawFrom(d);
		print(h);
		print(d);
	}

	// Update is called once per frame
	void Update() {

	}
}