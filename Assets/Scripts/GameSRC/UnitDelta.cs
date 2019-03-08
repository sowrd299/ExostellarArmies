using System.Collections;
using System.Collections.Generic;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game {
	public class UnitDelta : Delta {
		private Unit target;
		internal Unit Target {
			get { return target; }
		}

		private Unit source;
		internal Unit Source {
			get { return source; }
		}

		private int amount;
		public int Amount {
			get { return amount; }
		}

		private DamageType dmgType;
		public DamageType DmgType {
			get { return dmgType; }
		}

		internal UnitDelta(Unit t, int a, DamageType type, Unit s) {
			target = t;
			amount = a;
			dmgType = type;
			source = s;
		}

		internal override void Apply() {
			switch(dmgType) {
				case DamageType.RANGED:
					target.takeRangedDamage(amount);
					break;
				case DamageType.MELEE:
					target.takeMeleeDamage(amount);
					break;
				case DamageType.TOWER:
					target.takeTowerDamage(amount);
					break;
				case DamageType.HEAL:
					target.heal(amount);
					break;
			}
		}

		internal override void Revert() {
			if(dmgType == DamageType.HEAL) {
				target.takeTowerDamage(amount);
			} else {
				int mod = (dmgType == DamageType.RANGED
								? target.getTakeRangedDamageModifier()
								: (dmgType == DamageType.MELEE
									? target.getTakeMeleeDamageModifier()
									: 0));
				target.heal(amount >= mod ? amount - mod : 0);
			}
		}

		public enum DamageType {
			RANGED, MELEE, TOWER, HEAL
		}
	}
}
