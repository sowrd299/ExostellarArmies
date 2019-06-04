using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class SphereGreaser : Ability
	{
		public override string GetMainText() {
			return "Supporting Drone Deploy: Return the supported unit to your hand.";
		}

		public SphereGreaser() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += SphereGreaserInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= SphereGreaserInner;
		}
		
		public void SphereGreaserInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.IsFortifying(new string[] {"Drone"})) {
				deltas.AddRange(new List<Delta> {
					new RemoveFromLaneDelta(gmLoc.SubjectLane, gmLoc.Side, 0),
					new AddToHandDelta(gmLoc.SubjectPlayer.Hand, gmLoc.FrontUnit.Card)
				});
			}
		}
	}
}