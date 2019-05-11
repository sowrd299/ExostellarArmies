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

		public override void ApplyTo(Unit u, GameState initialGameState)
		{
			void RushInner(List<Delta> deltas, GameStateLocation gameStateLoc) {
				int side = gameStateLoc.Side;
				if(gameStateLoc.SubjectUnit != gameStateLoc.SubjectLane.Units[side, RushTo()])
					deltas.AddRange(gameStateLoc.SubjectLane.GetInLaneSwapDeltas(side));
			}
			u.AddRecurringDeployDeltas += RushInner;
		}

		protected abstract int RushTo();
	}

	public class RushFront : Rush { override protected int RushTo() { return 0; } }
	public class RushBack : Rush { override protected int RushTo() { return 1; } }
}