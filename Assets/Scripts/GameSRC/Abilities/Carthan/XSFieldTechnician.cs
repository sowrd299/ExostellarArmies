using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Supporting Drone Deploy: Return the supported unit to your hand.

	public class XSFieldTechnician : Ability
	{
		public XSFieldTechnician() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += XSFieldTechnicianInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= XSFieldTechnicianInner;
		}
		
		public void XSFieldTechnicianInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.IsSupporting(new string[] {"Drone"})) {
				deltas.AddRange(new List<Delta> {
					new RemoveFromLaneDelta(gmLoc.SubjectLane, gmLoc.Side, 0),
					new AddToHandDelta(gmLoc.SubjectPlayer.Hand, gmLoc.FrontUnit.Card)
				});
			}
		}
	}
}