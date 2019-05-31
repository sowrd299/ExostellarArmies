using SFB.Game.Management;
using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SFB.Game.Content {

    public class CardLoader : IdIssuer<Card>
	{ // maybe this doesn't want to be an IdIssuer... sure feels like it does though
        IDictionary<string, Card> old_listOfCards = new Dictionary<string, Card>();
		
		public void AddUnitCard(string name, int cost, Faction f, string uType, string mText, string flavor, int r, int m, int hp, params Ability[] a) {
			old_listOfCards.Add(name, new UnitCard(cost, name, f, uType, mText, flavor, r, m, hp, a));
		}

		public CardLoader() {
			string path = Path.Combine("Assets", "Resources", "Cards");
			foreach(string fileName in Directory.EnumerateFiles(path, "*.yaml")) {
				YamlStream yaml = new YamlStream();
				yaml.Load(File.OpenText(fileName));

				YamlMappingNode root = yaml.Documents[0].RootNode as YamlMappingNode;
				string cardName = root.Children[new YamlScalarNode("name")].ToString();
				int cost = int.Parse(root.Children[new YamlScalarNode("cost")].ToString());
				/*
				DeckList deck = new DeckList();
				YamlSequenceNode cardSequence = root.Children[new YamlScalarNode("cards")] as YamlSequenceNode;
				foreach(YamlMappingNode cardItem in cardSequence) {
					deck.AddCard(
						cardLoader.GetByID(cardItem.Children[new YamlScalarNode("id")].ToString()),
						int.Parse(cardItem.Children[new YamlScalarNode("count")].ToString())
					);
				}

				decks.Add(deckId, deck);
				int deckCardCount = deck.GetCards().Select(deck.GetCopiesOf).Sum();*/
				Console.WriteLine($"Loaded card {fileName} / {cardName} / {cost}.");
			}


			AddUnitCard("Outergrowth Husk", 0, Faction.MYXOR, "Myxori Infantry Unit",
				"Absorb (excess damage done to this does not overflow)", "Myxi jha maRtox lOz!",
				0, 0, 1, new Absorb());
			AddUnitCard("Scion Infiltrator 3rd Class", 1, Faction.JIRNOR, "Jirnorn Mutant Infantry Unit",
				"Lob (in combat, this deals damage to whatever is behind the opposing front line)", "Small, Innocuous and Hopeful",
				1, 1, 1, new Lob());
			AddUnitCard("Grafted Claw Spunner", 1, Faction.MYXOR, "Myxi Infantry Unit",
				"Swarm (when you deploy this, if it is behind a Unit that shares a tag with it other than “Unit”, draw a card)", "\"Myxori jha maRtox lOz!\"",
				0, 2, 2, new Swarm());
			old_listOfCards.Add("Mercenary Phantasm", new UnitCard(2, "Mercenary Phantasm", Faction.NONE, "", "Deploy: Your opponent reveals a card from their hand", "What do you want to know?", 1, 2, 3));
			old_listOfCards.Add("Commercial Shipper", new UnitCard(2, "Commercial Shipper", Faction.NONE, "", "Deploy: Draw a card, then put a card from your hand on the bottom of your deck", "Whatever you need, we got", 0, 3, 3));
			old_listOfCards.Add("Adaptive Protopod", new UnitCard(2, "Adaptive Protopod", Faction.MYXOR, "", "Support Infantry: This front line gains Spore 3 until end of turn.\nSupport Artillery: This gains Siege 1 until end of turn",
				"MyxI jha maRtox lOz!", 2, 0, 2));
			AddUnitCard("Screaming Spore Spunner", 1, Faction.MYXOR, "",
				"Spore 1 (when this dies, gain 1 )", "Muxi ji maRtox raj!",
				0, 2, 2, new Spore(1));
			old_listOfCards.Add("Pod-Cap Yxuu maRroz", new UnitCard(1, "Pod-Cap Yxuu maRroz", Faction.MYXOR, "", "Unique\nDeploy:Allied Infantry get +1M this turn.", "Far-Flung Mind-Spore Pod", 1, 0, 2));
			AddUnitCard("Undergrowth Smasher", 5, Faction.MYXOR, "",
				"Siege 1\nDeploy: Deal 1 damage to the enemy in front of this for each Allied Myxori Unit", "Myxi jha maRtox lOz!",
				0, 4, 4, new Siege(1));
			AddUnitCard("Aftergrowth Spunner", 2, Faction.MYXOR, "",
				"Spore  3 (when this dies, gain 3 s)", "toxhMar IOz Yzhmat rho!",
				0, 2, 2, new Spore(3));
			AddUnitCard("Mulch Pod", 2, Faction.MYXOR, "",
				"Absorb\nDeath:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)", "Myxi jha maRtox IOz",
				0, 1, 1, new Absorb());
			old_listOfCards.Add("Fecund Reflex Axon", new UnitCard(2, "Fecund Reflex Axon", Faction.MYXOR, "", "Deploy...\nSupporting Myxori: Gain 3 s and this gets +3R this turn.\n" +
					"Front Line:If there is no back line here, this gains Absorb until the end of turn.", "Myxi jha maRtox IOz", 0, 2, 2));
			old_listOfCards.Add("Grafted Fang Spunner", new UnitCard(1, "Grafted Fang Spunner", Faction.MYXOR, "", "Death:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
					"Myxi jha maRtox IOz", 0, 2, 2));
			AddUnitCard("Spore Pod", 1, Faction.MYXOR, "",
				"Spore 1 (when this dies, gain 1 )\nDeath:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
				"Myxi jha maRtox IOz",
				0, 0, 1, new Spore(1));
			old_listOfCards.Add("Horde Caller", new UnitCard(3, "Horde Caller", Faction.MYXOR, "", "Spore 2 (when this dies, gain 3 s)\nDeath: +1 Extra deploy", "MuxhRaz izt zraZxh",
					0, 3, 2, new Spore(2)));
			old_listOfCards.Add("Titanous Regrowth Trunk", new UnitCard(6, "Titanous Regrowth Trunk", Faction.MYXOR, "", "Persistent: Other allies gain Spore +1(when this dies, gain 1 )\n" +
			   "Front Line Persistent: Whenever an adjacent Font Line Infantry dies, Beta Swarm there", "Myxi jha maRtox lOz!", 0, 3, 7));
			old_listOfCards.Add("Exostellar Marine Squad", new UnitCard(3, "Exostellar Marine Squad", Faction.CARTH, "", "",
					"Bravely into the Darkness", 2, 2, 4));
			old_listOfCards.Add("Commercial Coms Relay", new UnitCard(0, "Commercial Coms Relay", Faction.NONE, "", "Deploy: Put a card from your hand beneath the top 4 cards of your deck.",
					"Binding the Stars", 0, 1, 2));
			old_listOfCards.Add("Adv. Infantry Support System", new UnitCard(5, "Adv. Infantry Support System", Faction.CARTH, "", "Melee Shield 1\nPersistent: At the start of your turn, generate an extra .",
					"Mobile Command Center", 0, 3, 6, new MeleeShield(1), new AdvInfantrySupportSystem()));
			old_listOfCards.Add("Paladin-Class XS Marines", new UnitCard(4, "Paladin-Class XS Marines", Faction.CARTH, "", "Front Line Recurring Deploy: If there is not a unit behind this, heal each adjacent allied front line 1.",
					"Defense Against the Darkness", 1, 3, 5, new PaladinClassExSMarines()));
			old_listOfCards.Add("Adv. Supply Drone", new UnitCard(2, "Adv. Supply Drone", Faction.CARTH, "", "Supporting Carthan Deploy: Heal the supported unit 2 and gain 3 s.",
					"762491", 0, 1, 2, new AdvSupplyDrone()));
			old_listOfCards.Add("XS Field Technician", new UnitCard(3, "XS Field Technician", Faction.CARTH, "", "Suporting Drone Deploy: Return the supported unit to your hand.",
					"Tuning Blades in the Darkness", 1, 2, 4, new XSFieldTechnician()));
			old_listOfCards.Add("Lt. Mgr. Tul Yorves", new UnitCard(4, "Lt. Mgr. Tul Yorves", Faction.CARTH, "", "Unique; Ranged Shield 1\nFront Line Persistent: Whenever you heal a Unit, heal it for 1 more and it deals 1 damage.",
					"Master Medic", 1, 3, 5, new RangedShield(2)));
			old_listOfCards.Add("Exostellar Snipers", new UnitCard(3, "Exostellar Snipers", Faction.CARTH, "", "",
					"Illuminate with Muzzle Flare!", 3, 1, 3));
			old_listOfCards.Add("Cmdr. Yos Lorth", new UnitCard(4, "Cmdr. Yos Lorth", Faction.CARTH, "", "Unique; Ranged Shield 1\nFront Line Persistent: Allied front line Elites have +1 and Ranged Shield +1",
					"Exostellar Champion", 2, 2, 5, new RangedShield(1), new CmdrYosLorth()));
			old_listOfCards.Add("Ancillary Medical Officer", new UnitCard(2, "Ancillary Medical Officer", Faction.CARTH, "", "Supporting Carthan Infantry Deploy: Heal this front line 2.",
					"Healer in the Darkness", 1, 2, 3, new AncillaryMedicalOfficer()));
			old_listOfCards.Add("Battle-Line Trauma Medic", new UnitCard(3, "Battle-Line Trauma Medic", Faction.CARTH, "", "Supporting Carthan Deploy: Discard a card; heal each ally 2.",
					"Forever Alive in the Dark!", 0, 2, 4));
			old_listOfCards.Add("Emergency Med Drop", new UnitCard(0, "Emergency Med Drop", Faction.CARTH, "", "Deploy: Heal this front line 2 and each adjacent front line 1",
					"756328", 0, 0, 1, new EmergencyMedDrop()));
            old_listOfCards.Add("Cannoneer Drone", new UnitCard(2, "Cannoneer Drone", Faction.CARTH, "", "Ranged Shield 2; Melee Shield 2\nFront Line Persistent: The tower behind this deals +3 damge.",
                    "Death to Your Enemies", 0, 0, 1, new RangedShield(2), new MeleeShield(2)));
            old_listOfCards.Add("Autonomous Range Finder", new UnitCard(2, "Autonomous Range Finder", Faction.CARTH, "", "Supporting Carthan Deploy: Give this front line +3R this turn.",
                    "56413", 0, 1, 3, new AutonomousRangeFinder()));

			old_listOfCards.Add("Resist Token", new UnitCard(0, "Resist Token", Faction.NONE, "Token", "Tower Shield 1", "", 0, 0, 10, new TowerShield(1)));
			old_listOfCards.Add("Mana Token", new UnitCard(0, "Mana Token", Faction.NONE, "Token", "Spore 10", "", 0, 0, 1, new Spore(10)));

			old_listOfCards.Add("Attack Token 1", new UnitCard(0, "Attack Token 1", Faction.NONE, "Token", "", "", 1, 1, 1));
			old_listOfCards.Add("Attack Token 2", new UnitCard(0, "Attack Token 2", Faction.NONE, "Token", "", "", 2, 2, 1));
			old_listOfCards.Add("Attack Token 3", new UnitCard(0, "Attack Token 3", Faction.NONE, "Token", "", "", 3, 3, 1));
			old_listOfCards.Add("Attack Token 4", new UnitCard(0, "Attack Token 4", Faction.NONE, "Token", "", "", 4, 4, 1));
			old_listOfCards.Add("Attack Token 5", new UnitCard(0, "Attack Token 5", Faction.NONE, "Token", "", "", 5, 5, 1));
			old_listOfCards.Add("Attack Token 6", new UnitCard(0, "Attack Token 6", Faction.NONE, "Token", "", "", 6, 6, 1));
			old_listOfCards.Add("Attack Token 10", new UnitCard(0, "Attack Token 10", Faction.NONE, "Token", "", "", 10, 10, 1));
		}

        protected override Card handleMiss(string id){
			if(old_listOfCards.ContainsKey(id))
				return old_listOfCards[id];
			else
				throw new System.Exception("Card \"" + id + "\" not found");
        }

    }
    
}