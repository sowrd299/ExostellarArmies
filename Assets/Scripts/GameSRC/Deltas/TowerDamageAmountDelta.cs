using System;
using System.Xml;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game
{
	public class TowerDamageAmountDelta : TargetedDelta<Tower>
	{
		public int Amount { get; private set; }

		public TowerDamageAmountDelta(Tower t, int amt)
			: base(t)
		{
			Amount = amt;
		}

		public TowerDamageAmountDelta(XmlElement from, CardLoader loader)
			: base(from, Tower.IdIssuer, loader)
		{
			Amount = Int32.Parse(from.Attributes["amount"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);
			
			XmlAttribute amtAttr = doc.CreateAttribute("amount");
			amtAttr.Value = "" + Amount;
			r.SetAttributeNode(amtAttr);

			return r;
		}

		internal override void Apply() {
			Target.Damage += Amount;
		}

		internal override void Revert() {
			Target.Damage += Amount;
		}
	}
}