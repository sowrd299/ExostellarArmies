using System.Collections;
using System.Collections.Generic;
using SFB.Game.Content;

namespace SFB.Game.Management {
	public class PlayUnitCardAction : PlayerAction {
		private UnitCard card;
		private Lane lane;
		private int pos;
		private int handIndex;

		internal PlayUnitCardAction(UnitCard c, Lane l, int p, int hi) {
			card = c;
			lane = l;
			pos = p;
			handIndex = hi;
		}

		internal override bool IsLegalAction(Player p) {
			return p.Hand[handIndex] == card && !lane.isOccupied(p.Num, pos);
		}
		
		internal override Delta[] GetDeltas(Player p) {
			return new Delta[] {
				new Hand.RemoveFromHandDelta(p.Hand, card, handIndex),
				new Lane.AddToLaneDelta(lane, card, p.Num, pos)
			};
		}
	}
}