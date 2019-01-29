using SFB.Game.Decks;

namespace SFB.Game{

    // a class to represent a player in a match
    public class Player{

        // the player's deck is stored here
        private Deck deck;
        internal Deck Deck{
            get { return deck; }
        }

        internal Player(DeckList d){
            this.deck = new Deck();
            deck.LoadCards(d);
        }

        // returns if the player owns the given deck
        internal bool Owns(Deck deck){
            return deck == this.deck;
        }

    }

}