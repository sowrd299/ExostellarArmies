using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class PersistentAbility : Ability
	{
		public PersistentAbility(int amount)
			: base(amount)
		{ }

		public override void ApplyTo(Unit u)
		{
			//u.
			Driver.instance.gameManager.AddBoardUpdateDeltas += RemoveEffect();
			Driver.instance.gameManager.AddBoardUpdateDeltas += AddEffect();
		}

		protected abstract AddDelta AddEffect();
		protected abstract AddDelta RemoveEffect();
	}
}