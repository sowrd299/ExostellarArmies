﻿using SFB.Game.Management;
using SFB.Game;
using System.Collections.Generic;
using System.Xml;
using SFB.Game.Content;

public class CommercialComsRelayIR : InputRequest<Card>
{
	public GameManager GameManager { get; private set; }
	public Deck Deck { get; private set; }
	public Hand Hand { get; private set; }

	public CommercialComsRelayIR(Player p, GameManager gm)
	{
		GameManager = gm;
		Deck = p.Deck;
		Hand = p.Hand;
	}

	public CommercialComsRelayIR(XmlElement e) :
		base(e, CardLoader.instance)
	{
		Deck = Deck.IdIssuer.GetByID(e.GetAttribute("deckId"));
		Hand = Hand.IdIssuer.GetByID(e.GetAttribute("handId"));
	}

	public override XmlElement ToXml(XmlDocument doc)
	{
		XmlElement e = base.ToXml(doc);

		e.SetAttribute("deckId", Deck.ID);
		e.SetAttribute("handId", Hand.ID);

		return e;
	}

	public override bool IsLegalChoice(Card chosen)
	{
		return Hand.Contains(chosen);
	}

	protected override bool MakeChoiceFrom(XmlElement e)
	{
		return MakeChoiceFrom(e, CardLoader.instance);
	}

	// put a card from hand beneath the top 4 cards of your deck
	public override Delta[] GetDeltas()
	{
		List<Delta> deltas = new List<Delta>();

		deltas.AddRange(Hand.GetRemoveDelta(chosen.Target));
		deltas.Add(new AddToDeckIndexDelta(Deck, chosen.Target, 4));

		return deltas.ToArray();
	}

}
