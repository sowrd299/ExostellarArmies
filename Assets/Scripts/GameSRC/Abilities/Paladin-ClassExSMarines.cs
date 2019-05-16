using SFB.Game.Management;
using System.Collections.Generic;
using System;

namespace SFB.Game
{
	// Front Line Recurring Deploy: If there is not a unit behind this, heal each adjacent allied front line unit 1.

	public class PaladinClassExSMarines : Ability
	{
		public PaladinClassExSMarines() : base(-1) { }

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.AddRecurringDeployDeltas += PaladinClassExSMarinesInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			u.AddRecurringDeployDeltas -= PaladinClassExSMarinesInner;
		}

		public void PaladinClassExSMarinesInner(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			if(gameStateLocation.Pos == 0 && gameStateLocation.BackUnit == null) {
				int lane = gameStateLocation.Lane;
				int side = gameStateLocation.Side;

				Unit leftLaneFront = gameStateLocation.LeftLane?.Units?[side, 0];
				if(leftLaneFront != null)
					deltas.Add(new UnitTakeDamageDelta(leftLaneFront, 1, Damage.Type.HEAL, gameStateLocation.SubjectUnit));

				Unit rightLaneFront = gameStateLocation.RightLane?.Units?[side, 0];
				if(rightLaneFront != null)
					deltas.Add(new UnitTakeDamageDelta(rightLaneFront, 1, Damage.Type.HEAL, gameStateLocation.SubjectUnit));
			}
		}
	}
}