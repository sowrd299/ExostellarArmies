using System.Collections;
using System.Collections.Generic;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game.Management {
	public class PlayUnitCardAction : PlayerAction {

		SendableTarget<Card> cardTarget;
		private UnitCard card{
			get{ return cardTarget.Target as UnitCard; }
		}

		SendableTarget<Lane> laneTarget;
		private Lane lane{
			get{ return laneTarget.Target; }
		}

		private int pos;

		// TODO: this looks like it will cause problems
		private int play; 

		internal PlayUnitCardAction(UnitCard c, Lane l, int play, int pos) {
			this.cardTarget = new SendableTarget<Card>("card",c);
			this.laneTarget = new SendableTarget<Lane>("lane",l);
			this.pos = pos;
			this.play = play;
        }

		// constructor used by PlayerAction.FromXml
		public PlayUnitCardAction(XmlElement e, IdIssuer<Card> cl, IdIssuer<Lane> lanes){
			this.cardTarget = new SendableTarget<Card>("card", e, cl);
			this.laneTarget = new SendableTarget<Lane>("lane", e, lanes);
			this.pos = GetXmlInt(e, "pos");
			this.play = GetXmlInt(e, "play");
		}

		public override XmlElement ToXml(XmlDocument doc){
			XmlElement e = base.ToXml(doc);
			e.SetAttributeNode(cardTarget.ToXml(doc));
			e.SetAttributeNode(laneTarget.ToXml(doc));
			SetXmlInt(doc, e, "pos", pos);
			SetXmlInt(doc, e, "play", play);
			return e;
		}

		internal override bool IsLegalAction(Player p) {
			// TODO: testing
        	bool r = p.Hand.Contains(card) &&
					!lane.isOccupied(play, pos) &&
					p.Mana.CanAfford(card.DeployCost);
			if(!r){
				Console.WriteLine("Checking legal action: {0}, {1}, {2}",
						p.Hand.Contains(card) ? "Have card" : "Don't have card",
						!lane.isOccupied(play, pos) ? "Space is free" : "Space isn't free",
						p.Mana.CanAfford(card.DeployCost) ? "Can afford" : "Can't afford");
			}
			return r;
		}
		
		internal override Delta[] GetDeltas(Player p) {
			return new Delta[] {
				p.Hand.GetRemoveDelta(card)[0],
				p.Mana.GetSubtractDeltas(card.DeployCost)[0],
				lane.getDeployDeltas(card, play, pos)[0]
			};
		}
	}
}