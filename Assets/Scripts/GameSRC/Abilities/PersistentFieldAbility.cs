using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class PersistentFieldAbility : Ability
	{
		private AddDeltaBoardUpdate remove2ndaryEffects;
		private AddDeltaBoardUpdate add2ndaryEffects;
		private AddDeltaPhase removeThisFromGM;
		private List<Unit> appliedTo;

		public PersistentFieldAbility() : base(-1)
		{
			remove2ndaryEffects = null;
			add2ndaryEffects = null;
			appliedTo = new List<Unit>();
		}

		protected override void AddEffectsToEvents(Unit source, GameManager gm)
		{
			remove2ndaryEffects = RemoveEffects(source);
			add2ndaryEffects = Add2ndaryEffects(source);
			removeThisFromGM = RemoveThisFromGM();

			gm.AddBoardUpdateDeltas += remove2ndaryEffects;
			gm.AddBoardUpdateDeltas += add2ndaryEffects;
			source.AddDeathDeltas += removeThisFromGM;
		}

		protected override void RemoveEffectsFromEvents(Unit source, GameManager gm)
		{
			throw new System.Exception("");
		}

		private AddDeltaBoardUpdate Add2ndaryEffects(Unit source)
		{
			void InnerAddEffects(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate) {
				Lane[] lanes = gmBoardUpdate.Lanes;
				for(int lane = 0; lane < lanes.Length; lane++) {
					for(int side = 0; side < 2; side++) {
						for(int pos = 0; pos < 2; pos++) {
							if(lanes[lane].Units[side, pos] != null && ApplyTo(lane, side, pos, lanes, source)) {
								deltas.AddRange(GetAddDeltas(lane, side, pos, lanes, source));
								appliedTo.Add(lanes[lane].Units[side, pos]);
							}
						}
					}
				}
			}

			return InnerAddEffects;
		}

		private AddDeltaBoardUpdate RemoveEffects(Unit source)
		{
			void InnerRemoveEffects(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate) {
				foreach(Unit u in appliedTo)
					deltas.AddRange(GetRemoveDeltas(u, source));
			}
			return InnerRemoveEffects;
		}

		private AddDeltaPhase RemoveThisFromGM()
		{
			void RemovePersistentFromGM(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase) {
				gmLoc.GameManager.AddBoardUpdateDeltas -= remove2ndaryEffects;
				gmLoc.GameManager.AddBoardUpdateDeltas -= add2ndaryEffects;
			}
			return RemovePersistentFromGM;
		}

		protected abstract Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source);
		protected abstract Delta[] GetRemoveDeltas(Unit target, Unit source);
		protected abstract bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source);
	}
}