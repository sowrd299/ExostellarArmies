using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class FireSupportDrone : TemporaryAbility
	{
		public override string GetMainText() {
			return "Fortify Carthan: Give the fortified unit +3R this turn.";
		}

		protected override bool ShouldApply(GMWithLocation gmLoc) {
			return gmLoc.IsFortifying("Carthan");
		}

		protected override Unit AppliedTo(GMWithLocation gmLoc) {
			return gmLoc.FrontUnit;
		}

		protected override Delta[] EffectAddDeltas(GMWithLocation gmLoc) {
			return new Delta[] {
				new UnitDamageAmountDelta(gmLoc.FrontUnit, 3, Damage.Type.RANGED, gmLoc.SubjectUnit)
			};
		}

		protected override Delta[] EffectRemoveDeltas(Unit appliedTo, GMWithLocation gmLoc) {
			return new Delta[] {
				new UnitDamageAmountDelta(appliedTo, -3, Damage.Type.RANGED, gmLoc.SubjectUnit)
			};
		}
	}
}