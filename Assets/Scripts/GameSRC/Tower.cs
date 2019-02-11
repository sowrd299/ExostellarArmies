using System.Collections;
using System.Collections.Generic;

namespace SFB.Game.Content {
	public class Tower {
		private int maxHP;
		private int hp;
		public int HP {
			get { return hp; }
		}

		public Tower() {
			maxHP = 2;
			hp = maxHP;
		}

		public void takeDamage() {
			hp--;
		}

		public void undoTakeDamage() {
			hp++;
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