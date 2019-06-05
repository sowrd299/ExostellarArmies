using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class ArchonEximusClass1 : Ability
	{
		public override string GetMainText() {
			return "Fortify Carthan Infantry: Discard your hand, and for each card discarded, give the fortified unit +1R this turn and heal it by 1";
		}

		private int turn;
		private Unit buffed;
		private int buffAmount;
		public ArchonEximusClass1() : base(-1) {
			turn = 0;
			buffed = null;
			buffAmount = 0;
		}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas += EXAvengerCustomDrone1Inner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas -= EXAvengerCustomDrone1Inner;
		}

		public void EXAvengerCustomDrone1Inner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(turn == 0) {
				if(gmLoc.IsFortifying("Carthan", "Infantry")) {
					foreach(Card c in gmLoc.SubjectPlayer.Hand)
						deltas.AddRange(gmLoc.SubjectPlayer.Hand.GetRemoveDelta(c));

					buffAmount = gmLoc.SubjectPlayer.Hand.Count;

					deltas.Add(new UnitDamageAmountDelta(gmLoc.FrontUnit, buffAmount,
								Damage.Type.RANGED, gmLoc.SubjectUnit));
					buffed = gmLoc.FrontUnit;

					deltas.AddRange(
						UnitHealthDelta.GetHealDeltas(
							gmLoc.FrontUnit,
							gmLoc.SubjectUnit,
							buffAmount,
							gmLoc.GameManager
						)
					);
				}
			} else if(turn == 1) {
				if(buffed != null) {
					deltas.Add(new UnitDamageAmountDelta(buffed, -buffAmount,
							Damage.Type.RANGED, gmLoc.SubjectUnit));
					buffed = null;
				}
			}
			turn++;
		}
	}
}