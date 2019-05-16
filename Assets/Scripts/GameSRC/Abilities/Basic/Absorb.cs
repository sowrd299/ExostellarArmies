using SFB.Game.Management;

namespace SFB.Game
{
	public class Absorb : Ability
	{
		public Absorb()
			: base(-1)
		{ }

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.ModifyDamageLeft += AbsorbInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			u.ModifyDamageLeft -= AbsorbInner;
		}

		public void AbsorbInner(ref int amt)
		{
			amt = 0;
		}
	}
}