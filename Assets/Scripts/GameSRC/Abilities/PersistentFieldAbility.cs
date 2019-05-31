using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class PersistentFieldAbility : Ability
	{
		private AddDeltaGMBoardUpdate remove2ndaryEffects;
		private AddDeltaGMBoardUpdate add2ndaryEffects;
		private AddDeltaGMLocPhase removeThisFromGM;
		private List<Unit> appliedTo;

		public PersistentFieldAbility() : base(-1)
		{
			remove2ndaryEffects = null;
			add2ndaryEffects = null;
			appliedTo = new List<Unit>();
		}

		protected override void AddEffectsToEvents(Unit source, GameManager gm)
		{
			remove2ndaryEffects = Remove2ndaryEffects(source, gm);
			add2ndaryEffects = Add2ndaryEffects(source, gm);
			removeThisFromGM = RemoveThisFromGM();

			gm.AddBoardUpdateDeltas += remove2ndaryEffects;
			gm.AddBoardUpdateDeltas += add2ndaryEffects;
			source.AddDeathDeltas += removeThisFromGM;
		}

		protected override void RemoveEffectsFromEvents(Unit source, GameManager gm)
		{
			throw new System.Exception("");
		}

		private AddDeltaGMBoardUpdate Add2ndaryEffects(Unit source, GameManager gm)
		{
			void InnerAddEffects(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate) {
				Lane[] lanes = gmBoardUpdate.Lanes;
				for(int lane = 0; lane < lanes.Length; lane++) {
					for(int side = 0; side < 2; side++) {
						for(int pos = 0; pos < 2; pos++) {
							if(lanes[lane].Units[side, pos] != null && ApplyTo(lane, side, pos, lanes, source)) {
								deltas.AddRange(GetAddDeltas(lane, side, pos, lanes, source, gm));
								appliedTo.Add(lanes[lane].Units[side, pos]);
							}
						}
					}
				}
			}

			return InnerAddEffects;
		}

		private AddDeltaGMBoardUpdate Remove2ndaryEffects(Unit source, GameManager gm)
		{
			void InnerRemoveEffects(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate) {
				foreach(Unit u in appliedTo)
					deltas.AddRange(GetRemoveDeltas(u, source, gm));
			}
			return InnerRemoveEffects;
		}

		private AddDeltaGMLocPhase RemoveThisFromGM()
		{
			void RemovePersistentFromGM(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase) {
				gmLoc.GameManager.AddBoardUpdateDeltas -= remove2ndaryEffects;
				gmLoc.GameManager.AddBoardUpdateDeltas -= add2ndaryEffects;
			}
			return RemovePersistentFromGM;
		}

		protected abstract Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source, GameManager gm);
		protected abstract Delta[] GetRemoveDeltas(Unit target, Unit source, GameManager gm);
		protected abstract bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source);
	}
}