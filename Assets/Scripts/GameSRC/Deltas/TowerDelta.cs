using System;
using System.Xml;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game {
	public class TowerDelta : Delta {
		private SendableTarget<Tower> sendableTower;

		private int amount;
		public int Amount {
			get { return amount; }
		}

		private Damage.Type dmgType;
		public Damage.Type DmgType {
			get { return dmgType; }
		}

		public TowerDelta(Tower t, int amt, Damage.Type type) {
			sendableTower = new SendableTarget<Tower>("tower", t);
			amount = amt;
			dmgType = type;
		}

		public TowerDelta(XmlElement from, CardLoader loader)
			: base(from, loader)
		{
			sendableTower = new SendableTarget<Tower>("tower", from, Tower.idIssuer);
			amount = Int32.Parse(from.Attributes["amount"].Value);
			dmgType = Damage.StringToDamageType(from.Attributes["dmgType"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);

			r.SetAttributeNode(sendableTower.ToXml(doc));
			
			XmlAttribute amtAttr = doc.CreateAttribute("amount");
			amtAttr.Value = "" + amount;
			r.SetAttributeNode(amtAttr);

			XmlAttribute typeAttr = doc.CreateAttribute("dmgType");
			typeAttr.Value = Damage.DamageTypeToString(dmgType);
			r.SetAttributeNode(typeAttr);

			return r;
		}

		internal override void Apply() {
			sendableTower.Target.TakeDamage(amount);
		}

		internal override void Revert() {
			sendableTower.Target.UndoTakeDamage(amount);
		}
	}
}