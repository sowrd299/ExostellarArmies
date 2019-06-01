using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class AncillaryMedicalOfficer : Ability
	{
		// Support Carthan Infantry: Heal this Front Line 2.

		public AncillaryMedicalOfficer() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += AncillaryMedicalOfficerInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= AncillaryMedicalOfficerInner;
		}

		public void AncillaryMedicalOfficerInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.IsSupporting("Carthan")) {
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