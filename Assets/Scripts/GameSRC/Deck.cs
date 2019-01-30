using System.Collections.Generic;
using System.Xml;
using SFB.Game.Decks;
using SFB.Game.Management;

namespace SFB.Game{

    // represents a player's deck of cards while they are playing
    class Deck : IIDed {

        // from whence all player id's at issued; static to avoid repeats
        private static IdIssuer<Deck> idIssuer = new IdIssuer<Deck>();
        public static IdIssuer<Deck> IdIssuer{
            get { return idIssuer; }
        }

        private readonly string id;
        public string ID{
            get { return id; }
        }

        List<Card> cards; // a list of all the deck

        public Deck(){
            cards = new List<Card>();
            id = idIssuer.IssueId(this);
        }

		// shouldnt be a method tbh
		public int Count() {
			return cards.Count;
		}

        // adds cards in the deck from a decklist
        // TODO: probably would be more efficient to figure out how to load them into random positions
        public void LoadCards(DeckList cards){
			System.Random rand = new System.Random();
			// for each different card in the deck
			foreach(Card c in cards.GetCards()){
                // for each copy of that deck
                int cps = cards.GetCopiesOf(c);
                for(int i = 0; i < cps; i++){
                    // add a copy to the deck
                    this.cards.Insert(rand.Next(0, this.cards.Count), c);
                }
            }
        }

        // randomize the order of cards in the deck
        public void Shuffle(){
			List<int> indexes = new List<int>();
			for(int i = 0; i < cards.Count; i++)
				indexes.Add(i);

			System.Random rand = new System.Random();
			List<int> randList = new List<int>();
			while(indexes.Count > 0) {
				int idx = indexes[rand.Next(0, indexes.Count)];
				indexes.Remove(idx);
				randList.Add(idx);
			}

			List<Card> tempCards = new List<Card>();
			for(int i = 0; i < cards.Count; i++)
				tempCards.Add(cards[randList[i]]);

			for(int i = 0; i < cards.Count; i++)
				cards[i] = tempCards[i];
        }

		public Card DrawCard() {
			Card c = cards[0];
			cards.Remove(c);
			return c;
		}


        // returns a delta that removes the top card from the deck
        // adding that card to the hand will need to be implemented elsewhere
        public RemoveFromDeckDelta GetDrawDelta(){
            return new RemoveFromDeckDelta(null, null, this, cards[0], 0);
        }

        // returns the top i cards of the deck 
        // used to calculate card-draw deltas w/o changing the deck
        // TODO: may be better to implement this with producing deltas here
        public Card[] GetTopCards(int i){
            Card[] r = new Card[i];
            for(int j = 0; j < i; j++){
                r[j] = cards[j];
            }
            return r;
        }

		override public string ToString() {
			string s = "Deck(";

			foreach(Card c in cards)
				s += c + " ";

			return s + ")";
		}


        // a class to represent removing the given card from the given index the give card from the given index
        public class RemoveFromDeckDelta : TargetedDelta<Deck> {

            Card card;
            int index;

            public RemoveFromDeckDelta(XmlNode from, IdIssuer<Deck> issuer, Deck deck, Card c, int i)
				: base(from, issuer)
			{
                target = deck;
                card = c;
                index = i;
            }

            public RemoveFromDeckDelta(XmlNode from): base(from, Deck.IdIssuer) { }
            
            public override bool VisibleTo(Player p){
                return p.Owns(target);
            }

            internal override void Apply(){
                // TODO: implement removing card from the Ith position
                // flip out if can't
            }

        }

	}

}