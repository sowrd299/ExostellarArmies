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
			if(gmLoc.IsFortifying("Carthan", "Infantry")) {
				foreach(Card c in gmLoc.SubjectPlayer.Hand)
					deltas.AddRange(gmLoc.SubjectPlayer.Hand.GetRemoveDelta(c));

				deltas.Add(new UnitDamageAmountDelta(gmLoc.FrontUnit, 1,
							Damage.Type.RANGED, gmLoc.SubjectUnit));

				deltas.AddRange(
					UnitHealthDelta.GetHealDeltas(
						gmLoc.FrontUnit,
						gmLoc.SubjectUnit,
						gmLoc.SubjectPlayer.Hand.Count,
						gmLoc.GameManager
					)
				);
			}
		}
	}
}