using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
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

        // optionally takes ids to be used instead of generating new ones
		internal Player(string name, DeckList d, XmlElement ids = null){
            //if given id's, manage them
            string deckId = ""; //if this is passed in, will still generate ID
            if(ids != null){
                deckId = ids.Attributes["deckId"].Value;
            }
            //deck
			this.deck = new Deck(deckId);
            deck.LoadCards(d);
            //hand
			this.hand = new Hand();
			this.handSize = 3;
            //misc
			this.name = name;

			this.discard = new List<Card>();
		}

		internal void UseCard(int i) {
			discard.Add(this.hand[i]);
			this.hand.RemoveAt(i);
		}

 
		public List<Delta> GetDrawDeltas() {
			List<Delta> l = new List<Delta>();

			Deck.RemoveFromDeckDelta[] rDeltas = this.deck.GetDrawDeltas(count: this.handSize - this.hand.Count);
			foreach(Delta d in rDeltas)
				l.Add(d);
			foreach(Delta d in this.hand.GetDrawDeltas(rDeltas))
				l.Add(d);

			return l;
		}

		internal void DrawCards() {
			while(this.hand.Count < this.HandSize)
				this.hand.DrawFrom(this.deck);
		}

        // returns if the player owns the given deck
        internal bool Owns(Deck deck){
            return deck == this.deck;
        }

		// returns if the player owns the given hand
		internal bool Owns(Hand hand) {
			return hand == this.hand;
		}

		// returns an XML representation of all of the player's objects ID's,
		// to sync them between client/server
		public XmlElement GetPlayerIDs(XmlDocument doc){
            XmlElement e = doc.CreateElement("playerIds");
            // the deck id
            XmlAttribute deckId = doc.CreateAttribute("deck");
            deckId.Value = deck.ID;
            e.SetAttributeNode(deckId); 
            // the hand id
            XmlAttribute handId = doc.CreateAttribute("hand");
            handId.Value = hand.ID;
            e.SetAttributeNode(handId); 
            // return
            return e;
        }

		public override string ToString() {
			return name;
		}
	}

}