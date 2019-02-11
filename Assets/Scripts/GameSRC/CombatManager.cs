using System.Collections;
using System.Collections.Generic;
using SFB.Game;
using SFB.Game.Content;

namespace SFB.Game.Management {
	// rn the players param of these methods isn't being used
	// there are 2 possible ways i think we could decrease player hp idk which is better
	//     let whatever calls these methods determine if it's a towerdelta and check if the turret gets destroyed
	//     use the param to pass into unit and the tower delta
	public class CombatManager {
		public static List<Delta> getRangedDeltas(Lane[] lanes, Player[] players) {
			List<Delta> list = new List<Delta>();

			foreach(Lane l in lanes)
				for(int play = 0; play < 2; play++)
					for(int pos = 0; pos < 2; pos++)
						if(l.isOccupied(play, pos))
							list.Add(l.Unit(play, pos).getRangedDamagingDelta(l, System.Math.Abs(play-1)));

			return list;
		}

		public static List<Delta> getMeleeDeltas(Lane[] lanes, Player[] players) {
			List<Delta> list = new List<Delta>();

			foreach(Lane l in lanes)
				for(int play = 0; play < 2; play++)
					for(int pos = 0; pos < 2; pos++)
						if(l.isOccupied(play, pos))
							list.Add(l.Unit(play, pos).getMeleeDamagingDelta(l, System.Math.Abs(play-1)));

			return list;
		}
	}
}
