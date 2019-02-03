using System.Collections.Generic;

namespace SFB.Game {

	public class Hand : CardList {

		public override string ID{
			get{ return "0"; }
		}

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