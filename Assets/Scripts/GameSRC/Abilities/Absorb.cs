﻿using SFB.Game.Management;
using SFB.Game.Content;
using System.Collections.Generic;

namespace SFB.Game
{
	public class Absorb : Ability
	{
		public Absorb()
			: base(-1)
		{ }

		public override void ApplyTo(Unit u, GameState initialGameState)
		{
			void AbsorbInner(ref int amt) {
				amt = 0;
			}
			u.ModifyDamageLeft += AbsorbInner;
		}

		public override void ApplyTo(GameManager gm)
		{ }
	}
}