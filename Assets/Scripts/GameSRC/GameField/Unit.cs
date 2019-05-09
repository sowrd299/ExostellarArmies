using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
    // a class to represent a unit in play
    public class Unit : IIDed {
		// while all the ID code is repeated, can't use a common ancestor class
		// if we want to have seporate instance of IdIssuer for different things that need id's
		private static IdIssuer<Unit> idIssuer = new IdIssuer<Unit>();
		public static IdIssuer<Unit> IdIssuer { get { return idIssuer; } }

		// unit basic attributes
		public UnitCard Card { get; private set; } // the card the unit is an instance of

		public int RangedAttack { get; set; } // make these call gamemanger modify event
		public int MeleeAttack { get; set; }
		public int HealthPoints { get; set; }

		public bool FirstDeploy { get; private set; }
		
		// combat abilities
		public event Ability.FilterTargets FilterTargets;

		public event Ability.ModifyInt ModifyRangedResistance;
		public event Ability.ModifyInt ModifyMeleeResistance;
		public event Ability.ModifyInt ModifyTowerResistance;

		public event Ability.ModifyInt ModifyDamageLeft;
		public event Ability.ModifyInt ModifyTowerDamage;

		// triggered abilities
		public event Ability.AddDelta AddInitialDeployDeltas;
		public event Ability.AddDelta AddRecurringDeployDeltas;
		public event Ability.AddDelta AddDeathDeltas;

		readonly public string id;
        public string ID {
            get{ return id; }
        }

        public Unit(UnitCard card, GameManager gm) {
			this.id = IdIssuer.IssueId(this);
			Constructor(card, gm);
		}

		public Unit(UnitCard card, int id, GameManager gm) {
			this.id = ""+id;
			IdIssuer.RegisterId(this.id, this);
			Constructor(card, gm);
		}

		private void Constructor(UnitCard card, GameManager gm) {
			this.Card = card;
			this.RangedAttack = card.RangedAttack;
			this.MeleeAttack = card.MeleeAttack;
			this.HealthPoints = card.HealthPoints;
			
			this.FirstDeploy = true;

			foreach(Ability a in card.Abilities) {
				a.ApplyTo(this);
				a.ApplyTo(gm);
			}
		}
		
		public Delta[] GetRangedDamagingDeltas(Lane l, int oppPlay) {
			return GetDamagingDeltas(l, oppPlay, Damage.Type.RANGED);
		}
		
		public Delta[] GetMeleeDamagingDeltas(Lane l, int oppPlay) {
			return GetDamagingDeltas(l, oppPlay, Damage.Type.MELEE);
		}
		
		public Delta[] GetDamagingDeltas(Lane l, int oppSide, Damage.Type dmgType) {
			int dmgLeft = (dmgType==Damage.Type.RANGED ? RangedAttack : MeleeAttack);

			List<Delta> deltas = new List<Delta>();

			Unit[] targets = { l.Units[oppSide, 0], l.Units[oppSide, 1] };
			FilterTargets?.Invoke(targets);

			int pos = 0;
			while(dmgLeft > 0 && pos < 2) {
				if(targets[pos] != null) {
					Unit target = targets[pos];
					int resistance = target.GetResistance(dmgType);
					int deal = System.Math.Min(target.HealthPoints+resistance, dmgLeft);

					deltas.Add(new UnitDamageDelta(target, deal, dmgType, this));
					dmgLeft = dmgLeft - deal;
					target.ModifyDamageLeft?.Invoke(ref dmgLeft);
				}
				pos++;
			}

			if(dmgLeft > 0) {
				int towerDamage = 1;
				ModifyTowerDamage?.Invoke(ref towerDamage);
				deltas.AddRange(l.Towers[oppSide].GetDamageDeltas(towerDamage, dmgType));
			}

			return deltas.ToArray();
		}

		public int GetResistance(Damage.Type dmgType) {
			int resist = 0;
			switch(dmgType) {
				case Damage.Type.RANGED:
					ModifyRangedResistance?.Invoke(ref resist);
					return resist;
				case Damage.Type.MELEE:
					ModifyMeleeResistance?.Invoke(ref resist);
					return resist;
				case Damage.Type.TOWER:
					ModifyTowerResistance?.Invoke(ref resist);
					return resist;
				default:
					throw new System.Exception($"Invalid Damage Type: {dmgType}");
			}
		}

		public void TakeDamage(int dmg, Damage.Type dmgType) {
			HealthPoints -= System.Math.Max(dmg - GetResistance(dmgType), 0);
        }

		public void Heal(int amt) {
			HealthPoints += amt;
		}

		public Delta[] OnEachDeployPhase(int side, int pos, int lane, Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			
			AddRecurringDeployDeltas?.Invoke(deltas, side, pos, lane, lanes, players);

			if(this.FirstDeploy) {
				AddInitialDeployDeltas?.Invoke(deltas, side, pos, lane, lanes, players);
				this.FirstDeploy = false;
			}
			return deltas.ToArray();
		}

		public Delta[] OnDeath(int side, int pos, int lane, Lane[] lanes, Player[] players) {
			List<Delta> deltas = new List<Delta>();
			AddDeathDeltas?.Invoke(deltas, side, pos, lane, lanes, players);
			return deltas.ToArray();
		}
	}
}