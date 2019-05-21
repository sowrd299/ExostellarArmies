using SFB.Game.Management;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Reflection;

namespace SFB.Game
{
	// Abilities Not Implemented For:
	/*   Exostellar Tactical Squad
	 *   Mgr. Lt. Tul Yorves
	 *   Commercial Shipper / Commercial Comms Relay
	 *   Mercenary Phantasm
	 * 
	 */
	public abstract class Ability
	{
		public delegate void AddDelta(List<Delta> deltas, GameStateLocation gameStateLocation);
		public delegate void FilterTargets(Unit[] targets);
		public delegate void ModifyInt(ref int amt);
		
		public int Amount { get; private set; }

		public Ability(int amount)
		{
			Amount = amount;
		}

		// gamestate may be null
		public void ApplyTo(Unit u, GameState gameState) {
			u.Abilities.Add(this);
			ApplyEffects(u, gameState);
		}
		public void RemoveFrom(Unit u, GameState gameState) {
			u.Abilities.Remove(this);
			RemoveEffects(u, gameState);
		}
		protected abstract void ApplyEffects(Unit u, GameState gameState);
		protected abstract void RemoveEffects(Unit u, GameState gameState);

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