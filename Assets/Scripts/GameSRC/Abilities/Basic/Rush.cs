using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;
using System;

namespace SFB.Game
{
	public abstract class Rush : Ability
	{
		public Rush()
			: base(-1)
		{}

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.AddRecurringDeployDeltas += RushInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState) {
			u.AddRecurringDeployDeltas -= RushInner;
		}

		protected void RushInner(List<Delta> deltas, GameStateLocation gameStateLoc)
		{
			int side = gameStateLoc.Side;
			if(gameStateLoc.SubjectUnit != gameStateLoc.SubjectLane.Units[side, RushTo()])
				deltas.AddRange(gameStateLoc.SubjectLane.GetInLaneSwapDeltas(side));
		}

		protected abstract int RushTo();
	}

	public class RushFront : Rush { override protected int RushTo() { return 0; } }
	public class RushBack : Rush { override protected int RushTo() { return 1; } }
}