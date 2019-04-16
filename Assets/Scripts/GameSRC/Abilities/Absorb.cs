using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFB.Game
{
	public class Absorb : Ability {
		public Absorb() : base() { }
		public override int takeDamageLeftModifier(int dmgLeft, int deal) { return -dmgLeft; }
	}
}
