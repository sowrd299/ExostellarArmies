using SFB.Game;
using SFB.Game.Content;
using SFB.Game.Management;
using System.Xml;
using System.Collections.Generic;

namespace SFB.Game
{
    // a class to represent a player in a match
    public class Player
	{
		public const int HAND_SIZE = 3; // CONST HAND SIZE IMPLEMENTED HERE

		public Deck Deck { get; private set; }
		public Hand Hand { get; private set; }
		public Discard Discard { get; private set; }

		public ResourcePool LivesPool { get; private set; }
		public int Lives {
			get { return LivesPool.Count; }
		}

		public ResourcePool ManaPool { get; private set; }
		public int Mana {
			get { return ManaPool.Count; }
		}

		// how many deployment phases the player gets
		public ResourcePool DeployPhasesPool { get; private set; }
		public int DeployPhases {
			get{ return DeployPhasesPool.Count; }
		}

		// optionally takes ids to be used instead of generating new ones
		public Player(DeckList d, XmlElement ids = null)
		{
            //if given id's, manage them
            string deckId = ""; 
            string handId = "";
			string discardId = "";
			string manaId = "";
			string depId = "";
			string livesId = "";

			//if this is passed in, will still generate ID
			if(ids != null)
			{
				deckId = ids.Attributes["deck"].Value;
				handId = ids.Attributes["hand"].Value;
				manaId = ids.Attributes["mana"].Value;
				depId = ids.Attributes["dep"].Value;
				livesId = ids.Attributes["lives"].Value;
				discardId = ids.Attributes["discard"].Value;
			}

			//deck
			this.Deck = new Deck(deckId);
            Deck.LoadCards(d);
			Deck.Shuffle();

            //hand
			this.Hand = new Hand(handId);

            //misc
			this.Discard = new Discard(discardId);
			this.LivesPool = new ResourcePool(4, livesId);
			LivesPool.Add(4);
			this.ManaPool = new ResourcePool(12, manaId); // CONST MAX MANA IMPLEMENTED HERE
			this.DeployPhasesPool = new ResourcePool(2, depId);
		}
 
		public Delta[] GetDrawDeltas(GameManager gm)
		{
			List<Delta> l = new List<Delta>();
			RemoveFromDeckDelta[] rDeltas = this.Deck.GetDrawDeltas(count: HAND_SIZE - this.Hand.Count);
			foreach(Delta d in rDeltas)
				l.Add(d);
			foreach(Delta d in this.Hand.GetDrawDeltas(rDeltas, gm))
				l.Add(d);

			return l.ToArray();
		}

		public Delta[] GetManaDeltas()
		{
			return ManaPool.GetAddDeltas(6 - Lives);
		}

		// get the deltas for when this tower goes down
		public Delta[] GetDeployPhaseDeltas()
		{
			return DeployPhasesPool.GetAddDeltas(1);
		}

		// get the deltas for after have used a deploy phase
		// NOTE: I'm not actually sure this is what we want to here, or just want to skip deltas for this
		public Delta[] GetPostDeployPhaseDeltas()
		{
			return DeployPhasesPool.GetAddDeltas(-1);
		}

        // returns if the player owns the given deck
        internal bool Owns(Deck deck)
		{
            return deck == this.Deck;
        }

		// returns if the player owns the given hand
		internal bool Owns(Hand hand)
		{
			return hand == this.Hand;
		}

		// returns if the player owns the given discard
		internal bool Owns(Discard discard)
		{
			return discard == this.Discard;
		}

		// returns an XML representation of all of the player's objects ID's,
		// to sync them between client/server
		public XmlElement GetPlayerIDs(XmlDocument doc)
		{
            XmlElement e = doc.CreateElement("playerIds");
			// setup the ids to add
			// make arrays so can just itterate them
			IIDed[] elements = new IIDed[]{Deck, Hand, Discard, ManaPool, DeployPhasesPool, LivesPool };
			string[] elementNames = new string[]{"deck", "hand", "discard", "mana", "dep", "lives"};
			// add all the ids
			for(int i = 0; i < elements.Length; i++){
				XmlAttribute idAttr = doc.CreateAttribute(elementNames[i]);
				idAttr.Value = elements[i].ID;
				e.SetAttributeNode(idAttr); 
			}
            // return
            return e;
        }
	}
}