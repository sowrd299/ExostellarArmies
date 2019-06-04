using System.Collections;
using System.Collections.Generic;
using SFB.Game;
using SFB.Game.Content;

namespace SFB.Game.Management
{
	public class CombatManager
	{
		public static Delta[] GetUnitCombatDeltas(Lane[] lanes, Damage.Type phase, GameManager gm)
		{
			List<Delta> deltas = new List<Delta>();
			

			for(int l = 0; l < lanes.Length; l++) {
				Lane lane = lanes[l];
				for(int play = 0; play < 2; play++) {
					TowerDamageDelta maxTowerDelta = null;
					for(int pos = 0; pos < 2; pos++) {
						if(lane.IsOccupied(play, pos)) {
							//System.Console.WriteLine($"  Found {lane.Units[play, pos].Card.Name} at Lane{l} Side{play} Pos{pos}");
							Delta[] combatDeltas = lane.Units[play, pos].GetDamagingDeltas(lane, 1 - play, phase, gm);
							foreach(Delta d in combatDeltas) {
								if(d is TowerDamageDelta) {
									TowerDamageDelta t = d as TowerDamageDelta;
									if(maxTowerDelta == null || t.Amount > maxTowerDelta.Amount)
										maxTowerDelta = t;
								} else {
									deltas.Add(d);
								}
							}
						}
					}
					if(maxTowerDelta != null) {
						deltas.Add(maxTowerDelta);
						gm.UseAddTowerDamageDeltas(deltas, maxTowerDelta.Target);
					}
				}
			}
			
			return deltas.ToArray();
		}
		
		public static Delta[] GetTowerCombatDeltas(Lane[] lanes, GameManager gm)
		{
			List<Delta> list = new List<Delta>();

			foreach(Lane l in lanes) {
				for(int play = 1; play >= 0; play--) {
					int dmgLeft = l.Towers[System.Math.Abs(play - 1)].Damage;

					int pos = 0;

					while(dmgLeft > 0 && pos < 2) {
						if(l.IsOccupied(play, pos)) {
							Unit target = l.Units[play, pos];
							int deal = System.Math.Min(target.HealthPoints, dmgLeft);
							list.AddRange(
								UnitHealthDelta.GetDamageDeltas(
									target, 
									null,
									deal,
									Damage.Type.TOWER,
									gm));
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
