using System.Collections;
using System.Collections.Generic;

namespace SFB.Game
{
    public class TowerShield : Ability {
		public TowerShield(int n) : base(n) { }
		public override int takeTowerDamageModifier() { return Num; }
	}
}
