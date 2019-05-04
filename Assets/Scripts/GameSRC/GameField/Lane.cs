using System.Collections;
using System.Collections.Generic;
using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game.Content
{
	public class Lane : IIDed
	{
		public const string TAG_NAME = "laneIds";

		private static IdIssuer<Lane> idIssuer = new IdIssuer<Lane>();
		public static IdIssuer<Lane> IdIssuer
		{
			get { return idIssuer; }
		}

		private readonly string _ID;
		public string ID { get { return _ID; } }

		// Index corresponds to player sideIndex
		public Tower[] Towers { get; private set; }

		// For Units[a, b]:
		//   Index a corresponds to player sideIndex
		//   Index b corresponds to 0 front, 1 back
		public Unit[,] Units { get; private set; }

		public Lane()
		{
			_ID = idIssuer.IssueId(this);

			Towers = new Tower[2] { new Tower(), new Tower() };
			Units = new Unit[2, 2];
		}

		public Lane(XmlElement from)
		{
			_ID = from.Attributes["id"].Value;
			idIssuer.RegisterId(_ID, this);

			Towers = new Tower[2];
			foreach (XmlElement towerElement in from.GetElementsByTagName(Tower.TAG_NAME))
			{
				int towerIndex = int.Parse(towerElement.Attributes["index"].Value);
				Towers[towerIndex] = new Tower(towerElement);
			}
			Units = new Unit[2, 2];
		}

		public XmlElement ToXml(XmlDocument doc, int index)
		{
			XmlElement element = doc.CreateElement(TAG_NAME);

			element.SetAttribute("id", ID);
			element.SetAttribute("index", index.ToString());

			for (int i = 0; i < Towers.Length; i++)
			{
				element.AppendChild(Towers[i].ToXml(doc, i));
			}

			return element;
		}

		public bool Contains(Unit unit)
		{
			foreach (Unit u in Units)
				if (unit == u)
					return true;
			return false;
		}

		public void Kill(int side, int pos)
		{
			Units[side, pos] = null;
		}

		public void Kill(Unit u)
		{
			for (int side = 0; side < Units.GetLength(0); side++)
			{
				for (int pos = 0; pos < Units.GetLength(1); pos++)
				{
					if (Units[side, pos] == u)
					{
						Kill(side, pos);
						return;
					}
				}
			}
			//error?
		}

		public bool IsOccupied(int side, int pos)
		{
			return Units[side, pos] != null;
		}

		private void Place(UnitCard uc, int side, int pos)
		{
			Units[side, pos] = new Unit(uc);
		}

		private void Place(Unit u, int side, int pos)
		{
			Units[side, pos] = u;
		}

		private void PlaceFront(UnitCard uc, int p) { Place(uc, p, 0); }
		private void PlaceBack(UnitCard uc, int p) { Place(uc, p, 1); }

		public bool NeedFillFront(int side) {
			return Units[side, 0] == null && Units[side, 1] != null;
		}


		public AddToLaneDelta[] GetDeployDeltas(UnitCard card, int side, int pos)
		{
			return new AddToLaneDelta[] { new AddToLaneDelta(this, card, side, pos) };
		}

		public RemoveFromLaneDelta[] GetDeathDeltas(int side, int pos)
		{
			return new RemoveFromLaneDelta[] { new RemoveFromLaneDelta(this, side, pos) };
		}

		public SwapPositionDelta[] GetSwapPositionDeltas(int side)
		{
			return new SwapPositionDelta[] { new SwapPositionDelta(this, side) };
		}


		public class SwapPositionDelta : TargetedDelta<Lane>
		{
			private int SideIndex;

			public SwapPositionDelta(Lane l, int s)
				: base(l)
			{
				SideIndex = s;
			}

			public SwapPositionDelta(XmlElement from, CardLoader loader)
				: base(from, Lane.IdIssuer, loader)
			{
				this.SideIndex = Int32.Parse(from.Attributes["sideIndex"].Value);
			}

			internal override void Apply()
			{
				Unit u = Target.Units[SideIndex, 0];
				Target.Units[SideIndex, 0] = Target.Units[SideIndex, 1];
				Target.Units[SideIndex, 1] = u;
			}

			public override XmlElement ToXml(XmlDocument doc)
			{
				XmlElement r = base.ToXml(doc);

				XmlAttribute sideIndexAttr = doc.CreateAttribute("sideIndex");
				sideIndexAttr.Value = "" + SideIndex;
				r.SetAttributeNode(sideIndexAttr);

				return r;
			}

			internal override void Revert() { Apply(); }
		}

		public class RemoveFromLaneDelta : TargetedDelta<Lane>
		{
			private int SideIndex;
			private int Position;

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

		public class AddToLaneDelta : TargetedDelta<Lane>
		{
			private int SideIndex;
			private int Position;
			private Unit Unit; // only id sent; rest handled via card

			public AddToLaneDelta(Lane lane, UnitCard card, int sideIndex, int pos)
				: base(lane)
			{
				this.SendableCard = new SendableTarget<Card>("card", card);
				this.SideIndex = sideIndex;
				this.Position = pos;
				this.Unit = new Unit(card);
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
				sideIndexAttr.Value = ""+SideIndex;
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
}