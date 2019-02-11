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

		private Lane lane;
		public Lane Lane {
			get { return lane; }
		}

		internal UnitDelta(Unit u, int h, Lane l) {
			unit = u;
			dHP = h;
			lane = l;
		}

		internal override void Apply() {
			unit.takeDamage(-dHP, lane);
		}

		internal override void Revert() {
			
		}
	}
}
