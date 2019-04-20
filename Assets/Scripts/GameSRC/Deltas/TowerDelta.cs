using System.Collections;
using System.Collections.Generic;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game {
	public class TowerDelta : Delta {
		private Tower tower;
		private int amount;
		public int Amount {
			get { return amount; }
		}

		public TowerDelta(Tower t, int amt) {
			tower = t;
			amount = amt;
		}

		internal override void Apply() {
			tower.takeDamage(amount);
		}

		internal override void Revert() {
			tower.undoTakeDamage(amount);
		}
	}
}