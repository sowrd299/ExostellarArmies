using SFB.Game.Management;
using SFB.Game;
using System.Collections.Generic;

public class CommercialShipperIR : InputRequest<Card>
{
	public GameManager GameManager { get; private set; }

	public CommercialShipperIR(Player p, GameManager gm)
		: base(p)
	{
		GameManager = gm;
	}

	public override bool IsLegalChoice(Card chosen)
	{
		return Player.Hand.Contains(chosen);
	}

	// put a card from hand on the bottom of your deck
	public override Delta[] GetDeltas()
	{
		List<Delta> deltas = new List<Delta>();

		deltas.AddRange(Player.Hand.GetRemoveDelta(Chosen.Target));
		deltas.Add(new AddToBottomDeckDelta(Player.Deck, Chosen.Target));

		return deltas.ToArray();
	}

}
