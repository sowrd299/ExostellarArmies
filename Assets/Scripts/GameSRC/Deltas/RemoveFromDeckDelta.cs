using SFB.Game.Content;
using System.Xml;

namespace SFB.Game
{
	// a class to represent removing the given card from the given index the give card from the given index
	public class RemoveFromDeckDelta : CardListDelta<Deck> {
		public RemoveFromDeckDelta(Deck deck, Card card, int index)
			: base(deck, card, index, CardListDelta<Deck>.DeltaMode.REMOVE)
		{ }

		public RemoveFromDeckDelta(XmlElement element, CardLoader loader)
			: base(element, Deck.IdIssuer, loader)
		{ }


		//TODO: this code can be generalized further
		public override bool VisibleTo(Player p) {
			return p.Owns(Target as Deck);
		}
	}
}