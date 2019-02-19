using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;
using System;

namespace SFB.Game{

    // a class to represent a unit in play
    class Unit : IIDed {

        // while all the ID code is repeated, can't use a common ancestor class
        // if we want to have seporate instance of IdIssuer for different things that need id's
        private static IdIssuer<Unit> idIssuer = new IdIssuer<Unit>();

        private UnitCard card; //the card the unit is an instance of

		private int rangedAttack;
		public int RangedAttack {
			get { return rangedAttack; }
		}

		private int meleeAttack;
		public int MeleeAttack {
			get { return meleeAttack; }
		}

		private int healthPoints;
		public int HealthPoints {
			get { return healthPoints; }
		}

		private AbilityList abilities;
		public AbilityList Abilities {
			get; set;
		}

		private bool firstDeploy;
		public bool FirstDeploy {
			get; set;
		}

		readonly public string id;
        public string ID {
            get{ return id; }
        }

        public Unit(UnitCard card) {
            this.card = card;
            this.id = idIssuer.IssueId(this);
			this.rangedAttack = card.RangedAttack;
			this.meleeAttack = card.MeleeAttack;
			this.healthPoints = card.HealthPoints;
			this.abilities = new AbilityList();
			foreach(Ability a in card.Abilities)
				this.abilities.Add(a);
			this.firstDeploy = true;
        }

		// play is other player
		public List<Delta> getRangedDamagingDelta(Lane l, int play) {
			int dmgLeft = rangedAttack;

			List<Delta> list = new List<Delta>();
			int pos = 0;

			//TODO switch statement for attrs like LOB
			while(dmgLeft > 0 && pos < 2) {
				if(l.isOccupied(play, pos)) {
					Unit target = l.Units[play, pos];
					int deal = System.Math.Max(target.HealthPoints, dmgLeft);
					list.Add(new UnitDelta(target, deal));
					dmgLeft -= deal;
				}
				pos++;
			}

			if(dmgLeft > 0)
				list.Add(new TowerDelta(l.Towers[play]));
			
			return list;
		}

		// play is other player
		public List<Delta> getMeleeDamagingDelta(Lane l, int play) {
			int dmgLeft = meleeAttack;

			List<Delta> list = new List<Delta>();
			int pos = 0;

			//TODO switch statement for attrs like LOB
			while(dmgLeft > 0 && pos < 2) {
				if(l.isOccupied(play, pos)) {
					Unit target = l.Units[play, pos];
					int deal = System.Math.Max(target.HealthPoints, dmgLeft);
					list.Add(new UnitDelta(target, deal));
					dmgLeft -= deal;
				}
				pos++;
			}

			if(dmgLeft > 0)
				list.Add(new TowerDelta(l.Towers[play]));

			return list;
		}

		public void takeDamage(int dmg) {
			healthPoints -= dmg;
		}

		private Delta[] onInitialDeploy(Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			foreach(Ability a in abilities) {
				deltas.AddRange(a.onInitialDeploy());
				deltas.AddRange(a.onInitialDeploy(lanes));
				deltas.AddRange(a.onInitialDeploy(players));
				deltas.AddRange(a.onInitialDeploy(lanes, players));
			}
			return deltas.ToArray();
		}

		public Delta[] onEachDeployPhase(Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			foreach(Ability a in abilities) {
				deltas.AddRange(a.onEachDeployPhase());
				deltas.AddRange(a.onEachDeployPhase(lanes));
				deltas.AddRange(a.onEachDeployPhase(players));
				deltas.AddRange(a.onEachDeployPhase(lanes, players));
			}
			if(this.firstDeploy) {
				deltas.AddRange(onInitialDeploy(lanes, players));
				this.firstDeploy = false;
			}
			return deltas.ToArray();
		}

		public Delta[] onDeath(Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			foreach(Ability a in abilities) {
				deltas.AddRange(a.onDeath());
				deltas.AddRange(a.onDeath(lanes));
				deltas.AddRange(a.onDeath(players));
				deltas.AddRange(a.onDeath(lanes, players));
			}
			return deltas.ToArray();
		}
	}

	class AbilityList : List<Ability> {

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

		public bool hasType(Type type) {
			return !this.TrueForAll(ability => ability.GetType() != type);
		}
	}
	
	abstract class Ability {
		private int num;
		public int Num {
			get; set;
		}

		public Ability(int num) {
			this.num = num;
		}

		public Ability(): this(-1) { }

		public virtual Delta[] onInitialDeploy() { return new Delta[] { }; }
		public virtual Delta[] onInitialDeploy(Lane[] lanes) { return new Delta[] { }; }
		public virtual Delta[] onInitialDeploy(Player[] players) { return new Delta[] { }; }
		public virtual Delta[] onInitialDeploy(Lane[] lanes, Player[] players) { return new Delta[] { }; }

		public virtual Delta[] onEachDeployPhase() { return new Delta[] { }; }
		public virtual Delta[] onEachDeployPhase(Lane[] lanes) { return new Delta[] { }; }
		public virtual Delta[] onEachDeployPhase(Player[] players) { return new Delta[] { }; }
		public virtual Delta[] onEachDeployPhase(Lane[] lanes, Player[] players) { return new Delta[] { }; }

		public virtual Delta[] onDeath(int play) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, Lane[] lanes) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, Player[] players) { return new Delta[] { }; }
		public virtual Delta[] onDeath(int play, Lane[] lanes, Player[] players) { return new Delta[] { }; }
	}

	class Lob : Ability {
		public Lob() : base() { }
	}

	// test
	class HealAlliesWhenDie : Ability {
		public HealAlliesWhenDie() : base() { }

		public override Delta[] onDeath(int play, Lane[] lanes) {
			List<Delta> deltas = new List<Delta>();
			foreach(Lane l in lanes)
				for(int pos = 0; pos < l.Units.GetLength(1); pos++)
					if(l.Units[play, pos] != null)
						deltas.Add(new UnitDelta(l.Units[play, pos], 1));
			return deltas.ToArray();
		}
	}
}