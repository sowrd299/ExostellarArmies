using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game {
	public class UnitDamageAmountDelta : UnitDelta {
		public int Amount { get; private set; }

		public Damage.Type DmgType { get; private set; }

		public UnitDamageAmountDelta(Unit t, int a, Damage.Type type, Unit s)
			: base(t, s)
		{
			Amount = a;
			DmgType = type;
		}

		public UnitDamageAmountDelta(XmlElement from, CardLoader cl)
			: base(from, cl)
		{
			Amount = Int32.Parse(from.Attributes["amount"].Value);
			DmgType = Damage.StringToDamageType(from.Attributes["dmgType"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc)
		{
			XmlElement r = base.ToXml(doc);

			XmlAttribute amtAttr = doc.CreateAttribute("amount");
			amtAttr.Value = "" + Amount;
			r.SetAttributeNode(amtAttr);

			XmlAttribute typeAttr = doc.CreateAttribute("dmgType");
			typeAttr.Value = Damage.DamageTypeToString(DmgType);
			r.SetAttributeNode(typeAttr);

			return r;
		}

		protected override void ApplyEffects(Unit u)
		{
			switch(DmgType) {
				case Damage.Type.MELEE:
					Target.MeleeAtkMod += (ref int a) => { a += Amount; };
					break;
				case Damage.Type.RANGED:
					Target.RangedAtkMod += (ref int a) => { a += Amount; };
					break;
				default:
					throw new IllegalDeltaException($"Invalid UnitDamageAmountDelta for dmgtype \"{DmgType}\"");
			}
		}
	}
}
