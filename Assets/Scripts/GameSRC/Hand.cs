using System.Collections.Generic;
using System.Xml;
using SFB.Game.Content;
using SFB.Game.Management;

namespace SFB.Game.Content {

	public class Hand : CardList {
		private static IdIssuer<Hand> idIssuer = new IdIssuer<Hand>();
		public static IdIssuer<Hand> IdIssuer {
			get { return idIssuer; }
		}

		private readonly string id;
		public override string ID {
			get { return id; }
		}

		internal void DrawFrom(Deck d) {
			this.Add(d.DrawCard());
		}

		public Hand(string id = "") {
			if(id == "") {
				this.id = idIssuer.IssueId(this);
			} else {
				idIssuer.RegisterId(id, this);
				this.id = id;
			}
		}

		public override string ToString() {
			string s = "Hand(";
			for(int i = 0; i < this.Count; i++)
				s += this[i] + (i < this.Count-1 ? " " : "");
			return s+")";
		}

		internal AddToHandDelta[] GetDrawDeltas(Deck.RemoveFromDeckDelta[] rDeltas) {
			AddToHandDelta[] a = new AddToHandDelta[rDeltas.Length];
			for(int i = 0; i < a.Length; i++) {
				a[i] = new AddToHandDelta(this, rDeltas[i].Card);
				// remove index is 0 because assumes all cards above will have been drawn at that point
			}
			return a;
		}

		public class AddToHandDelta : CardListDelta<Hand> {

			public AddToHandDelta(Hand hand, Card card) : base(hand, card, 0, CardListDelta<Hand>.Mode.ADD) { }

			public AddToHandDelta(XmlElement element, CardLoader loader) : base(element, Hand.IdIssuer, loader) { }


			//TODO: this code can be generalized further
			public override bool VisibleTo(Player p) {
				return p.Owns(target as Hand);
			}

		}



		public class RemoveFromHandDelta : CardListDelta<Hand> {

			public RemoveFromHandDelta(Hand hand, Card card, int index) : base(hand, card, index, CardListDelta<Hand>.Mode.REMOVE) { }

			public RemoveFromHandDelta(XmlElement element, CardLoader loader) : base(element, Hand.IdIssuer, loader) { }


			//TODO: this code can be generalized further
			public override bool VisibleTo(Player p) {
				return p.Owns(target as Hand);
			}

		}
	}
}