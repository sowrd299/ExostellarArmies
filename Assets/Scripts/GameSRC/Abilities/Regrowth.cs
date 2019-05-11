using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;
using System;

namespace SFB.Game
{
	public class Regrowth : Ability
	{
		public Func<GameStateLocation, bool> Function { get; private set; }

		public Regrowth(Func<GameStateLocation, bool> fxn)
			: base(-1)
		{
			Function = fxn;
		}

		public override void ApplyTo(Unit u, GameState initialGameState)
		{
			void RegrowthInner(List<Delta> deltas, GameStateLocation gameStateLoc) {
				if(Function(gameStateLoc))
					deltas.Add(new AddToHandDelta(
						gameStateLoc.SubjectPlayer.Hand,
						gameStateLoc.SubjectUnit.Card));
			}
			u.AddDeathDeltas += RegrowthInner;
		}
	}
}