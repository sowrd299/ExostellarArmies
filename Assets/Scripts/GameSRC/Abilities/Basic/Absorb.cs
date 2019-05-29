using SFB.Game.Management;

namespace SFB.Game
{
	public class Absorb : Ability
	{
		// Excess damage done to this does not overflow.

		public Absorb()
			: base(-1)
		{ }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.ModifyDamageLeft += AbsorbInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.ModifyDamageLeft -= AbsorbInner;
		}

		public void AbsorbInner(ref int amt)
		{
			amt = 0;
		}
	}
}