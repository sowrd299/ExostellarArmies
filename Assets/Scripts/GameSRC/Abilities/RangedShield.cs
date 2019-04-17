using System.Collections;
using System.Collections.Generic;

namespace SFB.Game
{
    public class RangedShield : Ability {
		public RangedShield(int n) : base(n) { }
		public override int takeRangedDamageModifier() { return Num; }
	}
}
