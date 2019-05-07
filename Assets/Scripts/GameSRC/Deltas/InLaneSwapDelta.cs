using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game
{
	public class InLaneSwapDelta : TargetedDelta<Lane>
	{
		private int SideIndex;

		public InLaneSwapDelta(Lane l, int s)
			: base(l)
		{
			SideIndex = s;
		}

		public InLaneSwapDelta(XmlElement from, CardLoader loader)
			: base(from, Lane.IdIssuer, loader)
		{
			this.SideIndex = Int32.Parse(from.Attributes["sideIndex"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc)
		{
			XmlElement r = base.ToXml(doc);

			XmlAttribute sideIndexAttr = doc.CreateAttribute("sideIndex");
			sideIndexAttr.Value = "" + SideIndex;
			r.SetAttributeNode(sideIndexAttr);

			return r;
		}

		internal override void Apply()
		{
			Unit u = Target.Units[SideIndex, 0];
			Target.Units[SideIndex, 0] = Target.Units[SideIndex, 1];
			Target.Units[SideIndex, 1] = u;
		}

		internal override void Revert() { Apply(); }
	}
}