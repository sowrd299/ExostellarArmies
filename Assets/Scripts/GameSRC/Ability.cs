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

		// play is the player that owns the unit
		public virtual Delta[] onDeath(int play) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, Lane[] lanes) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, Player[] players) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, Lane[] lanes, Player[] players) { return new Delta[] { }; }

		internal virtual Unit[,] filterTargets(Unit[,] arr, int oppPlay) { return arr; }
		public virtual int takeMeleeDamageModifier() { return 0; }
		public virtual int takeRangedDamageModifier() { return 0; }
	}

	class AbilityList : List<Ability> {
		// abilities must be in a set order because of filtering targets to damage
		// otherwise, say if you had flying and lob, depending on the order, the targets
		//     would be filtered differently - do you skip a target for
		//     lob before or after looking at only flying targets?
		// currently no clashes bc flying doesn't exist, but it could happen
		/*
		private static Type[] order = new Type[] {
			typeof(RangedShield), typeof(MeleeShield), typeof(Lob)
		};

		public AbilityList(params Ability[] abilities) : base(abilities) {
			// TODO: implement sorting here and in Add
		}*/

		public new void Add(Ability a) {
			if(this.hasType(a.GetType())) {
				if(a.Num == -1) {
					// error: trying to add a numberless ability to a unit that already has it
				} else {
					foreach(Ability ability in this) {
						if(ability.GetType() == a.GetType()) {
							ability.Num += a.Num;
							break;
						}
					}
				}
			} else {
				base.Add(a);
			}
		}

		public new void Remove(Ability a) {
			if(a.Num == -1) {
				base.Remove(a);
			} else {
				for(int i = 0; i < this.Count; i++) {
					if(this[i].GetType() == a.GetType()) {
						this[i].Num -= a.Num;
						if(this[i].Num <= 0)
							base.RemoveAt(i);
						break;
					}
				}
			}
		}

		public bool hasType(Type type) {
			return !this.TrueForAll(ability => ability.GetType() != type);
		}
	}
	
	public class MeleeShield : Ability {
		public MeleeShield(int n) : base(n) { }
		public override int takeMeleeDamageModifier() { return Num; }
	}
	public class RangedShield : Ability {
		public RangedShield(int n) : base(n) { }
		public override int takeRangedDamageModifier() { return Num; }
	}

	public class Lob : Ability {
		public Lob() : base() { }

		internal override Unit[,] filterTargets(Unit[,] arr, int oppPlay) {
			Unit[,] nArr = new Unit[arr.GetLength(0), arr.GetLength(1)];
			bool hasLobbed = false;
			for(int i = 0; i < arr.Length; i++)
				if(arr[oppPlay, i] != null) {
					if(!hasLobbed)
						hasLobbed = true;
					else
						nArr[oppPlay, i] = arr[oppPlay, i];
				}
			return nArr;
		}
	}

	// test
	public class HealAlliesWhenDie : Ability {
		private int amt;

		public HealAlliesWhenDie(int n) : base() {
			amt = n;
		}

		public override Delta[] onDeath(int play, Lane[] lanes) {
			List<Delta> deltas = new List<Delta>();
			foreach(Lane l in lanes)
				for(int pos = 0; pos < l.Units.GetLength(1); pos++)
					if(l.Units[play, pos] != null)
						deltas.Add(new UnitDelta(l.Units[play, pos], amt, UnitDelta.DamageType.HEAL));
			return deltas.ToArray();
		}
	}
}