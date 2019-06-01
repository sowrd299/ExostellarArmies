using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class LtMgrTulYorves : Ability
	{
		// Front Line Recurring Deploy: If there is not a unit behind this, heal each adjacent allied front line unit 1.

		public LtMgrTulYorves() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm) {
			u.AddRecurringDeployDeltas += LtMgrTulYorvesInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm) {
			u.AddRecurringDeployDeltas -= LtMgrTulYorvesInner;
		}

		public void LtMgrTulYorvesInner(List<Delta> deltas, GMWithLocation gmLoc) {
			if(gmLoc.Pos == 0 && gmLoc.BackUnit == null) {
				Unit left = gmLoc.LeftLane?.Units?[gmLoc.Side, 0];
				if(left != null)
					deltas.AddRange(
						UnitHealthDelta.GetHealDeltas(
							left,
							gmLoc.SubjectUnit,
							1,
							gmLoc.GameManager
						)
					);

				Unit right = gmLoc.RightLane?.Units?[gmLoc.Side, 0];
				if(right != null)
					deltas.AddRange(
						UnitHealthDelta.GetHealDeltas(
							right,
							gmLoc.SubjectUnit,
							1,
							gmLoc.GameManager
						)
					);
			}

			
		}
	}
}
