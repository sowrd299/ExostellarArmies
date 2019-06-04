using SFB.Game.Management;
using System.Collections.Generic;
using SFB.Game.Content;

namespace SFB.Game
{
	public class MgrLtTulYorves : Ability
	{
		public override string GetMainText() {
			return "Front Line Persistent: Whenever you heal a Unit, heal it for 1 more and deal 1 damage to whatever is opposing it.";
		}

		public Unit Source { get; private set; }

		public MgrLtTulYorves() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm) {
			Source = u;
			u.AddInitialDeployDeltas += AddToGM;
			u.AddDeathDeltas += RemoveFromGM;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm) {
			gm.AddHealDeltas -= MgrLtTulYorvesInner;
		}

		public void MgrLtTulYorvesInner(List<Delta> deltas, GameManager gm, UnitDelta ud) {
			System.Tuple<int, int, int> targetLSP = Lane.GetLaneSidePosOf(ud.Target, gm.Lanes);
			System.Tuple<int, int, int> sourceLSP = Lane.GetLaneSidePosOf(Source, gm.Lanes);

			if(targetLSP != null && sourceLSP != null) {
				if(targetLSP.Item2 == sourceLSP.Item2) {
					int side = targetLSP.Item2;
					Lane targetLane = gm.Lanes[targetLSP.Item1];

					deltas.Add(new UnitHealthDelta(ud.Target, Source, 1, Damage.Type.HEAL));

					if(targetLane.IsOccupied(1 - side, 0)) {
						deltas.Add(new UnitHealthDelta(targetLane.Units[1 - side, 0], Source, 1, Damage.Type.ABILITY));
					} else if(targetLane.IsOccupied(1 - side, 1)) {
						deltas.Add(new UnitHealthDelta(targetLane.Units[1 - side, 1], Source, 1, Damage.Type.ABILITY));
					} else {
						deltas.Add(new TowerDamageDelta(targetLane.Towers[1 - side], 1, Damage.Type.ABILITY));
					}
				}
			}
		}

		public void AddToGM(List<Delta> deltas, GMWithLocation gmLoc) {
			gmLoc.GameManager.AddHealDeltas += MgrLtTulYorvesInner;
		}

		public void RemoveFromGM(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase) {
			gmLoc.GameManager.AddHealDeltas -= MgrLtTulYorvesInner;
		}
	}
}
