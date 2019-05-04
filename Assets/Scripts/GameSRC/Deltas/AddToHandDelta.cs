using SFB.Game.Content;
using System.Xml;

namespace SFB.Game
{
	public class AddToHandDelta : CardListDelta<Hand> {
		public AddToHandDelta(Hand hand, Card card)
			: base(hand, card, 0, CardListDelta<Hand>.DeltaMode.ADD)
		{ }

		public AddToHandDelta(XmlElement element, CardLoader loader)
			: base(element, Hand.IdIssuer, loader)
		{ }

		//TODO: this code can be generalized further
		public override bool VisibleTo(Player p) {
			return p.Owns(Target as Hand);
		}
	}
}