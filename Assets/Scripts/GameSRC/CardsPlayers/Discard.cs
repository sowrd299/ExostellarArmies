using System;
using System.Collections.Generic;
using SFB.Game.Content;
using SFB.Game.Management;

namespace SFB.Game
{
    // represents a player's deck of cards while they are playing
    public class Discard : CardList
	{
        // from when all player id's at issued; static to avoid repeats
        private static IdIssuer<Discard> idIssuer = new IdIssuer<Discard>();
        public static IdIssuer<Discard> IdIssuer {
            get { return idIssuer; }
        }

        private readonly string id;
        public override string ID {
            get { return id; }
        }
		
        public Discard(string id = "")
		{
            if(id == "") {
                this.id = idIssuer.IssueId(this);
            } else {
                idIssuer.RegisterId(id, this);
                this.id = id;
            }
        }

		public AddToDiscardDelta[] GetDiscardDeltas(Card[] cards)
		{
			AddToDiscardDelta[] deltas = new AddToDiscardDelta[cards.Length];
			for(int i = 0; i < cards.Length; i++) {
				deltas[i] = new AddToDiscardDelta(this, cards[i]);
			}
			return deltas;
		}
	}
}