using System.Collections;
using System.Collections.Generic;

namespace SFB.Game
{
    public class MeleeShield : Ability {
		public MeleeShield(int n) : base(n) { }
		public override int takeMeleeDamageModifier() { return Num; }
	}
}
