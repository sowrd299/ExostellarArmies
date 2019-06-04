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
		public delegate void AddDeltaGM(
			List<Delta> deltas, GameManager gm);
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
		public delegate void AddDeltaGMTower(
			List<Delta> deltas, GameManager gm, Tower tower);

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
		public abstract string GetMainText();

		public static Ability FromXmlUnitAbilityDelta(XmlElement from)
		{
			return Ability.FromStringInt(
				from.Attributes["abilityType"].Value,
				Int32.Parse(from.Attributes["abilityAmount"].Value)
			);
		}

		public static Ability FromString(string str)
		{
			string[] split = str.Split(' ');
			//Console.WriteLine($"[{split[0]}{(split.Length > 1 ? ", " + split[1] : "")}]");
			return split.Length == 1
					? Ability.FromStringInt(split[0], -1)
					: Ability.FromStringInt(split[0], int.Parse(split[1]));
		}

		private static Ability FromStringInt(string str, int amt)
		{
			Type type = Type.GetType(str);
			if(type == null)
				type = Type.GetType("SFB.Game." + str);
			// sorry couldn't think of a better way
			// i hope all abilities are in SFB.Game
			
			if(type.IsSubclassOf(typeof(Ability))) {
				if(amt == -1) {
					ConstructorInfo con = type.GetConstructor(new Type[] { });
					//Console.WriteLine($"No number, constructor found: {con != null}");
					return con?.Invoke(new object[] { }) as Ability;
				} else {
					ConstructorInfo con = type.GetConstructor(new Type[] { typeof(int) });
					//Console.WriteLine($"Number, constructor found: {con != null}");
					return con?.Invoke(new object[] { amt }) as Ability;
				}
			} else {
				//Console.WriteLine("Not subclass");
				return null;
			}
		}

		public override bool Equals(Object other) {
			return other.GetType() == this.GetType() && this.ToString().Equals(other.ToString());
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public static bool operator==(Ability lhs, Ability rhs) {
			return lhs.Equals(rhs);
		}

		public static bool operator!=(Ability lhs, Ability rhs) {
			return !(lhs.Equals(rhs));
		}

		public override string ToString()
		{
			return ""+this.GetType() + (Amount == -1 ? "" : ""+Amount);
		}
	}
}