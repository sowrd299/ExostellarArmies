using System.Collections.Generic;
using SFB.Game.Decks;

namespace SFB.Game {

	public class Hand {
		private List<Card> cards;
		internal List<Card> Cards {
			get { return cards; }
		}

		public Hand() {
			cards = new List<Card>();
		}

		internal void DrawFrom(Deck d) {
			cards.Add(d.DrawCard());
		}

		public override string ToString() {
			string s = "Hand(";
			for(int i = 0; i < cards.Count; i++)
				s += cards[i] + (i < cards.Count-1 ? " " : "");
			return s+")";
		}
	}
}