using SFB.Game.Management;
using SFB.Game;
using System.Collections.Generic;
using System.Xml;
using SFB.Game.Content;

public class CommercialShipperIR : InputRequest<Card>
{
	public GameManager GameManager { get; private set; }
	public Player Player { get; private set; }

	public CommercialShipperIR(Player p, GameManager gm)
	{
		GameManager = gm;
		Player = p;
	}

	public override bool IsLegalChoice(Card chosen)
	{
		return Player.Hand.Contains(chosen);
	}

	protected override bool MakeChoiceFrom(XmlElement e)
	{
		return MakeChoiceFrom(e, new CardLoader());
	}

	// put a card from hand on the bottom of your deck
	public override Delta[] GetDeltas()
	{
		List<Delta> deltas = new List<Delta>();

		deltas.AddRange(Player.Hand.GetRemoveDelta(chosen.Target));
		deltas.Add(new AddToBottomDeckDelta(Player.Deck, chosen.Target));

		return deltas.ToArray();
	}

}
