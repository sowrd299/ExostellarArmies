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
			u.AddInitialDeployDeltas += AddBoardUpdate;
			u.AddDeathDeltas += RemoveBoardUpdate;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm) {
			gm.AddPersistentDeltas -= RushInner;
		}

		public void AddBoardUpdate(List<Delta> deltas, GMWithLocation gmLoc) {
			gmLoc.GameManager.AddPersistentDeltas += RushInner;
			Tuple<int, int, int> lsp = Lane.GetLaneSidePosOf(gmLoc.SubjectUnit, gmLoc.Lanes);
			Lane = gmLoc.Lanes[lsp.Item1];
			Unit = gmLoc.SubjectUnit;
			Side = lsp.Item2;
		}

		public void RemoveBoardUpdate(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase)
		{
			gmLoc.GameManager.AddPersistentDeltas -= RushInner;
		}

		protected void RushInner(List<Delta> deltas, GameManager gm)
		{
			if(Unit != Lane.Units[Side ?? -1, RushTo()])
				deltas.AddRange(Lane.GetInLaneSwapDeltas(Side ?? -1, gm));
		}

		protected abstract int RushTo();
	}

	public class RushFront : Rush
	{
		public override string GetMainText() {
			return "Rush Front";
		}
		override protected int RushTo() { return 0; }
	}
	public class RushBack : Rush
	{
		public override string GetMainText() {
			return "Rush Back";
		}
		override protected int RushTo() { return 1; }
	}
}