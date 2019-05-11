using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Siege : Ability
	{
		public Siege(int amount)
			: base(amount)
		{}

		public override void ApplyTo(Unit u, GameState initialGameState)
		{
			void SiegeInner(ref int amt) {
				amt += Amount;
			}
			u.ModifyTowerDamage += SiegeInner;
		}

		public override void ApplyTo(GameManager gm)
		{ }
	}
}