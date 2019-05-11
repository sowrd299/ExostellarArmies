using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class PersistentFieldAbility : Ability
	{
		public PersistentFieldAbility()
			: base(-1)
		{ }

		public override void ApplyTo(Unit source, GameState initialGameState) {
			initialGameState.AddBoardUpdateDeltas += RemoveEffects(source);
			initialGameState.AddBoardUpdateDeltas += AddEffects(source);

			void RemovePersistentFromGM(List<Delta> deltas, GameStateLocation gameStateLoc)
			{
				gameStateLoc.GameState.AddBoardUpdateDeltas -= RemoveEffects(source);
				gameStateLoc.GameState.AddBoardUpdateDeltas -= AddEffects(source);
			}
			source.AddDeathDeltas += RemovePersistentFromGM;
		}
		
		private Ability.AddDelta AddEffects(Unit source)
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

		private Ability.AddDelta RemoveEffects(Unit source)
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

		protected abstract Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source);
		protected abstract Delta[] GetRemoveDeltas(int lane, int side, int pos, Lane[] lanes, Unit source);
		protected abstract bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source);
	}
}