using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;
using System;

namespace SFB.Game
{
	public abstract class Rush : Ability
	{
		// After you deploy a Unit <in a specific position>, they switch places.

		private Lane Lane;
		private Unit Unit;
		private int? Side;

		public Rush()
			: base(-1)
		{
			Lane = null;
			Unit = null;
			Side = null;
		}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			gm.AddBoardUpdateDeltas += RushInner;
			u.AddDeathDeltas += RemoveBoardUpdate;
			Tuple<int, int, int> lsp = Lane.GetLaneSidePosOf(u, gm.Lanes);
			Lane = gm.Lanes[lsp.Item1];
			Unit = u;
			Side = lsp.Item2;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm) {
			gm.AddBoardUpdateDeltas -= RushInner;
		}

		public void RemoveBoardUpdate(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase)
		{
			gmLoc.GameManager.AddBoardUpdateDeltas -= RushInner;
		}

		protected void RushInner(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate)
		{
			if(Unit != Lane.Units[Side ?? -1, RushTo()])
				deltas.AddRange(gmBoardUpdate.SubjectLane
											 .GetInLaneSwapDeltas(
												 Side ?? -1,
												 gmBoardUpdate.GameManager
											 )
				);
		}

		protected abstract int RushTo();
	}

	public class RushFront : Rush { override protected int RushTo() { return 0; } }
	public class RushBack : Rush { override protected int RushTo() { return 1; } }
}