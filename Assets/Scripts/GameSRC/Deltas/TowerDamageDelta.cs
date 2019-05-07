using System;
using System.Xml;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game {
	public class TowerDamageDelta : TargetedDelta<Tower>
	{
		public int Amount { get; private set; }
		public Damage.Type DmgType { get; private set; }

		public TowerDamageDelta(Tower t, int amt, Damage.Type type)
			: base(t)
		{
			Amount = amt;
			DmgType = type;
		}

		public TowerDamageDelta(XmlElement from, CardLoader loader)
			: base(from, Tower.IdIssuer, loader)
		{
			Amount = Int32.Parse(from.Attributes["amount"].Value);
			DmgType = Damage.StringToDamageType(from.Attributes["dmgType"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);
			
			XmlAttribute amtAttr = doc.CreateAttribute("amount");
			amtAttr.Value = "" + Amount;
			r.SetAttributeNode(amtAttr);

			XmlAttribute typeAttr = doc.CreateAttribute("dmgType");
			typeAttr.Value = Damage.DamageTypeToString(DmgType);
			r.SetAttributeNode(typeAttr);

			return r;
		}

		internal override void Apply() {
			Target.TakeDamage(Amount);
		}

		internal override void Revert() {
			Target.UndoTakeDamage(Amount);
		}
	}
}