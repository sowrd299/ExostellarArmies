using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class TemporaryAbility : Ability
	{
		private Unit appliedTo;
		private int turn;

		public TemporaryAbility()
			: base(-1)
		{
			appliedTo = null;
			turn = 0;
		}
		
		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas += TempInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			throw new System.Exception("Can't remove this ability - potential bugs");
		}
		
		public void TempInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(turn == 0 && ShouldApply(gmLoc)) {
				deltas.AddRange(EffectAddDeltas(gmLoc));
				appliedTo = AppliedTo(gmLoc);
			} else if(turn == 1 && appliedTo != null) {
				deltas.AddRange(EffectRemoveDeltas(appliedTo, gmLoc));
				appliedTo = null;
			}
			turn++;
		}
		
		protected abstract bool ShouldApply(GMWithLocation gmLoc);
		protected abstract Unit AppliedTo(GMWithLocation gmLoc);
		protected abstract Delta[] EffectAddDeltas(GMWithLocation gmLoc);
		protected abstract Delta[] EffectRemoveDeltas(Unit appliedTo, GMWithLocation gmLoc);
	}
}