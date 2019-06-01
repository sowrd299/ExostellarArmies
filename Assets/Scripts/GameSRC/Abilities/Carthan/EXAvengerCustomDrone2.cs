using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Front Line: This gets Ranged Shield 2 this turn.

	public class EXAvengerCustomDrone2 : TemporaryAbility
	{
		protected override bool ShouldApply(GMWithLocation gmLoc)
		{
			return gmLoc.Pos == 0;
		}

		protected override Unit AppliedTo(GMWithLocation gmLoc)
		{
			return gmLoc.SubjectUnit;
		}

		protected override Delta[] EffectAddDeltas(GMWithLocation gmLoc)
		{
			return new Delta[] {
				new UnitAbilityDelta(gmLoc.SubjectUnit,
									gmLoc.SubjectUnit,
									new RangedShield(2),
									UnitAbilityDelta.DeltaMode.ADD,
									gmLoc.GameManager)
			};
		}

		protected override Delta[] EffectRemoveDeltas(Unit appliedTo, GMWithLocation gmLoc)
		{
			return new Delta[] {
				new UnitAbilityDelta(appliedTo,
									gmLoc.SubjectUnit,
									new RangedShield(2),
									UnitAbilityDelta.DeltaMode.REMOVE,
									gmLoc.GameManager)
			};
		}
	}
}