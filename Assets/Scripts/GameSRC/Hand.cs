using System.Collections.Generic;
using SFB.Game.Decks;

namespace SFB.Game {

	public class Hand : List<Card> {
		internal void DrawFrom(Deck d) {
			this.Add(d.DrawCard());
		}

		public override string ToString() {
			string s = "Hand(";
			for(int i = 0; i < this.Count; i++)
				s += this[i] + (i < this.Count-1 ? " " : "");
			return s+")";
		}
	}
}