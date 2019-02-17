using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using System.Xml;
using System.Collections.Generic;

namespace SFB.Game{

    // a class to represent a player in a match
    public class Player {
		private int lives;
		internal int Lives {
			get { return lives; }
		}

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

		// TODO: what does this do?
		private int num;
		public int Num {
			get { return num; }
		}

		private ResourcePool mana;
		public ResourcePool Mana {
			get { return mana; }
		}

		// how many deployment phases the player gets
		private ResourcePool deployPhases;
		public int DeployPhases{
			get{ return deployPhases.Count; }
		}

		private List<Card> discard;
		internal List<Card> Discard {
			get { return discard; }
		}

        // optionally takes ids to be used instead of generating new ones
		internal Player(string name, int n, DeckList d, XmlElement ids = null){
            //if given id's, manage them
            string deckId = ""; //if this is passed in, will still generate ID
            string handId = ""; //if this is passed in, will still generate ID
			string manaId = "";
			string depId = "";
            if(ids != null){
                deckId = ids.Attributes["deck"].Value;
                handId = ids.Attributes["hand"].Value;
                manaId= ids.Attributes["mana"].Value;
                depId= ids.Attributes["dep"].Value;
            }
            //deck
			this.deck = new Deck(deckId);
            deck.LoadCards(d);
            //hand
			this.hand = new Hand(handId);
			this.handSize = 3; // CONST HAND SIZE IMPLEMENTED HERE
            //misc
			this.name = name;
			this.discard = new List<Card>();
			this.lives = 4; // CONST LIVES IMPLEMENETED HERE
			this.num = n;
			this.mana = new ResourcePool(12, manaId); // CONST MAX RESOURCES IMPLEMENTED HERE
			this.deployPhases = new ResourcePool(2, depId);
		}

		public void takeDamage() {
			lives--;
		}

 
		public Delta[] GetDrawDeltas() {
			List<Delta> l = new List<Delta>();

			Deck.RemoveFromDeckDelta[] rDeltas = this.deck.GetDrawDeltas(count: this.handSize - this.hand.Count);
			foreach(Delta d in rDeltas)
				l.Add(d);
			foreach(Delta d in this.hand.GetDrawDeltas(rDeltas))
				l.Add(d);

			return l.ToArray();
		}

		// get the deltas for when this tower goes down
		public Delta[] GetDeployPhaseDeltas(){
			return deployPhases.GetAddDeltas(1);
		}

		public void AddDeployPhase(){
			deployPhases.Add(1);
		}

		// get the deltas for after have used a deploy phase
		// NOTE: I'm not actually sure this is what we want to here, or just want to skip deltas for this
		public Delta[] GetPostDeployPhaseDeltas(){
			return deployPhases.GetAddDeltas(-1);
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
			// setup the ids to add
			// make arrays so can just itterate them
			IIDed[] elements = new IIDed[]{deck, hand, mana, deployPhases};
			string[] elementNames = new string[]{"deck", "hand", "mana", "dep"};
			// add all the ids
			for(int i = 0; i < elements.Length; i++){
				XmlAttribute idAttr = doc.CreateAttribute(elementNames[i]);
				idAttr.Value = elements[i].ID;
				e.SetAttributeNode(idAttr); 
			}
            // return
            return e;
        }

		public override string ToString() {
			return name;
		}
	}

}