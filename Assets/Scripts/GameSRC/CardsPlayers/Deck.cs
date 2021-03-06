using System;
using System.Collections.Generic;
using SFB.Game.Content;
using SFB.Game.Management;

namespace SFB.Game
{
    // represents a player's deck of cards while they are playing
    public class Deck : CardList
	{
        // from when all player id's at issued; static to avoid repeats
        private static IdIssuer<Deck> idIssuer = new IdIssuer<Deck>();
        public static IdIssuer<Deck> IdIssuer {
            get { return idIssuer; }
        }

        private readonly string id;
        public override string ID {
            get { return id; }
        }
		
        public Deck(string id = "")
		{
            if(id == "") {
                this.id = idIssuer.IssueId(this);
            } else {
                idIssuer.RegisterId(id, this);
                this.id = id;
            }
        }

		// adds cards in the deck from a decklist; inserts at random locations
		public void LoadCards(DeckList cards)
		{
			System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
			// for each different card in the deck
			foreach(Card c in cards.GetCards())
			{
                // for each copy of that deck
                int cps = cards.GetCopiesOf(c);
                for(int i = 0; i < cps; i++)
				{
                    // add a copy to the deck
                    this.Insert(rand.Next(0, this.Count), c);
                }
            }
        }

        // randomize the order of cards in the deck
        public void Shuffle()
		{
            lock(this)
			{
                List<int> indexes = new List<int>();
                for(int i = 0; i < this.Count; i++)
                    indexes.Add(i);

                System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
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

		override public string ToString() {
			string s = "Deck(";

			foreach(Card c in this)
				s += c + " ";

			return s + ")";
		}
	}
}