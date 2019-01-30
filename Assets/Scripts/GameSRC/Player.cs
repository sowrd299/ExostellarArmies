using SFB.Game.Decks;
using System.Xml;

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
        
        // returns an XML representation of all of the player's objects ID's,
        // to sync them between client/server
        public XmlElement GetPlayerIDs(XmlDocument doc){
            XmlElement e = doc.CreateElement("playerIds");
            // the deck id
            XmlAttribute deckId = doc.CreateAttribute("deck");
            deckId.Value = deck.ID;
            e.SetAttributeNode(deckId); 
        }

    }

}