using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;
using System.Collections.Generic;

namespace SFB.Game
{
	public class UnitHealthDelta : UnitDelta
	{
		public int Amount { get; private set; }

		public Damage.Type DmgType { get; private set; }

		public static Delta[] GetHealDeltas(Unit target, Unit source, int amt, GameManager gm)
		{
			List<Delta> deltas = new List<Delta>();
			UnitDelta ud = new UnitHealthDelta(target, source, amt, Damage.Type.HEAL);
			gm.UseAddHealDeltas(deltas, ud);
			return deltas.ToArray();
		}

		public static Delta[] GetDamageDeltas(Unit target, Unit source, int amt, Damage.Type type, GameManager gm)
		{
			// room for interacting with gm (like done above)
			// for if there was a card that modified damage
			return new Delta[] {
				new UnitHealthDelta(target, source, amt, type)
			};
		}

		public UnitHealthDelta(Unit t, Unit s, int a, Damage.Type type)
			: base(t, s)
		{
			Amount = a;
			DmgType = type;
		}

		public UnitHealthDelta(XmlElement from, CardLoader cl)
			: base(from, cl)
		{
			Amount = Int32.Parse(from.Attributes["amount"].Value);
			DmgType = Damage.StringToDamageType(from.Attributes["dmgType"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);

			XmlAttribute amtAttr = doc.CreateAttribute("amount");
			amtAttr.Value = ""+Amount;
			r.SetAttributeNode(amtAttr);

			XmlAttribute typeAttr = doc.CreateAttribute("dmgType");
			typeAttr.Value = Damage.DamageTypeToString(DmgType);
			r.SetAttributeNode(typeAttr);

			return r;
		}

		protected override void ApplyEffects(Unit u) {
			switch(DmgType) {
				case Damage.Type.HEAL:
					Target.Heal(Amount);
					break;
				default:
					Target.TakeDamage(Amount, DmgType);
					break;
			}
		}
	}
}
