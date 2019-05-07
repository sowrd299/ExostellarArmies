using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Spore : Ability
	{
		public Spore(int amount)
			: base(amount)
		{}

		public override void ApplyTo(Unit u)
		{
			void SporeInner(List<Delta> deltas, int side, int pos, int lane, Lane[] lanes, Player[] players) {
				deltas.AddRange(players[side].ManaPool.GetAddDeltas(Amount));
			}
			u.AddDeathDeltas += SporeInner;
		}
	}
}