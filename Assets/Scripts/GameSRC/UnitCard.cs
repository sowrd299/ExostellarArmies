namespace SFB.Game{

    // this file actually implements all cards that exist in the game
    class UnitCard : Card{

        private int rangedAttack;
        public int RangedAttack{
            get{ return rangedAttack; }
        }

        // damage dealt in melee combat
        private int meleeAttack;
        public int MeleeAttack{
            get{ return meleeAttack; }
        }

        private int healthPoints;
        public int HealthPoints{
            get{ return healthPoints; }
        }

        public UnitCard(int cost, string name, Faction faction, int rangedAttack, int meleeAttack, int healthPoints)
                :base(cost, name, faction)
        {
            this.rangedAttack = rangedAttack;
            this.meleeAttack = meleeAttack;
            this.healthPoints = healthPoints;
        }

    }

}