using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game
{
	public class AddToLaneDelta : TargetedDelta<Lane>
	{
		public int SideIndex { get; private set; }
		public int Position { get; private set; }
		private Unit Unit; // but only id sent; rest handled via card

		public AddToLaneDelta(Lane lane, UnitCard card, int sideIndex, int pos, GameManager gm)
			: base(lane)
		{
			this.SendableCard = new SendableTarget<Card>("card", card);
			this.SideIndex = sideIndex;
			this.Position = pos;
			this.Unit = new Unit(card, gm);
		}

		public AddToLaneDelta(XmlElement from, CardLoader loader)
			: base(from, Lane.IdIssuer, loader)
		{
			this.SendableCard = new SendableTarget<Card>("card", from, loader);
			this.SideIndex = Int32.Parse(from.Attributes["sideIndex"].Value);
			this.Position = Int32.Parse(from.Attributes["position"].Value);
			this.Unit = new Unit(SendableCard.Target as UnitCard, Int32.Parse(from.Attributes["unitId"].Value));
		}

		public override XmlElement ToXml(XmlDocument doc)
		{
			XmlElement r = base.ToXml(doc);

			r.SetAttributeNode(SendableCard.ToXml(doc));

			XmlAttribute sideIndexAttr = doc.CreateAttribute("sideIndex");
			sideIndexAttr.Value = "" + SideIndex;
			r.SetAttributeNode(sideIndexAttr);

			XmlAttribute posAttr = doc.CreateAttribute("position");
			posAttr.Value = "" + Position;
			r.SetAttributeNode(posAttr);

			XmlAttribute unitIdAttr = doc.CreateAttribute("unitId");
			unitIdAttr.Value = Unit.id;
			r.SetAttributeNode(unitIdAttr);

			return r;
		}


		public override bool VisibleTo(Player p)
		{
			return true;
		}

		internal override void Apply()
		{
			if (Target.IsOccupied(this.SideIndex, this.Position))
				throw new IllegalDeltaException("The lane and position you wish to put that Unit is already occupied");
			Target.Place(Unit, this.SideIndex, this.Position);
		}

		internal override void Revert()
		{
			if (!Target.IsOccupied(this.SideIndex, this.Position))
				throw new IllegalDeltaException("The lane and position you wish to remove that Unit from is already empty");

			Target.Kill(this.SideIndex, this.Position);
		}
	}
}