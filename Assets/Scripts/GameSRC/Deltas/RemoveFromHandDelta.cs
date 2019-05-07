using SFB.Game.Content;
using System.Xml;

namespace SFB.Game	
{
	public class RemoveFromHandDelta : CardListDelta<Hand> {

		public RemoveFromHandDelta(Hand hand, Card card, int index)
			: base(hand, card, index, CardListDelta<Hand>.DeltaMode.REMOVE)
		{ }

		public RemoveFromHandDelta(XmlElement element, CardLoader loader)
			: base(element, Hand.IdIssuer, loader)
		{ }


		//TODO: this code can be generalized further
		public override bool VisibleTo(Player p) {
			return p.Owns(Target as Hand);
		}
	}
}