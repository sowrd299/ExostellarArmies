using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Deploy…
	//   Front Line: This gets Ranged Shield 3 and Melee Shield 3 this turn.
	//   Support Carthan: This gets +3R this turn.

	public class ExostellarAvengerMarines : Ability
	{
		private Unit appliedShieldTo;
		private Unit applied3RTo;

		public ExostellarAvengerMarines() : base(-1) {
			appliedShieldTo = null;
			applied3RTo = null;
		}

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.AddInitialDeployDeltas += ExostellarAvengerMarinesAdd;
			u.AddRecurringDeployDeltas += ExostellarAvengerMarinesRemove;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			throw new System.Exception("Can't remove this ability - potential bugs");
		}

		public void ExostellarAvengerMarinesAdd(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			Unit self = gameStateLocation.SubjectUnit;
			Unit front = gameStateLocation.FrontUnit;
			if(gameStateLocation.Pos == 0) {
				// Front Line: This gets Ranged Shield 3 and Melee Shield 3 this turn.
				deltas.AddRange(new Delta[] {
					new UnitAbilityDelta(self, self, new RangedShield(3), UnitAbilityDelta.DeltaMode.ADD),
					new UnitAbilityDelta(self, self, new MeleeShield(3), UnitAbilityDelta.DeltaMode.ADD)
				});
				appliedShieldTo = self;
			} else if(gameStateLocation.Pos == 1 && front != null && front.Card.UnitType.Contains("Carthan")) {
				// Support Carthan: This gets +3R this turn.
				deltas.Add(new UnitDamageAmountDelta(self, 3, Damage.Type.RANGED, self));
				applied3RTo = self;
			}
		}

		public void ExostellarAvengerMarinesRemove(List<Delta> deltas, GameStateLocation gameStateLocation) {
			if(appliedShieldTo != null) {
				deltas.AddRange(new Delta[] {
					new UnitAbilityDelta(appliedShieldTo, appliedShieldTo, new RangedShield(3), UnitAbilityDelta.DeltaMode.REMOVE),
					new UnitAbilityDelta(appliedShieldTo, appliedShieldTo, new MeleeShield(3), UnitAbilityDelta.DeltaMode.REMOVE)
				});
				appliedShieldTo = null;
			}
			if(applied3RTo != null) {
				deltas.Add(new UnitDamageAmountDelta(applied3RTo, -3, Damage.Type.RANGED, applied3RTo));
				applied3RTo = null;
			}
		}
	}
}