using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class CommercialComsRelay : Ability
	{
		public override string GetMainText() {
			return "Deploy: Put a card from your hand beneath the top 4 cards of your deck.";
		}

		public CommercialComsRelay() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += InnerCommercialComsRelay;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= InnerCommercialComsRelay;
		}

		public void InnerCommercialComsRelay(List<Delta> delta, GMWithLocation gmLoc)
		{
			System.Console.WriteLine("COMM COMS RELAY INIT DEP");
			CommercialComsRelayIR inputRequest = new CommercialComsRelayIR(
				gmLoc.SubjectPlayer, gmLoc.GameManager
			);
		}
	}
}