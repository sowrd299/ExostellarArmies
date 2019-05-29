using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class AdvSupplyDrone : Ability
	{
		// Support Carthan: Heal this Front Line 2 and gain 3 Resources.

		public AdvSupplyDrone() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += AdvSupplyDroneInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= AdvSupplyDroneInner;
		}

		public void AdvSupplyDroneInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.IsSupporting("Carthan")) {
				deltas.Add(new UnitHealthDelta(gmLoc.FrontUnit, 2, Damage.Type.HEAL, gmLoc.SubjectUnit));
				deltas.AddRange(
					gmLoc.SubjectPlayer.ManaPool.GetAddDeltas(3)
				);
			}
		}
	}
}