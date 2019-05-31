using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class TemporaryAbility : Ability
	{
		private Unit appliedTo;

		public TemporaryAbility()
			: base(-1)
		{
			appliedTo = null;
		}
		
		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += AddIndividualEffect;
			gm.AddRecurringDeployDeltas += RemoveIndividualEffect;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			throw new System.Exception("Can't remove this ability - potential bugs");
		}
		
		public void AddIndividualEffect(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(ShouldApply(gmLoc)) {
				deltas.AddRange(EffectAddDeltas(gmLoc));
				appliedTo = AppliedTo(gmLoc);
			}
	
		}
		
		public void RemoveIndividualEffect(List<Delta> deltas, GMWithLocation gmLoc) {
			if(appliedTo != null) {
				deltas.AddRange(EffectRemoveDeltas(appliedTo, gmLoc));
				appliedTo = null;
			}
			gmLoc.GameManager.AddRecurringDeployDeltas -= RemoveIndividualEffect;
		}
		
		protected abstract bool ShouldApply(GMWithLocation gmLoc);
		protected abstract Unit AppliedTo(GMWithLocation gmLoc);
		protected abstract Delta[] EffectAddDeltas(GMWithLocation gmLoc);
		protected abstract Delta[] EffectRemoveDeltas(Unit appliedTo, GMWithLocation gmLoc);
	}
}