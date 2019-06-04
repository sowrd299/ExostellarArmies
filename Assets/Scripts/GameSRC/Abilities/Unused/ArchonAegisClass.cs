using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class ArchonAegisClass : TemporaryAbility
	{
		public override string GetMainText() {
			return "Fortify Carthan Infantry: The fortified unit gains Ranged Shield 3 this turn.";
		}

		protected override bool ShouldApply(GMWithLocation gmLoc)
		{
			return gmLoc.IsFortifying("Carthan", "Infantry");
		}

		protected override Unit AppliedTo(GMWithLocation gmLoc)
		{
			return gmLoc.FrontUnit;
		}

		protected override Delta[] EffectAddDeltas(GMWithLocation gmLoc)
		{
			return new Delta[] {
				new UnitAbilityDelta(gmLoc.FrontUnit,
									gmLoc.SubjectUnit,
									new RangedShield(3),
									UnitAbilityDelta.DeltaMode.ADD,
									gmLoc.GameManager)
			};
		}

		protected override Delta[] EffectRemoveDeltas(Unit appliedTo, GMWithLocation gmLoc)
		{
			return new Delta[] {
				new UnitAbilityDelta(appliedTo,
									gmLoc.SubjectUnit,
									new RangedShield(3),
									UnitAbilityDelta.DeltaMode.REMOVE,
									gmLoc.GameManager)
			};
		}
	}
}