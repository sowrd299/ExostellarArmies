using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Lob : Ability
	{
		// In combat, this deals damage to whatever is behind the opposing front line.

		public Lob() : base(-1) {}

		protected override void AddEffectsToEvents(Unit u, GameManager gm)
		{
			u.FilterTargets += LobInner;
		}

		protected override void RemoveEffectsFromEvents(Unit u, GameManager gm)
		{
			u.FilterTargets -= LobInner;
		}

		private void LobInner(Unit[] units) {
			for(int i = 0; i < units.Length; i++) {
				if(units[i] != null) {
					units[i] = null;
					return;
				}
			}
		}
	}
}