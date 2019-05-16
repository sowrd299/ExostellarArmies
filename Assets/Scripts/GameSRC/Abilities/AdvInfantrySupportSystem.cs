using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Front or Back Line: At the start of your turn, generate an extra resource.

	public class AdvInfantrySupportSystem : Ability
	{
		public AdvInfantrySupportSystem() : base(-1) { }

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.AddRecurringDeployDeltas += AdvInfantrySupportSystemInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			u.AddRecurringDeployDeltas -= AdvInfantrySupportSystemInner;
		}

		public void AdvInfantrySupportSystemInner(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			deltas.AddRange(gameStateLocation.SubjectPlayer.ManaPool.GetAddDeltas(1));
		}
	}
}