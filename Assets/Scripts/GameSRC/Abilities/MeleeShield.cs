using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFB.Game
{
    public class MeleeShield : Ability {
		public MeleeShield(int n) : base(n) { }
		public override int takeMeleeDamageModifier() { return Num; }
	}
}
