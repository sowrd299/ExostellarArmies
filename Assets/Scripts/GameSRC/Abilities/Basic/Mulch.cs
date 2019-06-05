using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Mulch : Ability
	{
		// When this dies, if there is nothing in this lane's back line, look at the top card of your deck.
		// If there is space in this lane's back line and if you can afford to deploy it in the back line
		// at 1 less Cost, do so; otherwise, discard it.
		public override string GetMainText() {
			return "Mulch";
		}

		public Mulch() : base(-1) {}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddDeathBeforeRemoveDeltas += MulchInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddDeathBeforeRemoveDeltas -= MulchInner;
		}

		void MulchInner(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase)
		{
			RemoveFromDeckDelta top = new RemoveFromDeckDelta(gmLoc.SubjectPlayer.Deck, gmLoc.SubjectPlayer.Deck[0], 0);
			deltas.Add(top);

			UnitCard topUnitCard = top.Card as UnitCard;
			// BackUnit works out perfectly because it returns null if its empty or if it contains SubjectUnit
			if(topUnitCard != null && gmLoc.SubjectPlayer.ManaPool.CanAfford(topUnitCard.DeployCost-1) && gmLoc.BackUnit == null) {
				deltas.AddRange(gmLoc.SubjectPlayer.ManaPool.GetSubtractDeltas(topUnitCard.DeployCost - 1));
				deltas.AddRange(gmLoc.SubjectLane.GetDeployDeltas(topUnitCard, gmLoc.Side, 1, gmLoc.GameManager));
			} else {
				deltas.AddRange(gmLoc.SubjectPlayer.Discard.GetDiscardDeltas(new Card[] { top.Card }));
			}
		}
	}
}