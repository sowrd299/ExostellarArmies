using System.Collections.Generic;

namespace SFB.Game.Decks{

    // a class to contain the non-unique set (bag) of cards in a deck
    public class DeckList {

        // stores what card: how many copies
        private Dictionary<Card, int> cards;

        public DeckList(){
            cards = new Dictionary<Card, int>();
        }

        // returns how many copies of a given card are in the list
        public int GetCopiesOf(Card c){
            if(cards.ContainsKey(c)){
                return cards[c];
            }
            return 0;
        }

        // returns all of the different cards in the deck
        // does not specify how many of each card there are
        // TODO: this a super inefficient way to implement this
        public Card[] GetCards(){
            Card[] r = new Card[cards.Count];
            cards.Keys.CopyTo(r, 0);
            return r;
        }

    }

}