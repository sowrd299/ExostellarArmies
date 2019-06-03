using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class CannoneerDrone : Ability
	{
		// Front Line Persistent: The tower behind this deals +3 damage.

		private AddDeltaGMBoardUpdate remove2ndaryEffects;
		private AddDeltaGMBoardUpdate add2ndaryEffects;
		private AddDeltaGMLocPhase removeThisFromGM;
		private Tower appliedTo;

		public CannoneerDrone() : base(-1)
		{
			remove2ndaryEffects = null;
			add2ndaryEffects = null;
			appliedTo = null;
		}

		protected override void AddEffectsToEvents(Unit source, GameManager gm)
		{
			remove2ndaryEffects = Remove2ndaryEffects();
			add2ndaryEffects = InnerAddEffects;
			removeThisFromGM = RemoveThisFromGM();

			gm.AddBoardUpdateDeltas += remove2ndaryEffects;
			gm.AddBoardUpdateDeltas += add2ndaryEffects;
			source.AddDeathDeltas += removeThisFromGM;
		}

		protected override void RemoveEffectsFromEvents(Unit source, GameManager gm)
		{
			throw new System.Exception("");
		}

		private void InnerAddEffects(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate)
		{
			Tower tower = gmBoardUpdate.SubjectLane.Towers[gmBoardUpdate.Side];
			deltas.Add(
				new TowerDamageAmountDelta(tower, 3)
			);
			appliedTo = tower;
		}

		private AddDeltaGMBoardUpdate Remove2ndaryEffects()
		{
			void InnerRemoveEffects(List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate) {
				deltas.Add(new TowerDamageAmountDelta(appliedTo, -3));
				appliedTo = null;
			}
			return InnerRemoveEffects;
		}

		private AddDeltaGMLocPhase RemoveThisFromGM()
		{
			void RemovePersistentFromGM(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase) {
				gmLoc.GameManager.AddBoardUpdateDeltas -= remove2ndaryEffects;
				gmLoc.GameManager.AddBoardUpdateDeltas -= add2ndaryEffects;
				if(appliedTo != null) {
					deltas.Add(new TowerDamageAmountDelta(appliedTo, -3));
					appliedTo = null;
				}
			}
			return RemovePersistentFromGM;
		}
	}
}
