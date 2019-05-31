using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game
{
	public class ResourcePoolDelta : Management.Delta {

		private int amount;
		public SendableTarget<ResourcePool> rp { get; private set; }

		public ResourcePoolDelta(int amount, ResourcePool rp) {
			this.amount = amount;
			this.rp = new SendableTarget<ResourcePool>("poolId", rp);
		}

		public ResourcePoolDelta(XmlElement e, CardLoader cl)
				: base(e, cl) {
			this.rp = new SendableTarget<ResourcePool>("poolId", e, ResourcePool.IdIssuer);
			this.amount = Int32.Parse(e.Attributes["amount"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);
			r.SetAttributeNode(rp.ToXml(doc));
			XmlAttribute amount = doc.CreateAttribute("amount");
			amount.Value = this.amount.ToString();
			r.SetAttributeNode(amount);
			return r;
		}

		internal override void Apply() {
			if(amount > 0)
				rp.Target.Add(amount);
			else
				rp.Target.Subtract(-amount);
		}

		internal override void Revert() {
			if(amount > 0)
				rp.Target.Subtract(amount);
			else
				rp.Target.Add(-amount);
		}
	}
}