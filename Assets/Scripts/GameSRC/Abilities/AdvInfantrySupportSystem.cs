using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class AdvInfantrySupportSystem : Ability
	{
		// Front or Back Line: At the start of your turn, generate an extra resource.

		public AdvInfantrySupportSystem() : base(-1) { }

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