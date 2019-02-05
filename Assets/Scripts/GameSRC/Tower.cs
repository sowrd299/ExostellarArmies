using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFB.Game.Content {
	public class Tower {
		private int maxHP;
		private int hp;

		public Tower() {
			maxHP = 2;
			hp = maxHP;
		}

		public void revive() {
			maxHP++;
			hp = maxHP;
		}
	}
}