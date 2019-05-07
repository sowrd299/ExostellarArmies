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
			throw new System.Exception("Tried to kill a unit that isn't in this lane.");
		}

		public bool IsOccupied(int side, int pos)
		{
			return Units[side, pos] != null;
		}

		public void Place(UnitCard uc, int side, int pos)
		{
			Units[side, pos] = new Unit(uc);
		}

		public void Place(Unit u, int side, int pos)
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

		public InLaneSwapDelta[] GetInLaneSwapDeltas(int side)
		{
			return new InLaneSwapDelta[] { new InLaneSwapDelta(this, side) };
		}
	}
}