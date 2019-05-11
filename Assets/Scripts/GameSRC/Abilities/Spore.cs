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

		public override void ApplyTo(Unit u, GameState initialGameState)
		{
			void SporeInner(List<Delta> deltas, GameStateLocation gameStateLoc) {
				deltas.AddRange(gameStateLoc.SubjectPlayer.ManaPool.GetAddDeltas(Amount));
			}
			u.AddDeathDeltas += SporeInner;
		}
	}
}