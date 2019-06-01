using System.Xml;
using SFB.Game.Management;

namespace SFB.Game.Content
{
	public class Hand : CardList
	{
		private static IdIssuer<Hand> idIssuer = new IdIssuer<Hand>();
		public static IdIssuer<Hand> IdIssuer {
			get { return idIssuer; }
		}

		private readonly string id;
		public override string ID {
			get { return id; }
		}

		public Hand(string id = "")
		{
			if(id == "") {
				this.id = IdIssuer.IssueId(this);
			} else {
				IdIssuer.RegisterId(id, this);
				this.id = id;
			}
		}

		public override string ToString()
		{
			string s = "Hand(";
			for(int i = 0; i < this.Count; i++)
				s += this[i] + (i < this.Count-1 ? " " : "");
			return s+")";
		}

		public AddToHandDelta[] GetDrawDeltas(RemoveFromDeckDelta[] rDeltas, GameManager gm)
		{
			AddToHandDelta[] a = new AddToHandDelta[rDeltas.Length];
			for(int i = 0; i < a.Length; i++) {
				a[i] = new AddToHandDelta(this, rDeltas[i].Card);
				// remove index is 0 because assumes all cards above will have been drawn at that point
			}
			return a;
		}

		public Delta[] GetDrawDeltas(Card c, GameManager gm)
		{
			return new Delta[] { new AddToHandDelta(this, c) };
		}

		public Delta[] GetRemoveDelta(Card c)
		{
			return new Delta[] { new RemoveFromHandDelta(this, c, this.IndexOf(c)) };
		}
	}
}