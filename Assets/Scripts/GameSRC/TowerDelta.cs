using System.Collections;
using System.Collections.Generic;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game {
	public class TowerDelta : Delta {
		private Tower tower;

		public TowerDelta(Tower t) {
			tower = t;
		}

		internal override void Apply() {
			tower.takeDamage();
		}

		internal override void Revert() {
			tower.undoTakeDamage();
		}
	}
}