using SFB.Game.Management;
using System.Collections.Generic;
using SFB.Game.Content;

namespace SFB.Game
{
	public class KjacRostFTheBloomer : Ability
	{
		public override string GetMainText() {
			return "Deploy: Infantry you control get +1M this turn.";
		}

		private List<Unit> appliedTo;
		private int turn;

		public KjacRostFTheBloomer() : base(-1) {
			appliedTo = new List<Unit>();
			turn = 0;
		}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas += KjacRostFTheBloomerInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			throw new System.Exception("Can't remove this ability - potential bugs");
		}

		public void KjacRostFTheBloomerInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(turn == 0) {
				Unit source = gmLoc.SubjectUnit;
				int side = gmLoc.Side;

				foreach(Lane l in gmLoc.Lanes)
					for(int pos = 0; pos < 2; pos++)
						if(l.IsOccupied(side, pos) && l.Units[side, pos].Card.UnitType.Contains("Infantry")) {
							Unit target = l.Units[side, pos];
							deltas.Add(new UnitDamageAmountDelta(target, 1, Damage.Type.MELEE, source));
							appliedTo.Add(target);
						}
				
			} else if(turn == 1) {
				foreach(Unit u in appliedTo)
					deltas.Add(new UnitDamageAmountDelta(u, -1, Damage.Type.MELEE, gmLoc.SubjectUnit));
				appliedTo.Clear();
			}
			turn++;
		}
	}
}