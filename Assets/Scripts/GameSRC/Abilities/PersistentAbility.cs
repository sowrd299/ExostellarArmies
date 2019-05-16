using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class PersistentFieldAbility : Ability
	{
		private AddDelta removeEffects;
		private AddDelta addEffects;

		public PersistentFieldAbility() : base(-1)
		{
			removeEffects = null;
			addEffects = null;
		}

		protected override void ApplyEffects(Unit source, GameState gameState) {
			removeEffects = RemoveEffects(source);
			addEffects = AddEffects(source);

			gameState.AddBoardUpdateDeltas += removeEffects;
			gameState.AddBoardUpdateDeltas += addEffects;

			source.AddDeathDeltas += RemovePersistentFromGM;
		}

		protected override void RemoveEffects(Unit source, GameState gameState)
		{
			AddDelta removeEffects = RemoveEffects(source);
			AddDelta addEffects = AddEffects(source);

			gameState.AddBoardUpdateDeltas -= removeEffects;
			gameState.AddBoardUpdateDeltas -= addEffects;

			void RemovePersistentFromGM(List<Delta> deltas, GameStateLocation gameStateLoc) {
				gameStateLoc.GameState.AddBoardUpdateDeltas -= removeEffects;
				gameStateLoc.GameState.AddBoardUpdateDeltas -= addEffects;
			}
			source.AddDeathDeltas -= RemovePersistentFromGM;
		}

		private AddDelta AddEffects(Unit source)
		{
			void InnerAddEffects(List<Delta> deltas, GameStateLocation gameStateLoc) {
				Lane[] lanes = gameStateLoc.Lanes;
				for(int lane = 0; lane < lanes.Length; lane++) {
					for(int side = 0; side < 2; side++)
						for(int pos = 0; pos < 2; pos++)
							if(lanes[lane].Units[side, pos] != null && ApplyTo(lane, side, pos, lanes, source))
								deltas.AddRange(GetAddDeltas(lane, side, pos, lanes, source));
				}
			}

			return InnerAddEffects;
		}

		private AddDelta RemoveEffects(Unit source)
		{
			void InnerRemoveEffects(List<Delta> deltas, GameStateLocation gameStateLoc) {
				Lane[] lanes = gameStateLoc.Lanes;
				for(int lane = 0; lane < lanes.Length; lane++) {
					for(int side = 0; side < 2; side++)
						for(int pos = 0; pos < 2; pos++)
							if(lanes[lane].Units[side, pos] != null)
								deltas.AddRange(GetRemoveDeltas(lane, side, pos, lanes, source));
				}
			}
			return InnerRemoveEffects;
		}

		private void RemovePersistentFromGM(List<Delta> deltas, GameStateLocation gameStateLoc) {
			gameStateLoc.GameState.AddBoardUpdateDeltas -= removeEffects;
			gameStateLoc.GameState.AddBoardUpdateDeltas -= addEffects;
		}

		protected abstract Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source);
		protected abstract Delta[] GetRemoveDeltas(int lane, int side, int pos, Lane[] lanes, Unit source);
		protected abstract bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source);
	}
}