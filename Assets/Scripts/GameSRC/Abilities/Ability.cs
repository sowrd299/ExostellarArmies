using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class Ability
	{
		public delegate void AddDelta(
			List<Delta> deltas, int side, int pos, int lane,
			Lane[] lanes, Player[] players
		);
		public delegate void FilterTargets(Unit[] targets);
		public delegate void ModifyInt(ref int amt);
		
		public int Amount { get; private set; }

		public Ability(int amount) {
			Amount = amount;
		}

		public abstract void ApplyTo(Unit u);
	}
}