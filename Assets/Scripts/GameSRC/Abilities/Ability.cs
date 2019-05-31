using SFB.Game.Management;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Reflection;
using SFB.Game.Content;

namespace SFB.Game
{
	// Abilities Not Implemented For:
	/*   Exostellar Tactical Squad
	 *   Mgr. Lt. Tul Yorves
	 *   Commercial Shipper / Commercial Comms Relay
	 *   
	 *   
	 */

	// Abilities that create new deltas (not called from GM):
	/*   Regrowth: AddToHandDelta
	 * 
	 * 
	 */
	public abstract class Ability
	{
		public delegate void AddDeltaGMUnitDelta(
			List<Delta> deltas, GameManager gm, UnitDelta ud);
		public delegate void AddDeltaGMBoardUpdate(
			List<Delta> deltas, GMWithBoardUpdate gmBoardUpdate);
		public delegate void AddDeltaGMLoc(
			List<Delta> deltas, GMWithLocation gmLoc);
		public delegate void AddDeltaGMLocPhase(
			List<Delta> deltas, GMWithLocation gmLoc, Damage.Type? phase);
		public delegate void AddDeltaGMLocUnit(
			List<Delta> deltas, GMWithLocation gmLoc, Unit u);
		public delegate void AddDeltaGMLocTower(
			List<Delta> deltas, GMWithLocation gmLoc, Tower tower);

		public delegate void FilterTargets(Unit[] targets);
		public delegate void ModifyInt(ref int amt);
		
		public int Amount { get; private set; }

		public Ability(int amount)
		{
			Amount = amount;
		}
		
		public void ApplyTo(Unit u, GameManager gm) {
			u.Abilities.Add(this);
			AddEffectsToEvents(u, gm);
		}

		public void RemoveFrom(Unit u, GameManager gm) {
			u.Abilities.Remove(this);
			RemoveEffectsFromEvents(u, gm);
		}

		protected abstract void AddEffectsToEvents(Unit u, GameManager gm);
		protected abstract void RemoveEffectsFromEvents(Unit u, GameManager gm);

		public static Ability FromXml(XmlElement from)
		{
			return Ability.FromString(
				from.Attributes["abilityType"].Value,
				Int32.Parse(from.Attributes["abilityAmount"].Value)
			);
		}

		public static Ability FromString(string str, int amt)
		{
			Type type = Type.GetType(str);
			if(type.IsSubclassOf(typeof(Ability))) {
				if(amt == -1) {
					ConstructorInfo con = type.GetConstructor(new Type[] { });
					return con?.Invoke(new object[] { }) as Ability;
				} else {
					ConstructorInfo con = type.GetConstructor(new Type[] { typeof(int) });
					return con?.Invoke(new object[] { amt }) as Ability;
				}
			} else {
				return null;
			}
		}

		public override string ToString()
		{
			return ""+this.GetType();
		}
	}
}