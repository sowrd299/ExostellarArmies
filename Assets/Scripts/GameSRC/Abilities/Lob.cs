using System.Collections;
using System.Collections.Generic;

namespace SFB.Game
{
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
