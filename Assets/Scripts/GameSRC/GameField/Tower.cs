using SFB.Game.Management;

namespace SFB.Game.Content {
	public class Tower : IIDed {
		public static IdIssuer<Tower> idIssuer = new IdIssuer<Tower>();

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

		readonly public string id;
		public string ID {
			get { return id; }
		}

		public Tower() {
			maxHP = 2;
			hp = maxHP;
			baseDamage = 1;
			damage = baseDamage;
			id = idIssuer.IssueId(this);
		}

		public void TakeDamage(int amount) {
			hp -= amount;
		}

		public void UndoTakeDamage(int amount) {
			hp += amount;
		}

		public void revive() {
			if(hp <= 0) {
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