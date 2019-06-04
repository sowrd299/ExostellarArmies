using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Siege : Ability
	{
		public override string GetMainText() {
			return $"Siege {Amount}";
		}
		// After this deals damage to a tower, it deals <amount> extra damage to it.

		public Siege(int amount) : base(amount) {}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.ModifyTowerDamage += SiegeInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.ModifyTowerDamage -= SiegeInner;
		}

		public void SiegeInner(ref int amt) {
			amt += Amount;
		}
	}
}