using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class PersistentFieldUnitAbility : Ability
	{
		private Unit Source;
		private List<Unit> appliedTo;

		public PersistentFieldUnitAbility() : base(-1)
		{
			appliedTo = new List<Unit>();
		}

		protected override void AddEffectsToEvents(Unit source, GameManager gm)
		{
			Source = source;
			source.AddInitialDeployDeltas += AddPersistentToGM;
			source.AddDeathDeltas += RemovePersistentFromGM;
		}

		protected override void RemoveEffectsFromEvents(Unit source, GameManager gm)
		{
			throw new System.Exception("");
		}

		private void ReapplyEffects(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate) {
			// remove
			foreach(Unit u in appliedTo)
				deltas.AddRange(GetRemoveDeltas(u, Source, gmBoardUpdate.GameManager));
			appliedTo.Clear();

			// add
			Lane[] lanes = gmBoardUpdate.Lanes;
			for(int lane = 0; lane < lanes.Length; lane++) {
				for(int side = 0; side < 2; side++) {
					for(int pos = 0; pos < 2; pos++) {
						if(lanes[lane].Units[side, pos] != null && ApplyTo(lane, side, pos, lanes, Source)) {
							deltas.AddRange(GetAddDeltas(lane, side, pos, lanes, Source, gmBoardUpdate.GameManager));
							appliedTo.Add(lanes[lane].Units[side, pos]);
						}
					}
				}
			}
		}

		private void AddPersistentToGM(List<Delta> deltas, GMWithLocation gmLoc) {
			gmLoc.GameManager.AddBoardUpdateDeltas += ReapplyEffects;
		}

		private void RemovePersistentFromGM(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase) {
			gmLoc.GameManager.AddBoardUpdateDeltas -= ReapplyEffects;
		}

		protected abstract Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source, GameManager gm);
		protected abstract Delta[] GetRemoveDeltas(Unit target, Unit source, GameManager gm);
		protected abstract bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source);
	}
}