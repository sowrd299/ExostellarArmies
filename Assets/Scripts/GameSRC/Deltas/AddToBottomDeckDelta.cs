using SFB.Game.Content;
using System.Xml;

namespace SFB.Game
{
	public class AddToDeckIndexDelta : CardListDelta<Deck> {
		public AddToDeckIndexDelta(Deck deck, Card card, int index)
			: base(deck, card, index, CardListDelta<Deck>.DeltaMode.ADD)
		{ }

		public AddToDeckIndexDelta(XmlElement element, CardLoader loader)
			: base(element, Deck.IdIssuer, loader)
		{ }

		//TODO: this code can be generalized further
		public override bool VisibleTo(Player p) {
			return true;
		}
	}
}