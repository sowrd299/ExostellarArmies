using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB.Game.Content;
using SFB.Game.Management;

namespace SFB.Game
{
    public abstract class Rush : Ability {
		internal override Delta[] onEachDeployPhase(int play, Lane l, Unit u) {
			return (u != l.Units[play, rushTo()] ? l.getSwapPositionDeltas(play) : new Delta[0]);
		}
		public abstract int rushTo();
	}
}
