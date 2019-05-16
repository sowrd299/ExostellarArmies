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

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.ModifyTowerDamage += SiegeInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			u.ModifyTowerDamage -= SiegeInner;
		}

		public void SiegeInner(ref int amt) {
			amt += Amount;
		}
	}
}