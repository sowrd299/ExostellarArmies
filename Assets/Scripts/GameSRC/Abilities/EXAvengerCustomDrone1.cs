using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Support Carthan Infantry: Discard your hand to give the supported unit +1R this turn and heal it 1 for each card discarded.

	public class EXAvengerCustomDrone1 : Ability
	{
		public EXAvengerCustomDrone1() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += EXAvengerCustomDrone1Inner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= EXAvengerCustomDrone1Inner;
		}

		public void EXAvengerCustomDrone1Inner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.IsSupporting(new string[] { "Carthan", "Infantry" })) {
				foreach(Card c in gmLoc.SubjectPlayer.Hand)
					deltas.AddRange(gmLoc.SubjectPlayer.Hand.GetRemoveDelta(c));
				deltas.Add(new UnitDamageAmountDelta(gmLoc.FrontUnit, 1,
							Damage.Type.RANGED, gmLoc.SubjectUnit));
				deltas.Add(new UnitHealthDelta(gmLoc.FrontUnit, gmLoc.SubjectPlayer.Hand.Count,
							Damage.Type.RANGED, gmLoc.SubjectUnit));
			}
		}
	}
}