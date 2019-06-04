using SFB.Game.Management;
using SFB.Game.Content;
using System;

namespace SFB.Game
{
	public class BattleframeFireflyClass : PersistentFieldUnitAbility
	{
		public override string GetMainText() {
			return "Front Line: While this unit is fortified, it has +2M, else it has +1R.";
		}

		public enum AbilityState { UNAPPLIED, APPLIED1R, APPLIED2M }
		public AbilityState State { get; private set; }

		public BattleframeFireflyClass() {
			State = AbilityState.UNAPPLIED;
		}

		protected override Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source, GameManager gm)
		{
			Unit target = lanes[lane].Units[side, pos];
			if(pos == 0) {
				Delta[] deltas = new Delta[1];
				if(lanes[lane].Units[side, 1] == null) {
					deltas[0] = new UnitDamageAmountDelta(target, 1, Damage.Type.RANGED, source);
					State = AbilityState.APPLIED1R;
				} else {
					deltas[0] = new UnitDamageAmountDelta(target, 2, Damage.Type.MELEE, source);
					State = AbilityState.APPLIED2M;
				}
				return deltas;
			} else {
				return new Delta[] {};
			}
		}

		protected override Delta[] GetRemoveDeltas(Unit target, Unit source, GameManager gm)
		{
			switch(State) {
				case AbilityState.APPLIED1R:
					State = AbilityState.UNAPPLIED;
					return new Delta[] { new UnitDamageAmountDelta(target, -1, Damage.Type.RANGED, source) };
				case AbilityState.APPLIED2M:
					State = AbilityState.UNAPPLIED;
					return new Delta[] { new UnitDamageAmountDelta(target, -2, Damage.Type.MELEE, source) };
				default:
					return new Delta[] { };
			}
		}

		protected override bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			return source == lanes[lane].Units[side, pos];
		}
	}
}
