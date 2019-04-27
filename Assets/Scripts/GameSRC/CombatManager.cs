using System.Collections;
using System.Collections.Generic;
using SFB.Game;
using SFB.Game.Content;

namespace SFB.Game.Management {
	public class CombatManager {
		public static Delta[] getRangedDeltas(Lane[] lanes) {
			List<Delta> deltas = new List<Delta>();
			TowerDelta maxTowerDelta = null;

			foreach(Lane l in lanes)
				for(int play = 0; play < 2; play++)
					for(int pos = 0; pos < 2; pos++)
						if(l.isOccupied(play, pos)) {
							//Debug.Log("R" + play + " " + pos);
							Delta[] combatDeltas = l.Units[play, pos].getRangedDamagingDeltas(l, System.Math.Abs(play - 1));
							foreach(Delta d in combatDeltas) {
								if(d.GetType() == typeof(TowerDelta)) {
									TowerDelta t = d as TowerDelta;
									if(maxTowerDelta == null || t.Amount > maxTowerDelta.Amount)
										maxTowerDelta = t;
								} else {
									deltas.Add(d);
								}
							}
						}

			if(maxTowerDelta != null)
				deltas.Add(maxTowerDelta);
			
			return deltas.ToArray();
		}

		public static Delta[] getMeleeDeltas(Lane[] lanes) {
			List<Delta> deltas = new List<Delta>();
			TowerDelta maxTowerDelta = null;

			foreach(Lane l in lanes)
				for(int play = 0; play < 2; play++)
					for(int pos = 0; pos < 2; pos++)
						if(l.isOccupied(play, pos)) {
							//Debug.Log("R" + play + " " + pos);
							Delta[] combatDeltas = l.Units[play, pos].getMeleeDamagingDeltas(l, System.Math.Abs(play - 1));
							foreach(Delta d in combatDeltas) {
								if(d.GetType() == typeof(TowerDelta)) {
									TowerDelta t = d as TowerDelta;
									if(maxTowerDelta == null || t.Amount > maxTowerDelta.Amount)
										maxTowerDelta = t;
								} else {
									deltas.Add(d);
								}
							}
						}

			if(maxTowerDelta != null)
				deltas.Add(maxTowerDelta);

			return deltas.ToArray();
		}
		
		public static Delta[] getTowerDeltas(Lane[] lanes) {
			List<Delta> list = new List<Delta>();

			foreach(Lane l in lanes) {
				for(int play = 1; play >= 0; play--) {
					int dmgLeft = l.Towers[System.Math.Abs(play - 1)].Damage;

					int pos = 0;

					while(dmgLeft > 0 && pos < 2) {
						if(l.isOccupied(play, pos)) {
							Unit target = l.Units[play, pos];
							int deal = System.Math.Min(target.HealthPoints, dmgLeft);
							list.Add(new UnitDelta(target, deal, Damage.Type.TOWER, null));
							dmgLeft -= deal;
						}
						pos++;
					}
				}
			}
			return list.ToArray();
		}
	}
}
