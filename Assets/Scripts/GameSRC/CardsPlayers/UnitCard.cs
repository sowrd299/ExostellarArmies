using System.Collections.Generic;

namespace SFB.Game{
    // This file actually implements all cards that exist in the game
    public class UnitCard : Card
	{
		public int RangedAttack { get; private set; }
		public int MeleeAttack { get; private set; }
		public int HealthPoints { get; private set; }
		public List<Ability> Abilities { get; private set; }

		// abilities

		public UnitCard(int cost, string name, Faction faction, string mainText, string flavorText,
			int rangedAttack, int meleeAttack, int healthPoints, List<Ability> abList=null
		)
			: base(cost, name, faction, mainText, flavorText)
        {
			RangedAttack = rangedAttack;
			MeleeAttack = meleeAttack;
            HealthPoints = healthPoints;
			Abilities = (abList == null ? new List<Ability>() : abList);
        }

    }

}