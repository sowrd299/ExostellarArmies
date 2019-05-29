using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class PersistentFieldAbility : Ability
	{
		private AddDeltaBoardUpdate remove2ndEffects;
		private AddDeltaBoardUpdate add2ndEffects;
		private AddDeltaPhase removeThisFromGM;
		private List<Unit> appliedTo;

		public PersistentFieldAbility() : base(-1)
		{
			remove2ndEffects = null;
			add2ndEffects = null;
			removeThisFromGM = null;
			appliedTo = new List<Unit>();
		}

		protected override void AddEffectsToEvents(Unit source, GameManager gm)
		{
			remove2ndEffects = Remove2ndEffects(source);
			add2ndEffects = Add2ndEffects(source);
			removeThisFromGM = RemoveThisFromGM();

			gm.AddBoardUpdateDeltas += remove2ndEffects;
			gm.AddBoardUpdateDeltas += add2ndEffects;

			source.AddDeathDeltas += removeThisFromGM;
		}

		protected override void RemoveEffectsFromEvents(Unit source, GameManager gm)
		{
			throw new System.Exception("cannot remove persistent field ability effect");
		}

		private AddDeltaBoardUpdate Add2ndEffects(Unit source)
		{
			void InnerAdd2ndEffects(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate) {
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

			return InnerAdd2ndEffects;
		}

		private AddDeltaBoardUpdate Remove2ndEffects(Unit source)
		{
			void InnerRemove2ndEffects(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate) {
				foreach(Unit u in appliedTo)
					deltas.AddRange(GetRemoveDeltas(u, source));
			}
			return InnerRemove2ndEffects;
		}

		private AddDeltaPhase RemoveThisFromGM()
		{
			void InnerRemoveThisFromGM(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase) {
				gmLoc.GameManager.AddBoardUpdateDeltas -= remove2ndEffects;
				gmLoc.GameManager.AddBoardUpdateDeltas -= add2ndEffects;
			}
			return InnerRemoveThisFromGM;
		}
		

		protected abstract Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source);
		protected abstract Delta[] GetRemoveDeltas(Unit target, Unit source);
		protected abstract bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source);
	}
}