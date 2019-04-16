using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFB.Game
{
    // say there were a unit that would always stay in the backline, lob would still lob over it
    public class Lob : Ability
    {
        public Lob() : base() { }

        internal override Unit[,] filterTargets(Unit[,] arr, int oppPlay)
        {
            Unit[,] nArr = new Unit[arr.GetLength(0), arr.GetLength(1)];
            bool hasLobbed = false;
            for (int i = 0; i < arr.Length; i++)
                if (arr[oppPlay, i] != null)
                {
                    if (!hasLobbed)
                        hasLobbed = true;
                    else
                        nArr[oppPlay, i] = arr[oppPlay, i];
                }
            return nArr;
        }
    }
}
