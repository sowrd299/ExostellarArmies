using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Fortify: Heal the fortified unit by 2 and each of its neighbors by 1.

	public class ArchonEtherealClass : Ability
	{
		public ArchonEtherealClass() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += ArchonEtherealClassInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= ArchonEtherealClassInner;
		}

		public void ArchonEtherealClassInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			Unit front = gmLoc.FrontUnit;
			if(front != null) {
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
}