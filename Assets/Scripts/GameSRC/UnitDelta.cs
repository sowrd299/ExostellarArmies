using System.Collections;
using System.Collections.Generic;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game {
	public class UnitDelta : Delta {
		private Unit unit;
		internal Unit Unit {
			get { return unit; }
		}

		private int dHP;
		public int DHP {
			get { return dHP; }
		}

		internal UnitDelta(Unit u, int h) {
			unit = u;
			dHP = h;
		}

		internal override void Apply() {
			unit.takeDamage(-dHP);
		}

		internal override void Revert() {
			unit.takeDamage(dHP);
		}
	}
}
