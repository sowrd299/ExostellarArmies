using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class SphereSkeletalReshaper : Ability
	{
		// Fortify Carthan: Heal the fortified unit by 2.

		public SphereSkeletalReshaper() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += SphereSkeletalReshaperInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= SphereSkeletalReshaperInner;
		}

		public void SphereSkeletalReshaperInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.IsFortifying("Carthan")) {
				deltas.AddRange(
					UnitHealthDelta.GetHealDeltas(
						gmLoc.FrontUnit,
						gmLoc.SubjectUnit,
						2,
						gmLoc.GameManager
					)
				);
			}
		}
	}
}