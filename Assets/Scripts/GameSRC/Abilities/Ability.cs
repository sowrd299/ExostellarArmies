using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public abstract class Ability
	{
		public delegate void AddDelta(List<Delta> deltas, GameStateLocation gameStateLocation);
		public delegate void FilterTargets(Unit[] targets);
		public delegate void ModifyInt(ref int amt);
		
		public int Amount { get; private set; }

		public Ability(int amount)
		{
			Amount = amount;
		}

		// gm may be null
		public abstract void ApplyTo(Unit u, GameState initialGameState);
	}
}