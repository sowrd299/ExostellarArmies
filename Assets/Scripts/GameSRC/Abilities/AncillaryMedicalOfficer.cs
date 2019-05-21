using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Support Carthan Infantry: Heal this Front Line 2.

	public class AncillaryMedicalOfficer : Ability
	{
		public AncillaryMedicalOfficer() : base(-1) { }

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.AddInitialDeployDeltas += AncillaryMedicalOfficerInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			u.AddInitialDeployDeltas -= AncillaryMedicalOfficerInner;
		}

		public void AncillaryMedicalOfficerInner(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			if(gameStateLocation.IsSupporting(new string[] { "Carthan" })) {
				deltas.Add(new UnitHealthDelta(gameStateLocation.FrontUnit, 2, Damage.Type.HEAL, gameStateLocation.SubjectUnit));
			}
		}
	}
}