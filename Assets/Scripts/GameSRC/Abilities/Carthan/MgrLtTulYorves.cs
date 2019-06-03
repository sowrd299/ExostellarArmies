using SFB.Game.Management;
using System.Collections.Generic;
using SFB.Game.Content;

namespace SFB.Game
{
	public class MgrLtTulYorves : Ability
	{
		// Front Line Persistent: Whenever you heal a Unit, heal it for 1 more and deal 1 damage to whatever is opposing it.

		public Unit Source { get; private set; }

		public MgrLtTulYorves() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm) {
			Source = u;
			gm.AddHealDeltas += MgrLtTulYorvesInner;
			u.AddDeathDeltas += RemoveFromGM;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm) {
			gm.AddHealDeltas -= MgrLtTulYorvesInner;
		}

		public void MgrLtTulYorvesInner(List<Delta> deltas, GameManager gm, UnitDelta ud) {
			System.Tuple<int, int, int> lsp = Lane.GetLaneSidePosOf(ud.Target, gm.Lanes);

			if(lsp != null) {
				Lane lane = gm.Lanes[lsp.Item1];
				int side = lsp.Item2;

				deltas.Add(new UnitHealthDelta(ud.Target, Source, 1, Damage.Type.HEAL));
				if(lane.IsOccupied(1-side, 0)) {
					deltas.Add(new UnitHealthDelta(lane.Units[1 - side, 0], Source, 1, Damage.Type.ABILITY));
				} else if(lane.IsOccupied(1 - side, 1)) {
					deltas.Add(new UnitHealthDelta(lane.Units[1 - side, 1], Source, 1, Damage.Type.ABILITY));
				} else {
					deltas.Add(new TowerDamageDelta(lane.Towers[1-side], 1, Damage.Type.ABILITY));
				}
			}
		}

		public void RemoveFromGM(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase) {
			gmLoc.GameManager.AddHealDeltas -= MgrLtTulYorvesInner;
		}
	}
}
