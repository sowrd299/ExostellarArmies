using System.Collections;
using System.Collections.Generic;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game {
	public class UnitDelta : Delta {
		private Unit unit;
		internal Unit Unit {
			get { return unit; }
		}

		private int amount;
		public int Amount {
			get { return amount; }
		}

		private DamageType dmgType;
		public DamageType DmgType {
			get { return dmgType; }
		}

		internal UnitDelta(Unit u, int a, DamageType t) {
			unit = u;
			amount = a;
			dmgType = t;
		}

		internal override void Apply() {
			switch(dmgType) {
				case DamageType.RANGED:
					unit.takeRangedDamage(amount);
					break;
				case DamageType.MELEE:
					unit.takeMeleeDamage(amount);
					break;
				case DamageType.TRUE:
					unit.takeTrueDamage(amount);
					break;
				case DamageType.HEAL:
					unit.heal(amount);
					break;
			}
		}

		internal override void Revert() {
			if(dmgType == DamageType.HEAL) {
				unit.takeTrueDamage(amount);
			} else {
				int mod = (dmgType == DamageType.RANGED
								? unit.getRangedDamageModifier()
								: (dmgType == DamageType.MELEE
									? unit.getMeleeDamageModifier()
									: 0));
				unit.heal(amount >= mod ? amount - mod : 0);
			}
		}

		public enum DamageType {
			RANGED, MELEE, TRUE, HEAL
		}
	}
}
