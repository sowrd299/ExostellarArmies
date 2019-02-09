using System.Collections;
using System.Collections.Generic;
using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;

namespace SFB.Game.Content {
	public class Lane : IIDed {
		private static IdIssuer<Lane> idIssuer = new IdIssuer<Lane>();
		public static IdIssuer<Lane> IdIssuer {
			get { return idIssuer; }
		}

		private readonly string id;
		public string ID {
			get { return id; }
		}

		public Tower[] towers;
		internal Unit[,] unitss; // 0 front, 1 back

		public Lane(string id = "") {
			if(id == "") {
				this.id = idIssuer.IssueId(this);
			} else {
				idIssuer.RegisterId(id, this);
				this.id = id;
			}

			towers = new Tower[2] { new Tower(), new Tower() };
			unitss = new Unit[2, 2];
		}

		public bool isOccupied(int player, int pos) {
			return unitss[player, pos] != null;
		}

		private void remove(int player, int pos) {
			unitss[player, pos] = null;
		}
		
		private void place(UnitCard uc, int player, int pos) {
			unitss[player, pos] = new Unit(uc);
		}

		internal void placeFront(UnitCard uc, int p) { place(uc, p,  0); }
		internal void placeBack (UnitCard uc, int p) { place(uc, p,  1); }

		public void advance() {
			if(isOccupied(0, 1) && !isOccupied(0, 0)) {
				unitss[0, 0] = unitss[0, 1];
				unitss[0, 1] = null;
			}
			if(isOccupied(1, 1) && !isOccupied(1, 0)) {
				unitss[1, 0] = unitss[1, 1];
				unitss[1, 1] = null;
			}
		}

		/*public void doCombat() {
			doRangedCombat();
			doMeleeCombat();
		}

		public void doRangedCombat() {
			foreach(Unit u in yourUnits)
		}

		public void doMeleeCombat() {

		}*/

		public class AddToLaneDelta : TargetedDelta<Lane> {
			private int player;
			private int pos;

			internal AddToLaneDelta(Lane lane, UnitCard card, int player, int pos) : base(lane) {
				this.card = new SendableTarget<Card>("card", card);
				this.player = player;
				this.pos = pos;
			}

			public AddToLaneDelta(XmlElement element, CardLoader loader) : base(element, Lane.IdIssuer, loader) { }

			
			public override bool VisibleTo(Player p) {
				return true; // i believe so
			}

			internal override void Apply() {
				if((target as Lane).isOccupied(this.player, this.pos))
					throw new IllegalDeltaException("The lane and position you wish to put that Unit is already occupied");
				(target as Lane).place(this.card.Target as UnitCard, this.player, this.pos);
			}

			internal override void Revert() {
				if(!(target as Lane).isOccupied(this.player, this.pos))
					throw new IllegalDeltaException("The lane and position you wish to remove that Unit from is already empty");

				(target as Lane).remove(this.player, this.pos);
			}

		}
	}
}