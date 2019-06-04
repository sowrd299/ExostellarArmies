using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Spore : Ability
	{
		// When this dies, gain <amount> resource(s)
		public override string GetMainText() {
			return $"Spore {Amount}";
		}

		public Spore(int amount) : base(amount) {}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddDeathDeltas += SporeInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddDeathDeltas -= SporeInner;
		}

		void SporeInner(List<Delta> deltas, GMWithLocation gameStateLoc, Damage.Type? phase)
		{
			deltas.AddRange(gameStateLoc.SubjectPlayer.ManaPool
													  .GetAddDeltas(Amount));
		}
	}
}