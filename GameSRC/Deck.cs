using System.Collections.Generic;
using SFB.Game.Decks;

namespace SFB.Game{

    // represents a player's deck of cards while they are playing
    class Deck{

        List<Card> cards; // a list of all the deck

        public Deck(){
            cards = new List<Card>();
        }

        // adds cards in the deck from a decklist
        // TODO: probably would be more efficient to figure out how to load them into random positions
        public void LoadCards(DeckList cards){
            // for each different card in the deck
            foreach(Card c in cards.GetCards()){
                // for each copy of that deck
                int cps = cards.GetCopiesOf(c);
                for(int i = 0; i < cps; i++){
                    // add a copy to the deck
                    this.cards.Add(c);
                }
            }
			Shuffle();
        }

        // randomize the order of cards in the deck
        public void Shuffle(){
			List<int> indexes = new List<int>();
			for(int i = 0; i < cards.Count; i++)
				indexes.Add(i);

			List<int> randList = new List<int>();
			while(indexes.Count > 0)
				randList.Add(indexes[Random.Range(0, indexes.Count - 1)]);

			List<Card> tempCards = new List<int>();
			for(int i = 0; i < cards.Count; i++)
				tempCards.Add(cards[randList[i]]);

			for(int i = 0; i < cards.Count; i++)
				cards[i] = tempCards[i];
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

    }

}