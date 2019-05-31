using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Swarm : Ability
	{
		// When you deploy this, if it is behind a Unit that shares a tag with it
		// other than "Unit", draw a card.

		public Swarm() : base(-1) {}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas += SwarmInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.AddInitialDeployDeltas -= SwarmInner;
		}

		void SwarmInner(List<Delta> deltas, GMWithLocation gmLoc)
		{
			if(gmLoc.Pos == 1) {
				foreach(string s in gmLoc.SubjectUnit.Card.UnitType.Split(' ')) {
					if(s != "Unit" && gmLoc.IsSupporting(s)) {
						RemoveFromDeckDelta[] rs = gmLoc.SubjectPlayer.Deck
																	  .GetDrawDeltas(count: 1);
						deltas.AddRange(rs);
						deltas.AddRange(
							gmLoc.SubjectPlayer.Hand
											   .GetDrawDeltas(
												   rs,
												   gmLoc.GameManager
											   )
						);
						return;
					}
				}
			}
		}
	}
}