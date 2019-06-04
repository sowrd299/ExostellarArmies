using SFB.Game.Management;
using System.Collections.Generic;
using System;

namespace SFB.Game
{
	public class SphereMedicalVanguard : Ability
	{
		public override string GetMainText() {
			return "Front Line Recurring Deploy: If there is not a unit behind this, heal each adjacent allied front line unit 1.";
		}

		public SphereMedicalVanguard() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas += SphereMedicalVanguardInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas -= SphereMedicalVanguardInner;
		}

		public void SphereMedicalVanguardInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			Console.WriteLine($"Sphere Medical Vanguard L{gmLoc.Lane} S{gmLoc.Side} P{gmLoc.Pos}");
			if(gmLoc.Pos == 0 && gmLoc.BackUnit == null) {
				Console.WriteLine("  In");
				int lane = gmLoc.Lane;
				int side = gmLoc.Side;

				Unit leftLaneFront = gmLoc.LeftLane?.Units?[side, 0];
				if(leftLaneFront != null) {
					Console.WriteLine("  L");
					deltas.AddRange(
						UnitHealthDelta.GetHealDeltas(
							leftLaneFront,
							gmLoc.SubjectUnit,
							1,
							gmLoc.GameManager
						)
					);
				}

				Unit rightLaneFront = gmLoc.RightLane?.Units?[side, 0];
				if(rightLaneFront != null) {
					Console.WriteLine("  R");
					deltas.AddRange(
						UnitHealthDelta.GetHealDeltas(
							rightLaneFront,
							gmLoc.SubjectUnit,
							1,
							gmLoc.GameManager
						)
					);
				}
			}
		}
	}
}