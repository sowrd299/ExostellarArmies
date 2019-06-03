using SFB.Game.Management;
using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SFB.Game.Content
{
    public class CardLoader : IdIssuer<Card>
	{
		private IDictionary<string, Card> listOfCards;
		private IDictionary<string, Card> old_listOfCards;
		
		public void AddUnitCardOLD(string name, int cost, Faction f, string uType, string mText, string flavor, int r, int m, int hp, params Ability[] a) {
			old_listOfCards.Add(name, new UnitCard(cost, name, f, uType, mText, flavor, r, m, hp, a));
		}

		public void AddUnitCard(string name, int cost, Faction f, string uType, string mText, string flavor, int r, int m, int hp, params Ability[] a) {
			UnitCard c = new UnitCard(cost, name, f, uType, mText, flavor, r, m, hp, a);
			Console.WriteLine(c.ToString());
			listOfCards.Add(name, c);
		}
		public void AddUnitCard(string name, int cost, Faction f, string uType, string mText, string flavor, int r, int m, int hp, List<Ability> a) {
			UnitCard c = new UnitCard(cost, name, f, uType, mText, flavor, r, m, hp, a.ToArray());
			Console.WriteLine(c.ToString());
			listOfCards.Add(name, c);
		}

		public Faction StringToFaction(string s)
		{
			switch(s)
			{
				case "Carthan":	return Faction.CARTHAN;
				case "Myxori":	return Faction.MYXORI;
				case "Jirnorn":	return Faction.JIRNORN;
				default:		return Faction.NONE;
			}
		}

		public CardLoader() {
			listOfCards = new Dictionary<string, Card>();
			old_listOfCards = new Dictionary<string, Card>();

			Console.WriteLine(typeof(FireSupportDrone));

			string[] paths = new string[] {
				Path.Combine("Assets", "Resources", "Cards"),
				Path.Combine("Assets", "Resources", "Cards", "Carthan"),
				Path.Combine("Assets", "Resources", "Cards", "Myxori"),
				Path.Combine("Assets", "Resources", "Cards", "Jirnorn")
			};
			foreach(string path in paths) {
				foreach(string fileName in Directory.EnumerateFiles(path, "*.yaml")) {
					Console.WriteLine($"~~~\nLoading file {fileName}.");

					YamlStream yaml = new YamlStream();
					yaml.Load(File.OpenText(fileName));

					YamlMappingNode root = yaml.Documents[0].RootNode as YamlMappingNode;

					string cardName = root.Children[new YamlScalarNode("name")].ToString();
					int cost = int.Parse(root.Children[new YamlScalarNode("cost")].ToString());
					Faction faction = StringToFaction(root.Children[new YamlScalarNode("faction")].ToString());
					string unitType = root.Children[new YamlScalarNode("unitType")].ToString();
					string mainText = root.Children[new YamlScalarNode("mainText")].ToString();
					string flavorText = root.Children[new YamlScalarNode("flavorText")].ToString();
					int r = int.Parse(root.Children[new YamlScalarNode("rangedDamage")].ToString());
					int m = int.Parse(root.Children[new YamlScalarNode("meleeDamage")].ToString());
					int hp = int.Parse(root.Children[new YamlScalarNode("hp")].ToString());

					if(root.Children.ContainsKey(new YamlScalarNode("abilities"))) {
						List<Ability> abilities = new List<Ability>();
						YamlSequenceNode abilitySequence = root.Children[new YamlScalarNode("abilities")] as YamlSequenceNode;
						foreach(YamlScalarNode abilityNode in abilitySequence) {
							abilities.Add(Ability.FromString(abilityNode.ToString()));
						}
						AddUnitCard(cardName, cost, faction, unitType, mainText, flavorText, r, m, hp, abilities);
					} else {
						AddUnitCard(cardName, cost, faction, unitType, mainText, flavorText, r, m, hp);
					}
				}
			}


			AddUnitCardOLD("Outergrowth Husk", 0, Faction.MYXORI, "Myxori Infantry Unit",
				"Absorb (excess damage done to this does not overflow)", "Myxi jha maRtox lOz!",
				0, 0, 1, new Absorb());
			AddUnitCardOLD("Scion Infiltrator 3rd Class", 1, Faction.JIRNORN, "Jirnorn Mutant Infantry Unit",
				"Lob (in combat, this deals damage to whatever is behind the opposing front line)", "Small, Innocuous and Hopeful",
				1, 1, 1, new Lob());
			AddUnitCardOLD("Grafted Claw Spunner", 1, Faction.MYXORI, "Myxi Infantry Unit",
				"Swarm (when you deploy this, if it is behind a Unit that shares a tag with it other than “Unit”, draw a card)", "\"Myxori jha maRtox lOz!\"",
				0, 2, 2, new Swarm());
			old_listOfCards.Add("Mercenary Phantasm", new UnitCard(2, "Mercenary Phantasm", Faction.NONE, "", "Deploy: Your opponent reveals a card from their hand", "What do you want to know?", 1, 2, 3));
			old_listOfCards.Add("Commercial Shipper", new UnitCard(2, "Commercial Shipper", Faction.NONE, "", "Deploy: Draw a card, then put a card from your hand on the bottom of your deck", "Whatever you need, we got", 0, 3, 3));
			old_listOfCards.Add("Adaptive Protopod", new UnitCard(2, "Adaptive Protopod", Faction.MYXORI, "", "Support Infantry: This front line gains Spore 3 until end of turn.\nSupport Artillery: This gains Siege 1 until end of turn",
				"MyxI jha maRtox lOz!", 2, 0, 2));
			AddUnitCardOLD("Screaming Spore Spunner", 1, Faction.MYXORI, "",
				"Spore 1 (when this dies, gain 1 )", "Muxi ji maRtox raj!",
				0, 2, 2, new Spore(1));
			old_listOfCards.Add("Pod-Cap Yxuu maRroz", new UnitCard(1, "Pod-Cap Yxuu maRroz", Faction.MYXORI, "", "Unique\nDeploy:Allied Infantry get +1M this turn.", "Far-Flung Mind-Spore Pod", 1, 0, 2));
			AddUnitCardOLD("Undergrowth Smasher", 5, Faction.MYXORI, "",
				"Siege 1\nDeploy: Deal 1 damage to the enemy in front of this for each Allied Myxori Unit", "Myxi jha maRtox lOz!",
				0, 4, 4, new Siege(1));
			AddUnitCardOLD("Aftergrowth Spunner", 2, Faction.MYXORI, "",
				"Spore  3 (when this dies, gain 3 s)", "toxhMar IOz Yzhmat rho!",
				0, 2, 2, new Spore(3));
			AddUnitCardOLD("Mulch Pod", 2, Faction.MYXORI, "",
				"Absorb\nDeath:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)", "Myxi jha maRtox IOz",
				0, 1, 1, new Absorb());
			old_listOfCards.Add("Fecund Reflex Axon", new UnitCard(2, "Fecund Reflex Axon", Faction.MYXORI, "", "Deploy...\nSupporting Myxori: Gain 3 s and this gets +3R this turn.\n" +
					"Front Line:If there is no back line here, this gains Absorb until the end of turn.", "Myxi jha maRtox IOz", 0, 2, 2));
			old_listOfCards.Add("Grafted Fang Spunner", new UnitCard(1, "Grafted Fang Spunner", Faction.MYXORI, "", "Death:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
					"Myxi jha maRtox IOz", 0, 2, 2));
			AddUnitCardOLD("Spore Pod", 1, Faction.MYXORI, "",
				"Spore 1 (when this dies, gain 1 )\nDeath:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
				"Myxi jha maRtox IOz",
				0, 0, 1, new Spore(1));
			old_listOfCards.Add("Horde Caller", new UnitCard(3, "Horde Caller", Faction.MYXORI, "", "Spore 2 (when this dies, gain 3 s)\nDeath: +1 Extra deploy", "MuxhRaz izt zraZxh",
					0, 3, 2, new Spore(2)));
			old_listOfCards.Add("Titanous Regrowth Trunk", new UnitCard(6, "Titanous Regrowth Trunk", Faction.MYXORI, "", "Persistent: Other allies gain Spore +1(when this dies, gain 1 )\n" +
			   "Front Line Persistent: Whenever an adjacent Font Line Infantry dies, Beta Swarm there", "Myxi jha maRtox lOz!", 0, 3, 7));
			old_listOfCards.Add("Exostellar Marine Squad", new UnitCard(3, "Exostellar Marine Squad", Faction.CARTHAN, "", "",
					"Bravely into the Darkness", 2, 2, 4));
			old_listOfCards.Add("Commercial Coms Relay", new UnitCard(0, "Commercial Coms Relay", Faction.NONE, "", "Deploy: Put a card from your hand beneath the top 4 cards of your deck.",
					"Binding the Stars", 0, 1, 2));
			//old_listOfCards.Add("Adv. Infantry Support System", new UnitCard(5, "Adv. Infantry Support System", Faction.CARTHAN, "", "Melee Shield 1\nPersistent: At the start of your turn, generate an extra .",
			//		"Mobile Command Center", 0, 3, 6, new MeleeShield(1), new BattleframeAssizeClass()));
			//old_listOfCards.Add("Paladin-Class XS Marines", new UnitCard(4, "Paladin-Class XS Marines", Faction.CARTHAN, "", "Front Line Recurring Deploy: If there is not a unit behind this, heal each adjacent allied front line 1.",
			//		"Defense Against the Darkness", 1, 3, 5, new PaladinClassExSMarines()));
			//old_listOfCards.Add("Adv. Supply Drone", new UnitCard(2, "Adv. Supply Drone", Faction.CARTHAN, "", "Supporting Carthan Deploy: Heal the supported unit 2 and gain 3 s.",
			//		"762491", 0, 1, 2, new AdvSupplyDrone()));
			//old_listOfCards.Add("XS Field Technician", new UnitCard(3, "XS Field Technician", Faction.CARTHAN, "", "Suporting Drone Deploy: Return the supported unit to your hand.",
			//		"Tuning Blades in the Darkness", 1, 2, 4, new XSFieldTechnician()));
			//old_listOfCards.Add("Lt. Mgr. Tul Yorves", new UnitCard(4, "Lt. Mgr. Tul Yorves", Faction.CARTHAN, "", "Unique; Ranged Shield 1\nFront Line Persistent: Whenever you heal a Unit, heal it for 1 more and it deals 1 damage.",
			//		"Master Medic", 1, 3, 5, new RangedShield(2)));
			//old_listOfCards.Add("Exostellar Snipers", new UnitCard(3, "Exostellar Snipers", Faction.CARTHAN, "", "",
			//		"Illuminate with Muzzle Flare!", 3, 1, 3));
			//old_listOfCards.Add("Cmdr. Yos Lorth", new UnitCard(4, "Cmdr. Yos Lorth", Faction.CARTHAN, "", "Unique; Ranged Shield 1\nFront Line Persistent: Allied front line Elites have +1 and Ranged Shield +1",
			//		"Exostellar Champion", 2, 2, 5, new RangedShield(1), new CmdrYosLorth()));
			//old_listOfCards.Add("Ancillary Medical Officer", new UnitCard(2, "Ancillary Medical Officer", Faction.CARTHAN, "", "Supporting Carthan Infantry Deploy: Heal this front line 2.",
			//		"Healer in the Darkness", 1, 2, 3, new AncillaryMedicalOfficer()));
			old_listOfCards.Add("Battle-Line Trauma Medic", new UnitCard(3, "Battle-Line Trauma Medic", Faction.CARTHAN, "", "Supporting Carthan Deploy: Discard a card; heal each ally 2.",
					"Forever Alive in the Dark!", 0, 2, 4));
			//old_listOfCards.Add("Emergency Med Drop", new UnitCard(0, "Emergency Med Drop", Faction.CARTHAN, "", "Deploy: Heal this front line 2 and each adjacent front line 1",
			//		"756328", 0, 0, 1, new EmergencyMedDrop()));
            old_listOfCards.Add("Cannoneer Drone", new UnitCard(2, "Cannoneer Drone", Faction.CARTHAN, "", "Ranged Shield 2; Melee Shield 2\nFront Line Persistent: The tower behind this deals +3 damge.",
                    "Death to Your Enemies", 0, 0, 1, new RangedShield(2), new MeleeShield(2)));
            //old_listOfCards.Add("Autonomous Range Finder", new UnitCard(2, "Autonomous Range Finder", Faction.CARTHAN, "", "Supporting Carthan Deploy: Give this front line +3R this turn.",
            //        "56413", 0, 1, 3, new AutonomousRangeFinder()));

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

        protected override Card handleMiss(string id) {
			if(listOfCards.ContainsKey(id))
				return listOfCards[id];

			if(old_listOfCards.ContainsKey(id)) {
				// Console.WriteLine($"Card ({id}) is hardcoded and not yet implemented in a .yaml file");
				return old_listOfCards[id];
			}

			throw new System.Exception("Card \"" + id + "\" not found");
        }

    }
    
}