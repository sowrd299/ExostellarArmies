using System.Collections.Generic;
using SFB.Game.Decks;
using UnityEngine;

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
			while(indexes.Count > 0) {
				int idx = indexes[Random.Range(0, indexes.Count - 1)];
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
			string s = "";

			foreach(Card c in cards)
				s += c + "\n";

			return s;
		}

	}

}