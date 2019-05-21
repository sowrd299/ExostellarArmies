using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Support Carthan: Heal this Front Line 2 and gain 3 Resources.

	public class AdvSupplyDrone : Ability
	{
		public AdvSupplyDrone() : base(-1) { }

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.AddInitialDeployDeltas += AdvSupplyDroneInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			u.AddInitialDeployDeltas -= AdvSupplyDroneInner;
		}

		public void AdvSupplyDroneInner(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			if(gameStateLocation.IsSupporting(new string[] { "Carthan" })) {
				deltas.Add(new UnitHealthDelta(gameStateLocation.FrontUnit, 2, Damage.Type.HEAL, gameStateLocation.SubjectUnit));
				deltas.AddRange(
					gameStateLocation.SubjectPlayer.ManaPool.GetAddDeltas(3)
				);
			}
		}
	}
}