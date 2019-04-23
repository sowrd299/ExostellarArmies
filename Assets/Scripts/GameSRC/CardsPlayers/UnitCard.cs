using System.Collections.Generic;

namespace SFB.Game{
    // This file actually implements all cards that exist in the game
    public class UnitCard : Card {
		private int rangedAttack;
		public int RangedAttack {
			get { return rangedAttack; }
		}
		
        private int meleeAttack;
        public int MeleeAttack{
            get{ return meleeAttack; }
        }

		private int healthPoints;
        public int HealthPoints{
            get{ return healthPoints; }
        }

		private AbilityList abilities;
		public AbilityList Abilities {
			get { return abilities; }
		}

        public UnitCard(int cost, string name, Faction faction, string mainText, string flavorText, int rangedAttack, int meleeAttack, int healthPoints, AbilityList abList=null)
                :base(cost, name, faction, mainText, flavorText)
        {
			this.rangedAttack = rangedAttack;
			this.meleeAttack = meleeAttack;
            this.healthPoints = healthPoints;
			this.abilities = (abList == null ? new AbilityList() : abList);
			//Debug.Log(this.abilities);
        }

    }

}