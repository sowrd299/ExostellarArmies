using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB.Game.Content;
using SFB.Game.Management;

namespace SFB.Game
{
    public class Regrowth : Ability
    {
        private int play;
        private Func<Player[], Lane[], bool> func;

        public Regrowth(int p, Func<Player[], Lane[], bool> f)
        {
            play = p;
            func = f;
        }

        public override Delta[] onDeath(int play, Player[] players, Lane[] lanes, Card c)
        {
            List<Delta> l = new List<Delta>();

            if (func(players, lanes))
                l.Add(new Hand.AddToHandDelta(players[play].Hand, c));

            return l.ToArray();
        }
    }
}
