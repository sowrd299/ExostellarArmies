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

		public Tuple<int, int> GetSidePosOf(Unit unit)	
		{
			for(int side = 0; side < 2; side++)
				for(int pos = 0; pos < 2; pos++)
					if(Units[side, pos] == unit)
						return new Tuple<int, int>(side, pos);
			return null;
		}

		public static Tuple<int, int, int> GetLaneSidePosOf(Unit unit, Lane[] lanes) {
			for(int lane = 0; lane < lanes.Length; lane++) {
				Tuple<int, int> sidePos = lanes[lane].GetSidePosOf(unit);
				if(sidePos != null)
					return new Tuple<int, int, int>(lane, sidePos.Item1, sidePos.Item2);
			}
			return null;
		}

		public static int GetLaneIndexOf(Lane l, Lane[] lanes) {
			for(int i = 0; i < lanes.Length; i++)
				if(lanes[i] == l)
					return i;
			throw new System.Exception("Lane not found");
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

		public void Place(UnitCard uc, int side, int pos, GameManager gm)
		{
			Units[side, pos] = new Unit(uc, gm);
		}

		public void Place(Unit u, int side, int pos)
		{
			Units[side, pos] = u;
		}

		public bool NeedFillFront(int side) {
			return Units[side, 0] == null && Units[side, 1] != null;
		}


		public Delta[] GetDeployDeltas(UnitCard card, int side, int pos, GameManager gm)
		{
			List<Delta> deltas = new List<Delta>() { new AddToLaneDelta(this, card, side, pos, gm) };
			gm.UseAddBoardUpdateDeltas(deltas, BoardUpdate.GetAdd(Lane.GetLaneIndexOf(this, gm.Lanes), side, pos));
			return deltas.ToArray();
		}

		public Delta[] GetDeathDeltas(int side, int pos, GameManager gm)
		{
			RemoveFromLaneDelta rmd = new RemoveFromLaneDelta(this, side, pos);
			List<Delta> deltas = new List<Delta> { rmd };
			gm.UseAddUnitDeathDeltas(deltas, Units[rmd.SideIndex, rmd.Position]);
			gm.UseAddBoardUpdateDeltas(deltas, BoardUpdate.GetRemove(Lane.GetLaneIndexOf(this, gm.Lanes), side, pos));
			return deltas.ToArray();
		}

		public Delta[] GetInLaneSwapDeltas(int side, GameManager gm)
		{
			List<Delta> deltas = new List<Delta> { new InLaneSwapDelta(this, side) };
			gm.UseAddBoardUpdateDeltas(deltas, BoardUpdate.GetSwap(Lane.GetLaneIndexOf(this, gm.Lanes), side));
			return deltas.ToArray();
		}
	}
}