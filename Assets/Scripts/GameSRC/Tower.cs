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

		public void takeDamage() {
			hp--;
		}

		public void revive() {
			if(hp == 0) {
				maxHP++;
				hp = maxHP;
			} else {
				//exception?
			}
		}
	}
}