using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Deploy: Heal this Front Line 2 and each adjacent Front Line 1.

	public class EmergencyMedDrop : Ability
	{
		public EmergencyMedDrop() : base(-1) { }

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.AddInitialDeployDeltas += EmergencyMedDropInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			u.AddInitialDeployDeltas -= EmergencyMedDropInner;
		}

		public void EmergencyMedDropInner(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			// TODO: Deploy: Heal this Front Line 2 and each adjacent Front Line 1.
			Unit front = gameStateLocation.FrontUnit;
			if(front != null) // returns null if already front
				deltas.Add(new UnitTakeDamageDelta(front, 2, Damage.Type.HEAL, gameStateLocation.SubjectUnit));

			int side = gameStateLocation.Side;

			Unit left = gameStateLocation.LeftLane?.Units?[side, 0];
			if(left != null)
				deltas.Add(new UnitTakeDamageDelta(left, 1, Damage.Type.HEAL, gameStateLocation.SubjectUnit));

			Unit right = gameStateLocation.RightLane?.Units?[side, 0];
			if(right != null)
				deltas.Add(new UnitTakeDamageDelta(right, 1, Damage.Type.HEAL, gameStateLocation.SubjectUnit));
		}
	}
}