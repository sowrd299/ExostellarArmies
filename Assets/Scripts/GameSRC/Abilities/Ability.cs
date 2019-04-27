using System;
using System.Collections.Generic;
using SFB.Game.Content;
using SFB.Game.Management;

namespace SFB.Game {
	public abstract class Ability {
		private int num;
		public int Num {
			get; set;
		}

		public Ability(int num) {
			this.num = num;
		}

		public Ability() : this(-1) { }

		public virtual Delta[] onInitialDeploy(int play) { return new Delta[] { }; }
		public virtual Delta[] onInitialDeploy(int play, Lane[] lanes) { return new Delta[] { }; }
		public virtual Delta[] onInitialDeploy(int play, Player[] players) { return new Delta[] { }; }
		public virtual Delta[] onInitialDeploy(int play, Lane[] lanes, Player[] players) { return new Delta[] { }; }

		public virtual Delta[] onEachDeployPhase(int play) { return new Delta[] { }; }
		public virtual Delta[] onEachDeployPhase(int play, Lane[] lanes) { return new Delta[] { }; }
		public virtual Delta[] onEachDeployPhase(int play, Player[] players) { return new Delta[] { }; }
		public virtual Delta[] onEachDeployPhase(int play, Lane[] lanes, Player[] players) { return new Delta[] { }; }
		internal virtual Delta[] onEachDeployPhase(int play, Lane l, Unit u) { return new Delta[] { }; }

		// play is the player that owns the unit
		public virtual Delta[] onDeath(int play, int pos) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, int pos, Lane[] lanes) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, int pos, Player[] players) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, int pos, Lane[] lanes, Player[] players) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, Player[] players, Lane[] lanes, Card c) { return new Delta[] { }; }

		internal virtual Unit[,] filterTargets(Unit[,] arr, int oppPlay) { return arr; }
		public virtual int takeMeleeDamageModifier() { return 0; }
		public virtual int takeRangedDamageModifier() { return 0; }
		public virtual int takeTowerDamageModifier() { return 0; }
		public virtual int takeDamageLeftModifier(int dmgLeft, int deal) { return 0; }
		public virtual int dealTowerDamageModifier() { return 0; }
	}

	// test
	public class HealAlliesWhenDie : Ability {
		public HealAlliesWhenDie(int n) : base(n) { }

		public override Delta[] onDeath(int play, int pos, Lane[] lanes) {
			List<Delta> deltas = new List<Delta>();
			foreach(Lane l in lanes)
				for(int p = 0; p < l.Units.GetLength(1); p++)
					if(l.Units[play, p] != null)
						deltas.Add(new UnitDelta(l.Units[play, p], Num, Damage.Type.HEAL, null));
			return deltas.ToArray();
		}
	}
}