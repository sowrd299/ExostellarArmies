using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game
{
	public class CmdrYosLorth : PersistentFieldAbility
	{
		protected override Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			return new Delta[] { new UnitDamageAmountDelta(lanes[lane].Units[side, pos], 1, Damage.Type.MELEE, null) };
		}

		protected override Delta[] GetRemoveDeltas(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			return new Delta[] { new UnitDamageAmountDelta(lanes[lane].Units[side, pos], -1, Damage.Type.MELEE, null) };
		}

		protected override bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			return lanes[lane].Units[side, pos] != source;
		}
	}
}
