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
			if(gmLoc.IsSupporting(new string[] { "Carthan" })) {
				deltas.Add(new UnitHealthDelta(gmLoc.FrontUnit, 2, Damage.Type.HEAL, gmLoc.SubjectUnit));
			}
		}
	}
}