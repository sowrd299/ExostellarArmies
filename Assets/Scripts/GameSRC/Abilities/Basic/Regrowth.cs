using SFB.Game.Management;
using System.Collections.Generic;
using System;

namespace SFB.Game
{
	public class Regrowth : Ability
	{
		// When this dies, if <condition applies>, return it to your hand

		public Func<GMWithLocation, Damage.Type?, bool> Function { get; private set; }

		public Regrowth(Func<GMWithLocation, Damage.Type?, bool> fxn)
			: base(-1)
		{
			Function = fxn;
		}

		protected override void AddEffectsToEvents(Unit u, GameManager gm) {
			u.AddDeathDeltas += RegrowthInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm) {
			u.AddDeathDeltas -= RegrowthInner;
		}

		private void RegrowthInner(List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase)
		{
			if(Function(gmLoc, phase))
			{
				deltas.AddRange(
					gmLoc.SubjectPlayer.Hand
						 .GetDrawDeltas(
							 gmLoc.SubjectUnit.Card,
							 gmLoc.GameManager
						 )
				);
			}
		}
	}
}