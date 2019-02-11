using SFB.Game.Management;
using SFB.Game.Content;

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
		public Delta getRangedDamagingDelta(Lane l, int p) {
			if(rangedAttack == 0)
				return null;
			
			//TODO switch statement for attrs like LOB
			for(int i = 0; i < 2; i++)
				if(l.isOccupied(p, i))
					return new UnitDelta(l.Unit(p, i), rangedAttack, l);

			return new TowerDelta(l.Tower(p));
		}

		// p is other player
		public Delta getMeleeDamagingDelta(Lane l, int p) {
			if(meleeAttack == 0)
				return null;

			for(int i = 0; i < 2; i++)
				if(l.isOccupied(p, i))
					return new UnitDelta(l.Unit(p, i), meleeAttack, l);

			return new TowerDelta(l.Tower(p));
		}

		public void takeDamage(int dmg, Lane l) {
			healthPoints -= dmg;
			if(healthPoints <= 0)
				l.kill(this);
		}
    }

	// preliminary, might not be an enum if theres stuff like (Kicker X)
	enum Attribute {
		LOB, 
	}

}