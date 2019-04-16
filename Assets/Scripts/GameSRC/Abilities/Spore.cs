using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB.Game.Management;

namespace SFB.Game
{
    public class Spore : Ability {
		public Spore(int n) : base(n) { }
		public override Delta[] onDeath(int play, int pos, Player[] players) { return players[play].Mana.GetAddDeltas(Num); }
	}
}
