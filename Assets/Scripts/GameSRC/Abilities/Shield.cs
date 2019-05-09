using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class Shield : Ability
	{
		public Shield(int amount)
			: base(amount)
		{}

		public override void ApplyTo(Unit u)
		{
			AddResistType(u);
		}
		public override void ApplyTo(GameManager gm)
		{ }

		protected void ShieldInner(ref int amt) {
			amt += Amount;
		}

		protected abstract void AddResistType(Unit u);
	}

	public class MeleeShield : Shield
	{
		public MeleeShield(int amount) : base(amount) { }
		protected override void AddResistType(Unit u)
		{
			u.ModifyMeleeResistance += ShieldInner;
		}
	}

	public class RangedShield : Shield {
		public RangedShield(int amount) : base(amount) { }
		protected override void AddResistType(Unit u) {
			u.ModifyRangedResistance += ShieldInner;
		}
	}

	public class TowerShield : Shield {
		public TowerShield(int amount) : base(amount) { }
		protected override void AddResistType(Unit u) {
			u.ModifyTowerResistance += ShieldInner;
		}
	}
}