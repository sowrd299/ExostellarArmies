using SFB.Game.Decks;
using System.Xml;
using System.Collections.Generic;

namespace SFB.Game{

    // a class to represent a player in a match
    public class Player{

        // the player's deck is stored here
        private Deck deck;
        internal Deck Deck{
            get { return deck; }
        }

		private Hand hand;
		internal Hand Hand {
			get { return hand; }
		}

		private int handSize;
		internal int HandSize {
			get { return handSize; }
		}

		private string name;
		internal string Name {
			get { return name; }
		}

		private List<Card> discard;
		internal List<Card> Discard {
			get { return discard; }
		}

		internal Player(string name, DeckList d){
			this.deck = new Deck();
            deck.LoadCards(d);

			this.hand = new Hand();
			this.handSize = 3;

			this.name = name;

			this.discard = new List<Card>();
		}

		internal void UseCard(int i) {
			discard.Add(this.hand.Cards[i]);
			this.hand.Cards.RemoveAt(i);
		}

		internal void DrawCards() {
			while(this.hand.Cards.Count < this.HandSize)
				this.hand.DrawFrom(this.deck);
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
            return e;
        }

		public override string ToString() {
			return name;
		}
	}

}