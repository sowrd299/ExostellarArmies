using SFB.Game.Management;
using System.Collections.Generic;
using System;

namespace SFB.Game
{
	// Front Line Recurring Deploy: If there is not a unit behind this, heal each adjacent allied front line unit 1.

	public class SphereMedicalVanguard : Ability
	{
		public SphereMedicalVanguard() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas += SphereMedicalVanguardInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas -= SphereMedicalVanguardInner;
		}

		public void SphereMedicalVanguardInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.Pos == 0 && gmLoc.BackUnit == null) {
				int lane = gmLoc.Lane;
				int side = gmLoc.Side;

				Unit leftLaneFront = gmLoc.LeftLane?.Units?[side, 0];
				if(leftLaneFront != null)
					deltas.AddRange(
						UnitHealthDelta.GetHealDeltas(
							leftLaneFront,
							gmLoc.SubjectUnit,
							1,
							gmLoc.GameManager
						)
					);

				Unit rightLaneFront = gmLoc.RightLane?.Units?[side, 0];
				if(rightLaneFront != null)
					deltas.AddRange(
						UnitHealthDelta.GetHealDeltas(
							rightLaneFront,
							gmLoc.SubjectUnit,
							1,
							gmLoc.GameManager
						)
					);
			}
		}
	}
}