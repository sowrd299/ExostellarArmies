using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class CommercialShipper : Ability
	{
		public override string GetMainText() {
			return "Deploy: Put a card from hand on the bottom of your deck, then draw a card.";
		}

		public CommercialShipper() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += InnerCommercialShipper;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= InnerCommercialShipper;
		}

		public void InnerCommercialShipper(List<Delta> deltas, GMWithLocation gmLoc)
		{
			System.Console.WriteLine("COMM SHIP INIT DEP");
			deltas.AddRange(gmLoc.SubjectPlayer.GetDrawDeltas(gmLoc.GameManager));
			CommercialShipperIR inputRequest = new CommercialShipperIR(
				gmLoc.SubjectPlayer, gmLoc.GameManager
			);
			gmLoc.SubjectPlayer.AssignInputRequest(inputRequest);
		}
	}
}