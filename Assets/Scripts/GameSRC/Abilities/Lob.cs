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

		public override void ApplyTo(Unit u, GameState initialGameState)
		{
			void LobInner(Unit[] units) {
				for(int i = 0; i < units.Length; i++) {
					if(units[i] != null) {
						units[i] = null;
						return;
					}
				}
			}
			u.FilterTargets += LobInner;
		}
	}
}