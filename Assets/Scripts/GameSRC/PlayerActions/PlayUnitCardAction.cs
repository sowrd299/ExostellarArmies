using System.Collections;
using System.Collections.Generic;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game.Management {
	public class PlayUnitCardAction : PlayerAction {

		private SendableTarget<Card> cardTarget;
		public UnitCard card{
			get{ return cardTarget.Target as UnitCard; }
		}

		private SendableTarget<Lane> laneTarget;
		private Lane lane{
			get{ return laneTarget.Target; }
		}

		private int pos;

		private int sideIndex; 
		
		public PlayUnitCardAction(UnitCard card, Lane lane, int sideIndex, int position) {
			this.cardTarget = new SendableTarget<Card>("card",card);
			this.laneTarget = new SendableTarget<Lane>("lane",lane);
			this.pos = position;
			this.sideIndex = sideIndex;
        }

		// constructor used by PlayerAction.FromXml
		public PlayUnitCardAction(XmlElement e, IdIssuer<Card> cl, IdIssuer<Lane> lanes){
			this.cardTarget = new SendableTarget<Card>("card", e, cl);
			this.laneTarget = new SendableTarget<Lane>("lane", e, lanes);
			this.pos = GetXmlInt(e, "pos");
			this.sideIndex = GetXmlInt(e, "sideIndex");
		}

		public override XmlElement ToXml(XmlDocument doc){
			XmlElement e = base.ToXml(doc);
			e.SetAttributeNode(cardTarget.ToXml(doc));
			e.SetAttributeNode(laneTarget.ToXml(doc));
			SetXmlInt(doc, e, "pos", pos);
			SetXmlInt(doc, e, "sideIndex", sideIndex);
			return e;
		}

		internal override bool IsLegalAction(Player p) {
			// TODO: testing
        	bool r = p.Hand.Contains(card) &&
					!lane.IsOccupied(sideIndex, pos) &&
					p.ManaPool.CanAfford(card.DeployCost);
			if(!r){
				Console.WriteLine("Checking legal action: {0}, {1}, {2}",
						p.Hand.Contains(card) ? "Have card" : "Don't have card",
						!lane.IsOccupied(sideIndex, pos) ? "Space is free" : "Space isn't free",
						p.ManaPool.CanAfford(card.DeployCost) ? "Can afford" : "Can't afford");
			}
			return r;
		}
		
		internal override Delta[] GetDeltas(Player p, GameManager gm) {
			return new Delta[] {
				p.Hand.GetRemoveDelta(card)[0],
				p.ManaPool.GetSubtractDeltas(card.DeployCost)[0],
				lane.GetDeployDeltas(card, sideIndex, pos, gm)[0]
			};
		}
	}
}