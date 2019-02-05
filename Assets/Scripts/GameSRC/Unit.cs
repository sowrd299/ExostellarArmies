using SFB.Game.Management;

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

        public Unit(UnitCard card){
            this.card = card;
            this.id = idIssuer.IssueId(this);
			this.rangedAttack = card.RangedAttack;
			this.meleeAttack = card.MeleeAttack;
			this.healthPoints = card.HealthPoints;
        }

    }

}