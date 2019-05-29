using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	// Deploy: Heal this Front Line 2 and each adjacent Front Line 1.

	public class EmergencyMedDrop : Ability
	{
		public EmergencyMedDrop() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += EmergencyMedDropInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= EmergencyMedDropInner;
		}

		public void EmergencyMedDropInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			Unit front = gmLoc.FrontUnit;
			if(front != null) // returns null if already front
				deltas.Add(new UnitHealthDelta(front, 2, Damage.Type.HEAL, gmLoc.SubjectUnit));

			int side = gmLoc.Side;

			Unit left = gmLoc.LeftLane?.Units?[side, 0];
			if(left != null)
				deltas.Add(new UnitHealthDelta(left, 1, Damage.Type.HEAL, gmLoc.SubjectUnit));

			Unit right = gmLoc.RightLane?.Units?[side, 0];
			if(right != null)
				deltas.Add(new UnitHealthDelta(right, 1, Damage.Type.HEAL, gmLoc.SubjectUnit));
		}
	}
}