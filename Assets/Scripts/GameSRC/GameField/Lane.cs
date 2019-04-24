using System.Collections;
using System.Collections.Generic;
using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;

namespace SFB.Game.Content
{
	public class Lane : IIDed
	{
		private static IdIssuer<Lane> idIssuer = new IdIssuer<Lane>();
		public static IdIssuer<Lane> IdIssuer
		{
			get { return idIssuer; }
		}

		private readonly string id;
		public string ID
		{
			get { return id; }
		}

		//index corresponds to player index in master array
		private Tower[] towers;
		public Tower[] Towers
		{
			get { return towers; }
		}

		//index a in Units[a,b] corresponds to player index in master array
		//b corresponds to 0 front, 1 back
		internal Unit[,] Units { get; private set; }

		public Lane()
		{
			id = idIssuer.IssueId(this);

			towers = new Tower[2] { new Tower(), new Tower() };
			Units = new Unit[2, 2];
		}

		public Lane(XmlElement from)
		{
			id = from.Attributes["id"].Value;
			idIssuer.RegisterId(id, this);

			towers = new Tower[2];
			foreach (XmlElement towerElement in from.GetElementsByTagName("tower"))
			{
				int towerIndex = int.Parse(towerElement.Attributes["index"].Value);
				towers[towerIndex] = new Tower(towerElement);
			}
			Units = new Unit[2, 2];
		}

		public XmlElement ToXml(XmlDocument doc, int index)
		{
			XmlElement element = doc.CreateElement("laneIds");

			element.SetAttribute("id", ID);
			element.SetAttribute("index", index.ToString());

			for (int i = 0; i < towers.Length; i++)
			{
				element.AppendChild(towers[i].ToXml(doc, i));
			}

			return element;
		}

		internal bool contains(Unit unit)
		{
			foreach (Unit u in Units)
				if (unit == u)
					return true;
			return false;
		}

		internal void kill(int play, int pos)
		{
			Units[play, pos] = null;
		}

		internal void kill(Unit u)
		{
			for (int play = 0; play < Units.GetLength(0); play++)
			{
				for (int pos = 0; pos < Units.GetLength(1); pos++)
				{
					if (Units[play, pos] == u)
					{
						kill(play, pos);
						return;
					}
				}
			}
			//error?
		}

		public bool isOccupied(int player, int pos)
		{
			return Units[player, pos] != null;
		}

		private void remove(int player, int pos)
		{
			Units[player, pos] = null;
		}

		private void place(UnitCard uc, int player, int pos)
		{
			Units[player, pos] = new Unit(uc);
		}

		internal void placeFront(UnitCard uc, int p) { place(uc, p, 0); }
		internal void placeBack(UnitCard uc, int p) { place(uc, p, 1); }

		public void advance()
		{
			if (isOccupied(0, 1) && !isOccupied(0, 0))
			{
				Units[0, 0] = Units[0, 1];
				Units[0, 1] = null;
			}
			if (isOccupied(1, 1) && !isOccupied(1, 0))
			{
				Units[1, 0] = Units[1, 1];
				Units[1, 1] = null;
			}
		}

		internal AddToLaneDelta[] getDeployDeltas(UnitCard card, int play, int pos)
		{
			return new AddToLaneDelta[] { new AddToLaneDelta(this, card, play, pos) };
		}

		internal SwapPositionDelta[] getSwapPositionDeltas(int play)
		{
			return new SwapPositionDelta[] { new SwapPositionDelta(this, play) };
		}

		public class SwapPositionDelta : TargetedDelta<Lane>
		{
			private int play;
			private Lane lane;

			public SwapPositionDelta(Lane l, int p) : base(l)
			{
				lane = l;
				play = p;
			}

			public SwapPositionDelta(XmlElement element, CardLoader loader) : base(element, Lane.IdIssuer, loader) { }

			internal override void Apply()
			{
				Unit u = lane.Units[play, 0];
				lane.Units[play, 0] = lane.Units[play, 1];
				lane.Units[play, 1] = u;
			}

			internal override void Revert() { Apply(); }
		}


		public class AddToLaneDelta : TargetedDelta<Lane>
		{
			private int player;
			private int pos;

			internal AddToLaneDelta(Lane lane, UnitCard card, int player, int pos) : base(lane)
			{
				this.card = new SendableTarget<Card>("card", card);
				this.player = player;
				this.pos = pos;
			}

			public AddToLaneDelta(XmlElement element, CardLoader loader) : base(element, Lane.IdIssuer, loader) { }


			public override bool VisibleTo(Player p)
			{
				return true; // i believe so
			}

			internal override void Apply()
			{
				if ((target as Lane).isOccupied(this.player, this.pos))
					throw new IllegalDeltaException("The lane and position you wish to put that Unit is already occupied");
				(target as Lane).place(this.card.Target as UnitCard, this.player, this.pos);
			}

			internal override void Revert()
			{
				if (!(target as Lane).isOccupied(this.player, this.pos))
					throw new IllegalDeltaException("The lane and position you wish to remove that Unit from is already empty");

				(target as Lane).remove(this.player, this.pos);
			}
		}


	}
}