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

	public class AbilityList : List<Ability> {
		
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

	public class Absorb : Ability {
		public Absorb() : base() { }
		public override int takeDamageLeftModifier(int dmgLeft, int deal) { return -dmgLeft; }
	}

	public class Spore : Ability {
		public Spore(int n) : base(n) { }
		public override Delta[] onDeath(int play, int pos, Player[] players) { return players[play].Mana.GetAddDeltas(Num); }
	}

	// BetaSwarm???

	// say there were a unit that would always stay in the backline, lob would still lob over it
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
	
	public class MeleeShield : Ability {
		public MeleeShield(int n) : base(n) { }
		public override int takeMeleeDamageModifier() { return Num; }
	}
	public class RangedShield : Ability {
		public RangedShield(int n) : base(n) { }
		public override int takeRangedDamageModifier() { return Num; }
	}
	public class TowerShield : Ability {
		public TowerShield(int n) : base(n) { }
		public override int takeTowerDamageModifier() { return Num; }
	}

	public abstract class Rush : Ability {
		internal override Delta[] onEachDeployPhase(int play, Lane l, Unit u) {
			return (u != l.Units[play, rushTo()] ? l.getSwapPositionDeltas(play) : new Delta[0]);
		}
		public abstract int rushTo();
	}
	public class RushFront : Rush { public override int rushTo() { return 0; } }
	public class RushBack : Rush { public override int rushTo() { return 1; } }


	public class Siege : Ability {
		public Siege(int n) : base(n) { }

		public override int dealTowerDamageModifier() {
			return Num;
		}
	}

	public class Regrowth : Ability {
		private int play;
		private Func<Player[], Lane[], bool> func;

		public Regrowth(int p, Func<Player[], Lane[], bool> f) {
			play = p;
			func = f;
		}

		public override Delta[] onDeath(int play, Player[] players, Lane[] lanes, Card c) {
			List<Delta> l = new List<Delta>();

			if(func(players, lanes))
				l.Add(new Hand.AddToHandDelta(players[play].Hand, c));

			return l.ToArray();
		}
	}
/*
	public class Swarm : Ability {
		public override Delta[] onInitialDeploy(int play, Lane[] lanes, Player[] players) {
			return new Delta[] { };
		}
	}
*/


	// test
	public class HealAlliesWhenDie : Ability {
		public HealAlliesWhenDie(int n) : base(n) { }

		public override Delta[] onDeath(int play, int pos, Lane[] lanes) {
			List<Delta> deltas = new List<Delta>();
			foreach(Lane l in lanes)
				for(int p = 0; p < l.Units.GetLength(1); p++)
					if(l.Units[play, p] != null)
						deltas.Add(new UnitDelta(l.Units[play, p], Num, UnitDelta.DamageType.HEAL, null));
			return deltas.ToArray();
		}
	}
}