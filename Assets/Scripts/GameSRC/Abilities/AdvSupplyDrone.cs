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
			Unit front = gameStateLocation.FrontUnit;
			if(gameStateLocation.Pos == 1 && front != null && front.Card.UnitType.Contains("Carthan")) {
				deltas.Add(new UnitTakeDamageDelta(front, 2, Damage.Type.HEAL, gameStateLocation.SubjectUnit));
				deltas.AddRange(
					gameStateLocation.SubjectPlayer.ManaPool.GetAddDeltas(3)
				);
			}
		}
	}
}