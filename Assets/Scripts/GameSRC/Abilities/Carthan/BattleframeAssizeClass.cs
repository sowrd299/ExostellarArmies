using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class BattleframeAssizeClass : Ability
	{
		public override string GetMainText() {
			return "Recurring Deploy: Gain 1 resource";
		}

		public BattleframeAssizeClass() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas += AdvInfantrySupportSystemInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas -= AdvInfantrySupportSystemInner;
		}

		public void AdvInfantrySupportSystemInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			deltas.AddRange(gmLoc.SubjectPlayer.ManaPool.GetAddDeltas(1));
		}
	}
}