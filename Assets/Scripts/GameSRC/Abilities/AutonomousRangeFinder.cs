using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Support Carthan: Give this Front Line +3R this turn.

	public class AutonomousRangeFinder : Ability
	{
		private Unit appliedTo;
		private Unit source;

		public AutonomousRangeFinder() : base(-1) {
			appliedTo = null;
			source = null;
		}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += AutonomousRangeFinderAdd;
			gm.AddRecurringDeployDeltas += AutonomousRangeFinderRemove;
			source = u;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			throw new System.Exception("Can't remove this ability");
		}

		public void AutonomousRangeFinderAdd(List<Delta> deltas, GMWithLocation gmLoc)
		{
			Unit front = gmLoc.FrontUnit;
			if(appliedTo == null && gmLoc.Pos == 1 && front != null && front.Card.UnitType.Contains("Carthan")) {
				deltas.Add(new UnitDamageAmountDelta(front, 3, Damage.Type.RANGED, gmLoc.SubjectUnit));
				appliedTo = front;
			}
		}

		public void AutonomousRangeFinderRemove(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(appliedTo != null) {
				deltas.Add(new UnitDamageAmountDelta(appliedTo, -3, Damage.Type.RANGED, source));
				appliedTo = null;
			}
			gmLoc.GameManager.AddRecurringDeployDeltas -= AutonomousRangeFinderRemove;
		}
	}
}