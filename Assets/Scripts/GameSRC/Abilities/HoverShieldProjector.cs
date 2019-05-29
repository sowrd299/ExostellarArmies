using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class HoverShieldProjector : TemporaryAbility
	{
		// Support Carthan Infantry: Give this Front Line Ranged Shield 3 this turn. (when this is deployed behind Carthan Infantry, activate this ability.

		protected override bool ShouldApply(GMWithLocation gmLoc)
		{
			Unit front = gmLoc.FrontUnit;
			return gmLoc.IsSupporting(new string[] { "Carthan", "Infantry" });
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
									UnitAbilityDelta.DeltaMode.ADD)
			};
		}

		protected override Delta[] EffectRemoveDeltas(Unit appliedTo, GMWithLocation gmLoc)
		{
			return new Delta[] {
				new UnitAbilityDelta(appliedTo,
									gmLoc.SubjectUnit,
									new RangedShield(3),
									UnitAbilityDelta.DeltaMode.REMOVE)
			};
		}
	}
}