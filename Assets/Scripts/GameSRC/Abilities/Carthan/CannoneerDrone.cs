using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class CannoneerDrone : Ability
	{
		public override string GetMainText() {
			return "Front Line Persistent: The tower behind this deals +3 damage.";
		}
		
		private Tower AppliedTo;

		public CannoneerDrone() : base(-1)
		{
			AppliedTo = null;
		}

		protected override void AddEffectsToEvents(Unit source, GameManager gm)
		{
			source.AddInitialDeployDeltas += ApplyEffects;
			source.AddDeathDeltas += RemoveEffects;
		}

		protected override void RemoveEffectsFromEvents(Unit source, GameManager gm)
		{
			throw new System.Exception("");
		}

		private void ApplyEffects(List<Delta> deltas, GMWithLocation gmLoc)
		{
			Tower tower = gmLoc.SubjectLane.Towers[gmLoc.Side];
			deltas.Add(
				new TowerDamageAmountDelta(tower, 3)
			);
			AppliedTo = tower;
		}

		private void RemoveEffects(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase)
		{
			if(AppliedTo != null) {
				deltas.Add(new TowerDamageAmountDelta(AppliedTo, -3));
				AppliedTo = null;
			}
		}
	}
}
