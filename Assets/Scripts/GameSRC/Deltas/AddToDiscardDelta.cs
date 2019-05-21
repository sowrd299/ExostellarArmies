using SFB.Game.Content;
using System.Xml;

namespace SFB.Game
{
	public class AddToDiscardDelta : CardListDelta<Discard> {
		public AddToDiscardDelta(Discard discard, Card card)
			: base(discard, card, 0, CardListDelta<Discard>.DeltaMode.ADD)
		{ }

		public AddToDiscardDelta(XmlElement element, CardLoader loader)
			: base(element, Discard.IdIssuer, loader)
		{ }

		//TODO: this code can be generalized further
		public override bool VisibleTo(Player p) {
			return p.Owns(Target as Discard);
		}
	}
}