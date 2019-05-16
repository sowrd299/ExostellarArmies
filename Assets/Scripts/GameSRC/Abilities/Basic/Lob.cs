using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Lob : Ability
	{
		public Lob()
			: base(-1)
		{}

		protected override void ApplyEffects(Unit u, GameState initialGameState)
		{
			u.FilterTargets += LobInner;
		}

		protected override void RemoveEffects(Unit u, GameState initialGameState)
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