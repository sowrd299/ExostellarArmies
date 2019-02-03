namespace SFB.Game{

    // this file actually implements all cards that exist in the game
    class UnitCard : Card{

		private int rangedAttack;
		public int RangedAttack {
			get { return rangedAttack; }
		}

		private int baseRangedAttack;
        public int BaseRangedAttack{
            get{ return baseRangedAttack; }
        }

        // damage dealt in melee combat
        private int meleeAttack;
        public int MeleeAttack{
            get{ return meleeAttack; }
        }

		private int baseMeleeAttack;
		public int BaseMeleeAttack {
			get { return baseMeleeAttack; }
		}

		private int healthPoints;
        public int HealthPoints{
            get{ return healthPoints; }
        }

        public UnitCard(int cost, string name, Faction faction, string mainText, string flavorText, int rangedAttack, int meleeAttack, int healthPoints)
                :base(cost, name, faction, mainText, flavorText)
        {
            this.baseRangedAttack = rangedAttack;
			this.rangedAttack = rangedAttack;
			this.baseMeleeAttack = meleeAttack;
			this.meleeAttack = meleeAttack;
            this.healthPoints = healthPoints;
        }

    }

}