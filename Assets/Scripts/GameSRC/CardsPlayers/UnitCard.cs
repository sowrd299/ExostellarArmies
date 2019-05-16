using System.Collections.Generic;

namespace SFB.Game{
    // This file actually implements all cards that exist in the game
    public class UnitCard : Card
	{
		public int RangedAttack { get; private set; }
		public int MeleeAttack { get; private set; }
		public int HealthPoints { get; private set; }

		public List<Ability> Abilities { get; private set; }

		public string UnitType { get; private set; }

		public UnitCard(int cost, string name, Faction faction, string unitType, string mainText, string flavorText,
			int rangedAttack, int meleeAttack, int healthPoints, List<Ability> abList=null
		)
			: base(cost, name, faction, mainText, flavorText)
        {
			RangedAttack = rangedAttack;
			MeleeAttack = meleeAttack;
            HealthPoints = healthPoints;
			Abilities = abList ?? new List<Ability>();
			UnitType = unitType;
        }

    }

}