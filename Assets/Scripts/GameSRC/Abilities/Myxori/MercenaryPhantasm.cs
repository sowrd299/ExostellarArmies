using SFB.Game.Management;
using System.Collections.Generic;

namespace SFB.Game
{
	public class MercenaryPhantasm : Ability
	{
		public override string GetMainText() {
			return "Deploy: Your opponent reveals a card from their hand at random.";
		}

		public MercenaryPhantasm() : base(-1) { }

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += ArchonBrynhildeClassInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= ArchonBrynhildeClassInner;
		}

		public void ArchonBrynhildeClassInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.IsFortifying("Carthan")) {
				// heal front line 2
				deltas.AddRange(
					UnitHealthDelta.GetHealDeltas(
						gmLoc.FrontUnit,
						gmLoc.SubjectUnit,
						2,
						gmLoc.GameManager
					)
				);
				// gain 3 mana
				deltas.AddRange(
					gmLoc.SubjectPlayer.ManaPool.GetAddDeltas(3)
				);
			}
		}
	}
}