using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class Shield : Ability
	{
		// Prevent <amount> damage of <damage type> type that would be done to this.

		public Shield(int amount)
			: base(amount)
		{}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			AddResistType(u);
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			RemoveResistType(u);
		}

		protected void ShieldInner(ref int amt) {
			amt += Amount;
		}

		protected abstract void AddResistType(Unit u);
		protected abstract void RemoveResistType(Unit u);
	}

	public class MeleeShield : Shield
	{
		public MeleeShield(int amount) : base(amount) { }
		protected override void AddResistType(Unit u) {
			u.ModifyMeleeResistance += ShieldInner;
		}
		protected override void RemoveResistType(Unit u) {
			u.ModifyMeleeResistance -= ShieldInner;
		}
	}

	public class RangedShield : Shield {
		public RangedShield(int amount) : base(amount) { }
		protected override void AddResistType(Unit u) {
			u.ModifyRangedResistance += ShieldInner;
		}
		protected override void RemoveResistType(Unit u) {
			u.ModifyRangedResistance -= ShieldInner;
		}
	}

	public class TowerShield : Shield {
		public TowerShield(int amount) : base(amount) { }
		protected override void AddResistType(Unit u) {
			u.ModifyTowerResistance += ShieldInner;
		}
		protected override void RemoveResistType(Unit u) {
			u.ModifyTowerResistance -= ShieldInner;
		}
	}
}