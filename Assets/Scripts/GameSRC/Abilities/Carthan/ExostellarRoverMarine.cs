using SFB.Game.Management;
using SFB.Game.Content;
using System;

namespace SFB.Game
{
	// Front Line: If this has a Back Line, this has +2M, else this has +1R.

	public class ExostellarRoverMarine : PersistentFieldUnitAbility
	{
		private Unit applied1RTo;
		private Unit applied2MTo;

		public ExostellarRoverMarine() {
			applied1RTo = null;
			applied2MTo = null;
		}

		protected override Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source, GameManager gm)
		{
			Unit target = lanes[lane].Units[side, pos];
			if(pos == 0) {
				Delta[] deltas = new Delta[1];
				if(lanes[lane].Units[side, 1] == null) {
					deltas[0] = new UnitDamageAmountDelta(target, 1, Damage.Type.RANGED, source);
					applied1RTo = target;
				} else {
					deltas[0] = new UnitDamageAmountDelta(target, 2, Damage.Type.MELEE, source);
					applied2MTo = target;
				}
				return deltas;
			} else {
				return new Delta[] {};
			}
		}

		protected override Delta[] GetRemoveDeltas(Unit target, Unit source, GameManager gm)
		{
			if(applied1RTo != null) {
				applied1RTo = null;
				return new Delta[] { new UnitDamageAmountDelta(target, -1, Damage.Type.RANGED, source) };
			}
			if(applied2MTo != null) {
				applied2MTo = null;
				return new Delta[] { new UnitDamageAmountDelta(target, -2, Damage.Type.MELEE, source) };
			}
			return new Delta[] { };
		}

		protected override bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			return source == lanes[lane].Units[side, pos];
		}
	}
}
