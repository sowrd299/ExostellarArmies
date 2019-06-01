using SFB.Game.Content;
using System.Xml;

namespace SFB.Game
{
	public class AddToBottomDeckDelta : CardListDelta<Deck> {
		public AddToBottomDeckDelta(Deck deck, Card card)
			: base(deck, card, deck.Count, CardListDelta<Deck>.DeltaMode.ADD)
		{ }

		public AddToBottomDeckDelta(XmlElement element, CardLoader loader)
			: base(element, Deck.IdIssuer, loader)
		{ }

		//TODO: this code can be generalized further
		public override bool VisibleTo(Player p) {
			return true;
		}
	}
}