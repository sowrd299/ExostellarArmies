using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game
{
	public class RemoveFromLaneDelta : TargetedDelta<Lane>
	{
		public int SideIndex { get; private set; }
		public int Position { get; private set; }

		public RemoveFromLaneDelta(Lane lane, int sideIndex, int pos)
			: base(lane)
		{
			this.SideIndex = sideIndex;
			this.Position = pos;
		}

		public RemoveFromLaneDelta(XmlElement from, CardLoader loader)
			: base(from, Lane.IdIssuer, loader)
		{
			this.SideIndex = Int32.Parse(from.Attributes["sideIndex"].Value);
			this.Position = Int32.Parse(from.Attributes["position"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc)
		{
			XmlElement r = base.ToXml(doc);

			XmlAttribute sideIndexAttr = doc.CreateAttribute("sideIndex");
			sideIndexAttr.Value = "" + SideIndex;
			r.SetAttributeNode(sideIndexAttr);

			XmlAttribute posAttr = doc.CreateAttribute("position");
			posAttr.Value = "" + Position;
			r.SetAttributeNode(posAttr);

			return r;
		}

		public override bool VisibleTo(Player p) {
			return true;
		}

		internal override void Apply() {
			if(!Target.IsOccupied(this.SideIndex, this.Position))
				throw new IllegalDeltaException("Tried to remove a Unit from an empty lane/side/position.");
			Target.Kill(this.SideIndex, this.Position);
		}

		internal override void Revert() {
			throw new IllegalDeltaException("The lane and position you wish to remove that Unit from is already empty");
		}
	}
}