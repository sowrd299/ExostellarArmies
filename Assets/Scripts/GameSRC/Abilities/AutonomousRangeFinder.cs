using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Support Carthan: Give this Front Line +3R this turn.

	public class AutonomousRangeFinder : Ability
	{
		private Unit appliedTo;
		private Unit source;

		public AutonomousRangeFinder() : base(-1) {
			appliedTo = null;
			source = null;
		}

		protected override void ApplyEffects(Unit u, GameState gameState)
		{
			u.AddInitialDeployDeltas += AutonomousRangeFinderAdd;
			gameState.AddRecurringDeployDeltas += AutonomousRangeFinderRemove;
			source = u;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			throw new System.Exception("Can't remove this ability");
		}

		public void AutonomousRangeFinderAdd(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			Unit front = gameStateLocation.FrontUnit;
			if(appliedTo == null && gameStateLocation.Pos == 1 && front != null && front.Card.UnitType.Contains("Carthan")) {
				deltas.Add(new UnitDamageAmountDelta(front, 3, Damage.Type.RANGED, gameStateLocation.SubjectUnit));
				appliedTo = front;
			}
		}

		public void AutonomousRangeFinderRemove(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			if(appliedTo != null) {
				deltas.Add(new UnitDamageAmountDelta(appliedTo, -3, Damage.Type.RANGED, source));
				appliedTo = null;
			}
			gameStateLocation.GameState.AddRecurringDeployDeltas -= AutonomousRangeFinderRemove;
		}
	}
}