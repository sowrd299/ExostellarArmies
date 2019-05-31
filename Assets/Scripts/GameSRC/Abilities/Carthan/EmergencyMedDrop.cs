using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Deploy: Heal this Front Line 2 and each adjacent Front Line 1.

	public class EmergencyMedDrop : Ability
	{
		public EmergencyMedDrop() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += EmergencyMedDropInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= EmergencyMedDropInner;
		}

		public void EmergencyMedDropInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			Unit front = gmLoc.FrontUnit;
			if(front != null)
				deltas.AddRange(
					UnitHealthDelta.GetHealDeltas(
						front,
						gmLoc.SubjectUnit,
						2,
						gmLoc.GameManager
					)
				);

			int side = gmLoc.Side;

			Unit left = gmLoc.LeftLane?.Units?[side, 0];
			if(left != null)
				deltas.AddRange(
					UnitHealthDelta.GetHealDeltas(
						left,
						gmLoc.SubjectUnit,
						1,
						gmLoc.GameManager
					)
				);

			Unit right = gmLoc.RightLane?.Units?[side, 0];
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