using SFB.Game.Management;
using SFB.Game;
using System.Collections.Generic;
using System.Xml;
using SFB.Game.Content;

public class CommercialComsRelayIR : InputRequest<Card>
{
	public GameManager GameManager { get; private set; }
	public Player Player { get; private set; }

	public CommercialComsRelayIR(Player p, GameManager gm)
	{
		GameManager = gm;
		Player = p;
	}

	public CommercialComsRelayIR(XmlElement e) :
	base(e, CardLoader.instance)
	{ }

	public override bool IsLegalChoice(Card chosen)
	{
		return Player.Hand.Contains(chosen);
	}

	protected override bool MakeChoiceFrom(XmlElement e)
	{
		return MakeChoiceFrom(e, CardLoader.instance);
	}

	// put a card from hand beneath the top 4 cards of your deck
	public override Delta[] GetDeltas()
	{
		List<Delta> deltas = new List<Delta>();

		deltas.AddRange(Player.Hand.GetRemoveDelta(chosen.Target));
		deltas.Add(new AddToDeckIndexDelta(Player.Deck, chosen.Target, 4));

		return deltas.ToArray();
	}

}
