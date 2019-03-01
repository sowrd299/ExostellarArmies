using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace SFB.Game{

    // a class to represent a unit in play
    class Unit : IIDed {

        // while all the ID code is repeated, can't use a common ancestor class
        // if we want to have seporate instance of IdIssuer for different things that need id's
        private static IdIssuer<Unit> idIssuer = new IdIssuer<Unit>();

        private UnitCard card; //the card the unit is an instance of
		public UnitCard Card {
			get;
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
            this.card = card;
            this.id = idIssuer.IssueId(this);
			this.rangedAttack = card.RangedAttack;
			this.meleeAttack = card.MeleeAttack;
			this.healthPoints = card.HealthPoints;
			this.abilities = new AbilityList();
            Debug.Log("Ab==nul:" + (card.Abilities == null));
			//foreach(Ability a in card.Abilities)
				//this.abilities.Add(a);
			this.firstDeploy = true;
        }
		
		public Delta[] getRangedDamagingDelta(Lane l, int oppPlay) {
			return getDamagingDeltas(l, oppPlay, UnitDelta.DamageType.RANGED);
		}
		
		public Delta[] getMeleeDamagingDelta(Lane l, int oppPlay) {
			return getDamagingDeltas(l, oppPlay, UnitDelta.DamageType.MELEE);
		}

		private Delta[] getDamagingDeltas(Lane l, int oppPlay, UnitDelta.DamageType type) {
			int dmgLeft = (type==UnitDelta.DamageType.RANGED ? rangedAttack : meleeAttack);

			List<Delta> list = new List<Delta>();
			int pos = 0;

			Unit[,] units = l.Units;
			foreach(Ability a in abilities)
				units = a.filterTargets(units, oppPlay);

			while(dmgLeft > 0 && pos < 2) {
				if(l.isOccupied(oppPlay, pos)) {
					Unit target = units[oppPlay, pos];
					int mod = (type == UnitDelta.DamageType.RANGED
								? getRangedDamageModifier()
								: (type == UnitDelta.DamageType.MELEE
									? getMeleeDamageModifier()
									: 0));
					int deal = System.Math.Max(target.HealthPoints + mod, dmgLeft);
					list.Add(new UnitDelta(target, deal, type));
					dmgLeft = dmgLeft - deal + getDamageLeftModifier(dmgLeft, deal);
				}
				pos++;
			}

			if(dmgLeft > 0)
				list.Add(new TowerDelta(l.Towers[oppPlay], 1 + getTowerDamageModifier()));

			return list.ToArray();
		}

		public void takeRangedDamage(int dmg) {
			healthPoints -= System.Math.Max(dmg - getRangedDamageModifier(), 0);
		}

		public void takeMeleeDamage(int dmg) {
			healthPoints -= System.Math.Max(dmg - getMeleeDamageModifier(), 0);
		}

		public void takeTrueDamage(int dmg) {
			healthPoints -= dmg;
		}

		public void heal(int amt) {
			healthPoints += amt;
		}

		public int getDamageLeftModifier(int dmgLeft, int deal) {
			int sum = 0;
			foreach(Ability a in abilities)
				sum += abilities[0].getDamageLeftModifier(dmgLeft, deal);
			return sum;
		}

		public int getTowerDamageModifier() {
			int sum = 0;
			foreach(Ability a in abilities)
				sum += abilities[0].getTowerDamageModifier();
			return sum;
		}

		public int getRangedDamageModifier() {
			int n = 0;
			foreach(Ability a in abilities)
				n += a.takeRangedDamageModifier();
			return n;
		}

		public int getMeleeDamageModifier() {
			int n = 0;
			foreach(Ability a in abilities)
				n += a.takeMeleeDamageModifier();
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

		public Delta[] onEachDeployPhase(int play, Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			foreach(Ability a in abilities) {
				deltas.AddRange(a.onEachDeployPhase(play));
				deltas.AddRange(a.onEachDeployPhase(play, lanes));
				deltas.AddRange(a.onEachDeployPhase(play, players));
				deltas.AddRange(a.onEachDeployPhase(play, lanes, players));
			}
			if(this.firstDeploy) {
				deltas.AddRange(onInitialDeploy(play, lanes, players));
				this.firstDeploy = false;
			}
			return deltas.ToArray();
		}

		public Delta[] onDeath(int play, Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			foreach(Ability a in abilities) {
				deltas.AddRange(a.onDeath(play));
				deltas.AddRange(a.onDeath(play, lanes));
				deltas.AddRange(a.onDeath(play, players));
				deltas.AddRange(a.onDeath(play, lanes, players));
			}
			return deltas.ToArray();
		}
	}
}