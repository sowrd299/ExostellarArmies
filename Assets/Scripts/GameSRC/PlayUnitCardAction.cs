using System.Collections;
using System.Collections.Generic;
using SFB.Game.Content;

namespace SFB.Game.Management {
	public class PlayUnitCardAction : PlayerAction {
		private UnitCard card;
		private Lane lane;
		private int pos;
		private int play;

		internal PlayUnitCardAction(UnitCard c, Lane l, int play, int pos) {
			this.card = c;
			this.lane = l;
			this.pos = pos;
			this.play = play;
		}

		internal override bool IsLegalAction(Player p) {
         return p.Hand.Contains(card) &&
					!lane.isOccupied(play, pos) &&
					p.Mana.CanAfford(card.DeployCost);
		}
		
		internal override Delta[] GetDeltas(Player p) {
			return new Delta[] {
				p.Hand.GetRemoveDelta(card)[0],
				p.Mana.GetAddDeltas(card.DeployCost)[0],
				lane.getDeployDeltas(card, play, pos)[0]
			};
		}
	}
}