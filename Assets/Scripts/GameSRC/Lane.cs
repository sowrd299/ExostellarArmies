using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

		public Tower yourTower, theirTower;
		internal Unit[] yourUnits, theirUnits; // 0 front, 1 back

		public Lane(string id = "") {
			if(id == "") {
				this.id = idIssuer.IssueId(this);
			} else {
				idIssuer.RegisterId(id, this);
				this.id = id;
			}

			yourTower = new Tower();
			theirTower = new Tower();
			yourUnits = new Unit[2];
			theirUnits = new Unit[2];
		}

		public bool isOccupied(bool yourSide, int i) {
			return (yourSide ? yourUnits : theirUnits)[i] != null;
		}

		private void remove(bool yourSide, int i) {
			(yourSide ? yourUnits : theirUnits)[i] = null;
		}
		
		private void place(UnitCard uc, bool yourSide, int i) {
			(yourSide ? yourUnits : theirUnits)[i] = new Unit(uc);
		}

		internal void placeYourFront (UnitCard uc) { place(uc, true,  0); }
		internal void placeYourBack  (UnitCard uc) { place(uc, true,  1); }
		internal void placeTheirFront(UnitCard uc) { place(uc, false, 0); }
		internal void placeTheirBack (UnitCard uc) { place(uc, false, 1); }

		public void advance() {
			if(isOccupied(true, 1) && !isOccupied(true, 0)) {
				yourUnits[0] = yourUnits[1];
				yourUnits[1] = null;
			}
			if(isOccupied(false, 1) && !isOccupied(false, 0)) {
				theirUnits[0] = theirUnits[1];
				theirUnits[1] = null;
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
			private bool yourSide;
			private int pos;

			internal AddToLaneDelta(Lane lane, UnitCard card, bool yourSide, int pos) : base(lane) {
				this.card = card;
				this.yourSide = yourSide;
				this.pos = pos;
			}

			public AddToLaneDelta(XmlElement element, CardLoader loader) : base(element, Lane.IdIssuer, loader) { }

			
			public override bool VisibleTo(Player p) {
				return true; // i believe so
			}

			internal override void Apply() {
				if((target as Lane).isOccupied(this.yourSide, this.pos))
					throw new IllegalDeltaException("The lane and position you wish to put that Unit is already occupied");
				(target as Lane).place(this.card as UnitCard, this.yourSide, this.pos);
			}

			internal override void Revert() {
				if(!(target as Lane).isOccupied(this.yourSide, this.pos))
					throw new IllegalDeltaException("The lane and position you wish to remove that Unit from is already empty");

				(target as Lane).remove(this.yourSide, this.pos);
			}

		}
	}
}