using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class AxonOfAdaptability : Ability
	{
		public override string GetMainText() {
			return "Fortify Artillery: Gain Spore 3 until end of turn.\nFortify Infantry: Gain Lob until end of turn.";
		}

		private Unit appliedSporeTo;
		private Unit appliedLobTo;
		private int turn;

		public AxonOfAdaptability() : base(-1) {
			appliedSporeTo = null;
			appliedLobTo = null;
			turn = 0;
		}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddRecurringDeployDeltas += SphereDeathseekersInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			throw new System.Exception("Can't remove this ability - potential bugs");
		}

		public void SphereDeathseekersInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(turn == 0) {
				Unit self = gmLoc.SubjectUnit;

				// Fortify Artillery: Gain Spore 3 until end of turn.
				if(gmLoc.IsFortifying("Artillery")) {
					deltas.Add(
						new UnitAbilityDelta(self, self, new Spore(3), UnitAbilityDelta.DeltaMode.ADD, gmLoc.GameManager)
					);
					appliedSporeTo = self;
				}

				// Fortify Infantry: Gain Lob until end of turn.
				if(gmLoc.IsFortifying("Infantry")) {
					deltas.Add(
						new UnitAbilityDelta(self, self, new Lob(), UnitAbilityDelta.DeltaMode.ADD, gmLoc.GameManager)
					);
					appliedLobTo = self;
				}
				
			} else if(turn == 1) {
				// Remove Spore
				if(appliedSporeTo != null) {
					deltas.Add(
						new UnitAbilityDelta(appliedSporeTo, appliedSporeTo,
							new Spore(3), UnitAbilityDelta.DeltaMode.REMOVE, gmLoc.GameManager)
					);
					appliedSporeTo = null;
				}

				// Remove Lob
				if(appliedLobTo != null) {
					deltas.Add(
						new UnitAbilityDelta(appliedLobTo, appliedLobTo,
							new Lob(), UnitAbilityDelta.DeltaMode.REMOVE, gmLoc.GameManager)
					);
					appliedLobTo = null;
				}
			}
			turn++;
		}
	}
}