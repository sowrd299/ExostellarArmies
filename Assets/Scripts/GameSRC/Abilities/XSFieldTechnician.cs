using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Supporting Drone Deploy: Return the supported unit to your hand.

	public class XSFieldTechnician : Ability
	{
		public XSFieldTechnician() : base(-1) { }

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.AddInitialDeployDeltas += XSFieldTechnicianInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
		{
			u.AddInitialDeployDeltas -= XSFieldTechnicianInner;
		}
		
		public void XSFieldTechnicianInner(List<Delta> deltas, GameStateLocation gameStateLocation)
		{
			if(gameStateLocation.IsSupporting(new string[] {"Drone"})) {
				deltas.AddRange(new List<Delta> {
					new RemoveFromLaneDelta(gameStateLocation.SubjectLane, gameStateLocation.Side, 0),
					new AddToHandDelta(gameStateLocation.SubjectPlayer.Hand, gameStateLocation.FrontUnit.Card)
				});
			}
		}
	}
}