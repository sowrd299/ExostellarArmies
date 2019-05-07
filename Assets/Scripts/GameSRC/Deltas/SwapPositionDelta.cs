using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game
{
	public class SwapPositionDelta : Delta
	{
		private SendableTarget<Lane> SendableLane1;
		private int SideIndex1, Position1;
		private SendableTarget<Lane> SendableLane2;
		private int SideIndex2, Position2;

		private Unit Unit1 {
			get { return SendableLane1.Target.Units[SideIndex1, Position1]; }
		}
		private Unit Unit2 {
			get { return SendableLane2.Target.Units[SideIndex2, Position2]; }
		}

		public SwapPositionDelta(Lane l1, int side1, int pos1, Lane l2, int side2, int pos2)
			: base()
		{
			SendableLane1 = new SendableTarget<Lane>("lane1", l1);
			SideIndex1 = side1;
			Position1 = pos1;
			SendableLane2 = new SendableTarget<Lane>("lane2", l2);
			SideIndex2 = side2;
			Position2 = pos2;
		}

		public SwapPositionDelta(XmlElement from, CardLoader loader)
			: base(from, loader)
		{
			SendableLane1 = new SendableTarget<Lane>("lane1", from, Lane.IdIssuer);
			this.SideIndex1 = Int32.Parse(from.Attributes["sideIndex1"].Value);
			this.Position1 = Int32.Parse(from.Attributes["position1"].Value);
			SendableLane2 = new SendableTarget<Lane>("lane2", from, Lane.IdIssuer);
			this.SideIndex2 = Int32.Parse(from.Attributes["sideIndex2"].Value);
			this.Position2 = Int32.Parse(from.Attributes["position2"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc)
		{
			XmlElement r = base.ToXml(doc);

			r.SetAttributeNode(SendableLane1.ToXml(doc));

			XmlAttribute sideIndex1Attr = doc.CreateAttribute("sideIndex1");
			sideIndex1Attr.Value = "" + SideIndex1;
			r.SetAttributeNode(sideIndex1Attr);

			XmlAttribute position1Attr = doc.CreateAttribute("position1");
			position1Attr.Value = "" + SideIndex2;
			r.SetAttributeNode(position1Attr);

			r.SetAttributeNode(SendableLane2.ToXml(doc));

			XmlAttribute sideIndex2Attr = doc.CreateAttribute("sideIndex2");
			sideIndex2Attr.Value = "" + SideIndex1;
			r.SetAttributeNode(sideIndex2Attr);

			XmlAttribute position2Attr = doc.CreateAttribute("position2");
			position2Attr.Value = "" + SideIndex2;
			r.SetAttributeNode(position2Attr);

			return r;
		}

		internal override void Apply()
		{
			Unit u = Unit1;
			SendableLane1.Target.Units[SideIndex1, Position1] = Unit2;
			SendableLane2.Target.Units[SideIndex2, Position2] = u;
		}

		internal override void Revert() { Apply(); }
	}
}