using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class CommercialShipper : Ability
	{
		// Deploy: Draw a card, then put a card from hand on the bottom of your deck.

		public CommercialShipper() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += InnerCommercialShipper;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= InnerCommercialShipper;
		}

		public void InnerCommercialShipper(List<Delta> delta, GMWithLocation gmLoc)
		{
			
			CommercialShipperIR inputRequest = new CommercialShipperIR(
				gmLoc.SubjectPlayer, gmLoc.GameManager
			);
		}
	}
}