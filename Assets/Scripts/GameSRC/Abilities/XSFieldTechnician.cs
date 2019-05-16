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
			Unit front = gameStateLocation.FrontUnit;
			if(gameStateLocation.Pos == 1 && front != null && front.Card.UnitType.Contains("Drone")) {
				deltas.AddRange(new List<Delta> {
					new RemoveFromLaneDelta(gameStateLocation.SubjectLane, gameStateLocation.Side, 0),
					new AddToHandDelta(gameStateLocation.SubjectPlayer.Hand, front.Card)
				});
			}
		}
	}
}