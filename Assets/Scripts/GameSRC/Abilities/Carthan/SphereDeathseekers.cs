using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class SphereDeathseekers : Ability
	{
		public override string GetMainText() {
			return "Deploy Front Line: This gains Ranged and Melee Shield 3 this turn.\n"+
			"Fortify Carthan: This gains +3R this turn.";
		}

		private Unit appliedShieldTo;
		private Unit applied3RTo;
		private int turn;

		public SphereDeathseekers() : base(-1) {
			appliedShieldTo = null;
			applied3RTo = null;
			turn = 0;
		}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas += SphereDeathseekersInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			throw new System.Exception("Can't remove this ability - potential bugs");
		}

		public void SphereDeathseekersInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(turn == 0) {
				Unit self = gmLoc.SubjectUnit;
				Unit front = gmLoc.FrontUnit;
				if(gmLoc.Pos == 0) {
					// Deploy Front Line: This gains Ranged and Melee Shield 3 this turn.
					deltas.AddRange(new Delta[] {
					new UnitAbilityDelta(self, self, new RangedShield(3), UnitAbilityDelta.DeltaMode.ADD, gmLoc.GameManager),
					new UnitAbilityDelta(self, self, new MeleeShield(3), UnitAbilityDelta.DeltaMode.ADD, gmLoc.GameManager)
				});
					appliedShieldTo = self;
				} else if(gmLoc.IsFortifying("Carthan")) {
					// Fortify Carthan: This gains +3R this turn.
					deltas.Add(new UnitDamageAmountDelta(self, 3, Damage.Type.RANGED, self));
					applied3RTo = self;
				}
			} else if(turn == 1) {
				if(appliedShieldTo != null) {
					deltas.AddRange(new Delta[] {
					new UnitAbilityDelta(appliedShieldTo, appliedShieldTo, new RangedShield(3), UnitAbilityDelta.DeltaMode.REMOVE, gmLoc.GameManager),
					new UnitAbilityDelta(appliedShieldTo, appliedShieldTo, new MeleeShield(3), UnitAbilityDelta.DeltaMode.REMOVE, gmLoc.GameManager)
				});
					appliedShieldTo = null;
				}
				if(applied3RTo != null) {
					deltas.Add(new UnitDamageAmountDelta(applied3RTo, -3, Damage.Type.RANGED, applied3RTo));
					applied3RTo = null;
				}
			}
			turn++;
		}
	}
}