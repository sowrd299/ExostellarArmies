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

		public TowerDelta(Tower t, int amt) {
			sendableTower = new SendableTarget<Tower>("tower", t);
			amount = amt;
		}

		public TowerDelta(XmlElement from, CardLoader loader)
			: base(from, loader)
		{
			sendableTower = new SendableTarget<Tower>("tower", from, Tower.idIssuer);
			amount = Int32.Parse(from.Attributes["amount"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);

			r.SetAttributeNode(sendableTower.ToXml(doc));
			
			XmlAttribute amtAttr = doc.CreateAttribute("amount");
			amtAttr.Value = "" + amount;
			r.SetAttributeNode(amtAttr);

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