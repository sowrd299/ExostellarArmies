using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Support Carthan Infantry: Give this Front Line Ranged Shield 3 this turn. (when this is deployed behind Carthan Infantry, activate this ability.

	public class HoverShieldProjector : TemporaryAbility {
		protected override bool ToApplyTo(GameStateLocation gameStateLocation)
		{
			Unit front = gameStateLocation.FrontUnit;
			return gameStateLocation.Pos == 1 && front != null &&
				front.Card.UnitType.Contains("Carthan") &&
				front.Card.UnitType.Contains("Infantry");
		}
		protected override Delta[] EffectAddDeltas(GameStateLocation gameStateLocation)
		{
			return new Delta[] {
				new UnitAbilityDelta(gameStateLocation.FrontUnit,
									gameStateLocation.SubjectUnit,
									new RangedShield(3),
									UnitAbilityDelta.DeltaMode.ADD)
			};
		}
		protected override Delta[] EffectRemoveDeltas(Unit appliedTo, GameStateLocation gameStateLocation)
		{
			return new Delta[] {
				new UnitAbilityDelta(gameStateLocation.FrontUnit,
									gameStateLocation.SubjectUnit,
									new RangedShield(3),
									UnitAbilityDelta.DeltaMode.REMOVE)
			};
		}
	}
}