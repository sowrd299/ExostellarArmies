using System.Collections;
using System.Collections.Generic;
using SFB.Game;
using SFB.Game.Content;

namespace SFB.Game.Management {
	public class CombatManager {
		public static Delta[] getRangedDeltas(Lane[] lanes) {
			List<Delta> list = new List<Delta>();

			foreach(Lane l in lanes)
				for(int play = 0; play < 2; play++)
					for(int pos = 0; pos < 2; pos++)
						if(l.isOccupied(play, pos))
							list.AddRange(l.Units[play, pos].getRangedDamagingDelta(l, System.Math.Abs(play-1)));

			return list.ToArray();
		}

		public static Delta[] getMeleeDeltas(Lane[] lanes) {
			List<Delta> list = new List<Delta>();

			foreach(Lane l in lanes)
				for(int play = 0; play < 2; play++)
					for(int pos = 0; pos < 2; pos++)
						if(l.isOccupied(play, pos))
							list.AddRange(l.Units[play, pos].getMeleeDamagingDelta(l, System.Math.Abs(play-1)));

			return list.ToArray();
		}
	}
}
