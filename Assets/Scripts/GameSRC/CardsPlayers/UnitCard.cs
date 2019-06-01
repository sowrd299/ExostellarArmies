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
			int rangedAttack, int meleeAttack, int healthPoints, params Ability[] abList
		)
			: base(cost, name, faction, mainText, flavorText)
        {
			RangedAttack = rangedAttack;
			MeleeAttack = meleeAttack;
            HealthPoints = healthPoints;
			Abilities = new List<Ability>();
			foreach(Ability a in abList)
				Abilities.Add(a);
			UnitType = unitType;
        }

		public override string ToString() {
			return $"name({Name}) cost({DeployCost}) faction({Faction}) type({UnitType}) " +
				$"main({MainText}) flavor({FlavorText}) " +
				$"r({RangedAttack}) m({MeleeAttack}) hp({HealthPoints}) a({Abilities.Count})";
		}
	}
}