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
			Unit front = gameStateLocation.FrontUnit;
			if(gameStateLocation.Pos == 1 && front != null && front.Card.UnitType.Contains("Carthan")) {
				deltas.Add(new UnitTakeDamageDelta(front, 2, Damage.Type.HEAL, gameStateLocation.SubjectUnit));
			}
		}
	}
}