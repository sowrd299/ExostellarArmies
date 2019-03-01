using System.Collections;
using System.Collections.Generic;

namespace SFB.Game.Content {
	public class Tower {
		private int maxHP;
		private int hp;
		public int HP {
			get { return hp; }
		}

		private int baseDamage;
		private int damage;
		public int Damage {
			get { return damage; }
			set { damage = value; }
		}

		public Tower() {
			maxHP = 2;
			hp = maxHP;
			baseDamage = 1;
			damage = baseDamage;
		}

		public void takeDamage(int n) {
			hp -= n;
		}

		public void undoTakeDamage(int n) {
			hp += n;
		}

		public void revive() {
			if(hp == 0) {
				hp = ++maxHP;
			} else {
				//exception?
			}
		}

		public void resetDamage() {
			damage = baseDamage;
		}
	}
}