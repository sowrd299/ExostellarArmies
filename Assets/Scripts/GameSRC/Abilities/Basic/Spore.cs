using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Spore : Ability
	{
		public Spore(int amount)
			: base(amount)
		{}

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.AddDeathDeltas += SporeInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			u.AddDeathDeltas -= SporeInner;
		}

		void SporeInner(List<Delta> deltas, GameStateLocation gameStateLoc)
		{
			deltas.AddRange(gameStateLoc.SubjectPlayer.ManaPool.GetAddDeltas(Amount));
		}
	}
}