using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
    // a class to represent a unit in play
    public class Unit : IIDed
	{
		// while all the ID code is repeated, can't use a common ancestor class
		// if we want to have seporate instance of IdIssuer for different things that need id's
		private static IdIssuer<Unit> idIssuer = new IdIssuer<Unit>();
		public static IdIssuer<Unit> IdIssuer { get { return idIssuer; } }

		// unit basic attributes
		public UnitCard Card { get; private set; } // the card the unit is an instance of

		public string MainText {
			get {
				string s = "";
				for(int i = 0; i < Abilities.Count; i++)
					s += Abilities[i].GetMainText() + (i == Abilities.Count - 1 ? "" : "\n");
				return s;
			}
		}

		public event Ability.ModifyInt RangedAtkMod;
		public int RangedAttack {
			get {
				int a = Card.RangedAttack;
				RangedAtkMod?.Invoke(ref a);
				return a;
			}
		}

		public event Ability.ModifyInt MeleeAtkMod;
		public int MeleeAttack {
			get {
				int a = Card.MeleeAttack;
				MeleeAtkMod?.Invoke(ref a);
				return a;
			}
		}

		public int HealthPoints { get; private set; }
		public bool FirstDeploy { get; private set; }

		public List<Ability> Abilities { get; private set; }

		// combat ability events
		public event Ability.FilterTargets FilterTargets;

		public event Ability.ModifyInt ModifyRangedResistance;
		public event Ability.ModifyInt ModifyMeleeResistance;
		public event Ability.ModifyInt ModifyTowerResistance;

		public event Ability.ModifyInt ModifyDamageLeft;
		public event Ability.ModifyInt ModifyTowerDamage;

		// triggered ability events
		public event Ability.AddDeltaGMLoc AddInitialDeployDeltas;
		public event Ability.AddDeltaGMLoc AddRecurringDeployDeltas;
		public event Ability.AddDeltaGMLocPhase AddDeathBeforeRemoveDeltas;

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
			this.HealthPoints = card.HealthPoints;
			
			this.FirstDeploy = true;

			this.Abilities = new List<Ability>();

			foreach(Ability a in card.Abilities)
				a.ApplyTo(this, gm);
		}
		
		public Delta[] GetDamagingDeltas(Lane l, int oppSide, Damage.Type dmgType, GameManager gm) {
			int dmgLeft = (dmgType==Damage.Type.RANGED ? RangedAttack : MeleeAttack);

			List<Delta> deltas = new List<Delta>();

			Unit[] targets = { l.Units[oppSide, 0], l.Units[oppSide, 1] };
			FilterTargets?.Invoke(targets);
			
			for(int pos = 0; dmgLeft > 0 && pos < 2; pos++) {
				if(targets[pos] != null) {
					//System.Console.WriteLine($"    Found Target at Pos{pos}");
					Unit target = targets[pos];
					int resistance = target.GetResistance(dmgType);
					int deal = System.Math.Min(target.HealthPoints+resistance, dmgLeft);
					//System.Console.WriteLine($"    Dealing{deal} to {target.Card.Name} with resist{resistance}");

					deltas.AddRange(
						UnitHealthDelta.GetDamageDeltas(
							target,
							this,
							deal,
							dmgType,
							gm
						)
					);
					dmgLeft -= deal;
					target.ModifyDamageLeft?.Invoke(ref dmgLeft);
					//System.Console.WriteLine($"    Damage Left{dmgLeft}");
				}
			}

			if(dmgLeft > 0) {
				int towerDamage = 1;
				ModifyTowerDamage?.Invoke(ref towerDamage);
				//System.Console.WriteLine($"    Hit Tower{towerDamage}");
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
					return resist;
			}
		}

		public void TakeDamage(int dmg, Damage.Type dmgType) {
			HealthPoints -= System.Math.Max(dmg - GetResistance(dmgType), 0);
        }

		public void Heal(int amt) {
			HealthPoints = System.Math.Min(Card.HealthPoints, HealthPoints+amt);
		}

		public Delta[] OnEachTurn(int lane, int side, int pos, GameManager gm) {
			List<Delta> deltas = new List<Delta>();
			
			AddRecurringDeployDeltas?.Invoke(deltas, gm.WithLocation(lane, side, pos));

			if(this.FirstDeploy) {
				AddInitialDeployDeltas?.Invoke(deltas, gm.WithLocation(lane, side, pos));
				this.FirstDeploy = false;
			}
			return deltas.ToArray();
		}

		public Delta[] OnDeathBeforeRemove(int lane, int side, int pos, GameManager gm, Damage.Type? phase) {
			List<Delta> deltas = new List<Delta>();
			AddDeathBeforeRemoveDeltas?.Invoke(deltas, gm.WithLocation(lane, side, pos), phase);
			return deltas.ToArray();
		}
	}
}