using System.Collections.Generic;
using System.Xml;
using SFB.Game.Content;
using SFB.Game.Management;

namespace SFB.Game{

    // represents a player's deck of cards while they are playing
    class Deck : CardList {

        // from whence all player id's at issued; static to avoid repeats
        private static IdIssuer<Deck> idIssuer = new IdIssuer<Deck>();
        public static IdIssuer<Deck> IdIssuer{
            get { return idIssuer; }
        }

        private readonly string id;
        public override string ID{
            get { return id; }
        }
		
        public Deck(string id = ""){
            if(id == ""){
                this.id = idIssuer.IssueId(this);
            }else{
                idIssuer.RegisterId(id, this);
                this.id = id;
            }
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
                    this.Insert(rand.Next(0, this.Count), c);
                }
            }
        }

        // randomize the order of cards in the deck
        public void Shuffle(){
            lock(this){
                List<int> indexes = new List<int>();
                for(int i = 0; i < this.Count; i++)
                    indexes.Add(i);

                System.Random rand = new System.Random();
                List<int> randList = new List<int>();
                while(indexes.Count > 0) {
                    int idx = indexes[rand.Next(0, indexes.Count)];
                    indexes.Remove(idx);
                    randList.Add(idx);
                }

                List<Card> tempCards = new List<Card>();
                for(int i = 0; i < this.Count; i++)
                    tempCards.Add(this[randList[i]]);

                for(int i = 0; i < this.Count; i++)
                    this[i] = tempCards[i];
            }
        }

		public Card DrawCard() {
            Card c;
            lock(this){
                c = this[0];
                this.Remove(c);
            }
			return c;
		}


        // returns a delta that removes the top card from the deck
        // adding that card to the hand will need to be implemented elsewhere
        public RemoveFromDeckDelta[] GetDrawDeltas(int startingIndex = 0, int count = 1){
            RemoveFromDeckDelta[] r = new RemoveFromDeckDelta[count - startingIndex];
            for(int i = startingIndex; i < startingIndex+count; i++){
                r[i-startingIndex] = new RemoveFromDeckDelta(this, this[i], 0);
                // remove index is 0 because assumes all cards above will have been drawn at that point
            }
            return r;
        }

        // returns the top i cards of the deck 
        // used to calculate card-draw deltas w/o changing the deck
        // TODO: may be better to implement this with producing deltas here
        public Card[] GetTopCards(int i){
            Card[] r = new Card[i];
            for(int j = 0; j < i; j++){
                r[j] = this[j];
            }
            return r;
        }

		override public string ToString() {
			string s = "Deck(";

			foreach(Card c in this)
				s += c + " ";

			return s + ")";
		}


        // a class to represent removing the given card from the given index the give card from the given index
        public class RemoveFromDeckDelta : CardListDelta<Deck> {

            public RemoveFromDeckDelta(Deck deck, Card card, int index) : base(deck, card, index, CardListDelta<Deck>.Mode.REMOVE) {}

            public RemoveFromDeckDelta(XmlElement element, CardLoader loader) : base(element, Deck.IdIssuer, loader) {}


            //TODO: this code can be generalized further
            public override bool VisibleTo(Player p){
                return p.Owns(target as Deck);
            }

        }

	}

}