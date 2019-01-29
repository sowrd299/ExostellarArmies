using System.Collections.Generic;
using SFB.Game.Decks;

namespace SFB.Game {

	public class Hand {
		private List<Card> cards;
		private int maxSize;

		public Hand() {
			cards = new List<Card>();
			maxSize = 3;
		}

		public void IndicateMaxHandSize(int n) {
			maxSize = n;
		}

		internal void DrawFrom(Deck d) {
			if(cards.Count < maxSize)
				cards.Add(d.DrawCard());
		}

		public override string ToString() {
			string s = "";
			foreach(Card c in cards)
				s += c + " ";
			return s;
		}
	}
}