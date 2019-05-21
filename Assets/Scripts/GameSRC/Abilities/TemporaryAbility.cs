using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class TemporaryAbility : Ability
	{
		private Unit appliedTo;

		public TemporaryAbility() : base(-1) {
			appliedTo = null;
		}

		protected override void ApplyEffects(Unit u, GameState gameState)
		{
			u.AddInitialDeployDeltas += AddIndividualEffect;
			gameState.AddRecurringDeployDeltas += RemoveIndividualEffect;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			throw new System.Exception("Can't remove this ability - potential bugs");
		}

		public void AddIndividualEffect(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			if(ToApplyTo(gameStateLocation)) {
				deltas.AddRange(EffectAddDeltas(gameStateLocation));
				appliedTo = gameStateLocation.SubjectUnit;
			}
	
		}

		public void RemoveIndividualEffect(List<Delta> deltas, GameStateLocation gameStateLocation) {
			if(appliedTo != null) {
				deltas.AddRange(EffectRemoveDeltas(appliedTo, gameStateLocation));
				appliedTo = null;
			}
			gameStateLocation.GameState.AddRecurringDeployDeltas -= RemoveIndividualEffect;
		}
		
		protected abstract bool ToApplyTo(GameStateLocation gameStateLocation);
		protected abstract Delta[] EffectAddDeltas(GameStateLocation gameStateLocation);
		protected abstract Delta[] EffectRemoveDeltas(Unit appliedTo, GameStateLocation gameStateLocation);
	}
}