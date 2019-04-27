using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;
using System;

namespace SFB.Game{

    // a class to represent a unit in play
    public class Unit : IIDed {

        // while all the ID code is repeated, can't use a common ancestor class
        // if we want to have seporate instance of IdIssuer for different things that need id's
        public static IdIssuer<Unit> idIssuer = new IdIssuer<Unit>();

        private UnitCard card; //the card the unit is an instance of
		public UnitCard Card {
			get { return card; }
		}

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
			this.id = idIssuer.IssueId(this);
			constructor(card);
		}

		public Unit(UnitCard card, int id) {
			this.id = ""+id;
			idIssuer.RegisterId(this.id, this);
			constructor(card);
		}

		public void constructor(UnitCard card) {
			this.card = card;
			this.rangedAttack = card.RangedAttack;
			this.meleeAttack = card.MeleeAttack;
			this.healthPoints = card.HealthPoints;
			this.abilities = new AbilityList();
			foreach(Ability a in card.Abilities)
				this.abilities.Add(a);
			this.firstDeploy = true;
		}
		
		public Delta[] getRangedDamagingDeltas(Lane l, int oppPlay) {
			return getDamagingDeltas(l, oppPlay, Damage.Type.RANGED);
		}
		
		public Delta[] getMeleeDamagingDeltas(Lane l, int oppPlay) {
			return getDamagingDeltas(l, oppPlay, Damage.Type.MELEE);
		}

		private Delta[] getDamagingDeltas(Lane l, int oppPlay, Damage.Type type) {
			int dmgLeft = (type==Damage.Type.RANGED ? rangedAttack : meleeAttack);
			//Debug.Log("INTIAL DAMAGE: " + dmgLeft);

			List<Delta> list = new List<Delta>();
			int pos = 0;

			Unit[,] units = l.Units;
			foreach(Ability a in abilities)
				units = a.filterTargets(units, oppPlay);

			while(dmgLeft > 0 && pos < 2) {
				if(l.isOccupied(oppPlay, pos)) {
					Unit target = units[oppPlay, pos];
					int mod = (type == Damage.Type.RANGED
								? getTakeRangedDamageModifier()
								: (type == Damage.Type.MELEE
									? getTakeMeleeDamageModifier()
									: (type == Damage.Type.TOWER
										? getTakeTowerDamageModifier()
										: 0
										)
									)
								);
					int deal = System.Math.Min(target.HealthPoints + mod, dmgLeft);
					//Debug.Log("    DMG LEFT: " + dmgLeft);
					//Debug.Log("        DEAL: " + deal);
					//Debug.Log("        T HP: " + target.HealthPoints);
					//Debug.Log("        MOD: " + mod);

					list.Add(new UnitDelta(target, deal, type, this));
					dmgLeft = dmgLeft - deal + getDamageLeftModifier(dmgLeft, deal);
				}
				pos++;
			}

			//Debug.Log("DMG AFTER UNITS: " + dmgLeft);
			if(dmgLeft > 0)
				list.Add(new TowerDelta(l.Towers[oppPlay], 1 + getTakeTowerDamageModifier(), type));

			return list.ToArray();
		}

		public void takeRangedDamage(int dmg) {
			healthPoints -= System.Math.Max(dmg - getTakeRangedDamageModifier(), 0);
        }

        public void takeMeleeDamage(int dmg) {
			healthPoints -= System.Math.Max(dmg - getTakeMeleeDamageModifier(), 0);
        }

        public void takeTowerDamage(int dmg) {
			healthPoints -= System.Math.Max(dmg - getTakeTowerDamageModifier(), 0);
		}

		public void heal(int amt) {
			healthPoints += amt;
		}

		public int getDamageLeftModifier(int dmgLeft, int deal) {
			int sum = 0;
			foreach(Ability a in abilities)
				sum += abilities[0].takeDamageLeftModifier(dmgLeft, deal);
			return sum;
		}

		public int getDealTowerDamageModifier() {
			int sum = 0;
			foreach(Ability a in abilities)
				sum += abilities[0].dealTowerDamageModifier();
			return sum;
		}

		public int getTakeRangedDamageModifier() {
			int n = 0;
			foreach(Ability a in abilities)
				n += a.takeRangedDamageModifier();
			return n;
		}

		public int getTakeMeleeDamageModifier() {
			int n = 0;
			foreach(Ability a in abilities)
				n += a.takeMeleeDamageModifier();
			return n;
		}

		public int getTakeTowerDamageModifier() {
			int n = 0;
			foreach(Ability a in abilities)
				n += a.takeTowerDamageModifier();
			return n;
		}

		private Delta[] onInitialDeploy(int play, Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			foreach(Ability a in abilities) {
				deltas.AddRange(a.onInitialDeploy(play));
				deltas.AddRange(a.onInitialDeploy(play, lanes));
				deltas.AddRange(a.onInitialDeploy(play, players));
				deltas.AddRange(a.onInitialDeploy(play, lanes, players));
			}
			return deltas.ToArray();
		}

		public Delta[] onEachDeployPhase(int play, int pos, Lane l, Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			foreach(Ability a in abilities) {
				deltas.AddRange(a.onEachDeployPhase(play));
				deltas.AddRange(a.onEachDeployPhase(play, lanes));
				deltas.AddRange(a.onEachDeployPhase(play, players));
				deltas.AddRange(a.onEachDeployPhase(play, lanes, players));
				deltas.AddRange(a.onEachDeployPhase(play, l, this));
			}
			if(this.firstDeploy) {
				deltas.AddRange(onInitialDeploy(play, lanes, players));
				this.firstDeploy = false;
			}
			return deltas.ToArray();
		}

		public Delta[] onDeath(int play, int pos, Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			foreach(Ability a in abilities) {
				deltas.AddRange(a.onDeath(play, pos));
				deltas.AddRange(a.onDeath(play, pos, lanes));
				deltas.AddRange(a.onDeath(play, pos, players));
				deltas.AddRange(a.onDeath(play, pos, lanes, players));
				deltas.AddRange(a.onDeath(play, players, lanes, this.Card));
			}
			return deltas.ToArray();
		}
	}
}