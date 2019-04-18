using System.Collections;
using System.Collections.Generic;

namespace SFB.Game
{
    public class Siege : Ability {
		public Siege(int n) : base(n) { }

		public override int dealTowerDamageModifier() {
			return Num;
		}
	}
}
