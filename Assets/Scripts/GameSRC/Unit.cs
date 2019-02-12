using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

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
        }

		// p is other player
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

		// p is other player
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
    }

	// preliminary, might not be an enum if theres stuff like (Kicker X)
	enum Attribute {
		LOB, 
	}

}