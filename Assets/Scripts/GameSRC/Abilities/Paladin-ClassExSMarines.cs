using SFB.Game.Management;
using System.Collections.Generic;
using System;

namespace SFB.Game
{
	// Front Line Recurring Deploy: If there is not a unit behind this, heal each adjacent allied front line unit 1.

	public class PaladinClassExSMarines : Ability
	{
		public PaladinClassExSMarines() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas += PaladinClassExSMarinesInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas -= PaladinClassExSMarinesInner;
		}

		public void PaladinClassExSMarinesInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.Pos == 0 && gmLoc.BackUnit == null) {
				int lane = gmLoc.Lane;
				int side = gmLoc.Side;

				Unit leftLaneFront = gmLoc.LeftLane?.Units?[side, 0];
				if(leftLaneFront != null)
					deltas.Add(new UnitHealthDelta(leftLaneFront, 1, Damage.Type.HEAL, gmLoc.SubjectUnit));

				Unit rightLaneFront = gmLoc.RightLane?.Units?[side, 0];
				if(rightLaneFront != null)
					deltas.Add(new UnitHealthDelta(rightLaneFront, 1, Damage.Type.HEAL, gmLoc.SubjectUnit));
			}
		}
	}
}