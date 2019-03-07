using SFB.Game.Content;
using SFB.Game;
using System.Collections.Generic;

// this is part of Server, and not Game.Deck, because it (in theory) needs contact to
// databases, account information, etc.
// also the client shouldn't have access to it
// at minimum it is not part of the pure abstract gamelogic
namespace SFB.Net.Server{
    
    class DeckListManager{

        // hardcode carthan starter deck
        private DeckList CarthStarter(){
            DeckList r = new DeckList();
            r.AddCard(new UnitCard(3, "Exostellar Marine Squade", Faction.CARTH, "",
                    "Bravely into the Darkness", 2, 2, 4, new AbilityList()),2);

            r.AddCard(new UnitCard(2, "Commercial Shipper", Faction.NONE, "Deploy: Draw a card, then put a card from your hand on the bottom of your deck.",
                    "What do you need? We got it.", 0, 3, 3, new AbilityList()), 1);

            r.AddCard(new UnitCard(0, "Commercial Coms Replay", Faction.NONE, "Deploy: Put a card from your hand beneath the top 4 cards of your deck.",
                    "Binding the Stars", 0,1,2, new AbilityList()),1);

            r.AddCard(new UnitCard(5, "Adv. Infantry Support System", Faction.CARTH, "Melee Shield 1\nPersistent: At the start of your turn, generate an extra resource.",
                    "Mobile Command Center", 0, 3, 6, new AbilityList()),2);

            r.AddCard(new UnitCard(4, "Paladin-Class XS Marnines", Faction.CARTH, "Front Line Recurring Deploy: If there is not a unit behind this, heal each adjacent allied front line 1.",
                    "Defense Against the Darkness", 1, 3, 5, new AbilityList()),1);

            r.AddCard(new UnitCard(2, "Adv. Supply Drone", Faction.CARTH, "Supporting Carthan Deploy: Heal the supported unit 2 and gain 3 resources.",
                    "762491", 0, 1, 2, new AbilityList()),2);

            r.AddCard(new UnitCard(3, "XS Field Technician", Faction.CARTH, "Suporting Drone Deploy: Return the supported unit to your hand.",
                    "Tuning Blades in the Darkness", 1, 2, 4, new AbilityList()), 1);

            r.AddCard(new UnitCard(4, "Mgr. Lt. Tul Yorves", Faction.CARTH, "Unique; Ranged Shield 1\nFront Line Persistent: Whenever you heal a Unit, heal it for 1 more and it deals 1 damage.",
                    "Master Medic", 1, 3, 5, new AbilityList()),1);

            r.AddCard(new UnitCard(3, "Exostellar Snipers", Faction.CARTH, "",
                    "Illuminate with Muzzle Flare!", 3,1,3, new AbilityList()),2);

            r.AddCard(new UnitCard(4, "Cmdr. Yos Lorth", Faction.CARTH, "Unique; Ranged Shield 1\nFront Line Persistent: Allied front line Elites have +1 and Ranged Shield +1",
                    "Exostellar Champion", 2, 2, 5, new AbilityList()),1);

            r.AddCard(new UnitCard(2, "Ancillary Medical Officer", Faction.CARTH, "Supporting Carthan Infantry Deploy: Heal this front line 2.",
                    "Healer in the Darkness", 1, 2, 3, new AbilityList()),1);

            r.AddCard(new UnitCard(3, "Battle-Line Trauma Medic", Faction.CARTH, "Supporting Carthan Deploy: Discard a card; heal each ally 2.",
                    "Forever Alive in the Dark!", 0, 2, 4, new AbilityList()),1);
            
            r.AddCard(new UnitCard(0, "Emergancy Med Drop", Faction.CARTH, "Deploy: Heal this front line 2 and each adjacent front line 1",
                    "756328", 0, 0, 1, new AbilityList()),1);
            
            r.AddCard(new UnitCard(2, "Cannoneer Drone", Faction.CARTH, "Ranged Shield 2; Melee Shield 2\nFront Line Persistent: The tower behind this deals +3 damge.",
                    "Death to Your Enemies", 0, 0, 1, new AbilityList()),1);

            r.AddCard(new UnitCard(2, "Autonomous Range Finder", Faction.CARTH, "Supporting Carthan Deploy: Give this front line +3R this turn.",
                    "56413", 0, 1, 3),2);
            return r;
        }

        // hardcoded Myxorit starter deck
        private DeckList MyxoriStarter()
        {
            DeckList deck = new DeckList();
            deck.AddCard(new UnitCard(2, "Mercenary Phatasm",Faction.NONE, "Deploy: Your opponent reveals a card from their hand", "What do you want to know?", 1,2,3, new AbilityList()), 1);
            deck.AddCard(new UnitCard(2, "Commercial Shipper",Faction.NONE, "Deploy: Draw a card, then put a card from your hand on the bottom of your deck", "Whatever you need, we got", 0,3,3, new AbilityList()), 1);
            deck.AddCard(new UnitCard(2, "Adaptive Protopod", Faction.MYXOR, "Support Infantry: This front line gains Spore 3 until end of turn.\nSupport Artillery: This gains Siege 1 until end of turn", 
               "MyxI jha maRtox lOz!", 2, 0, 2, new AbilityList()), 2);
            deck.AddCard(new UnitCard(1, "Screaming Spore Spunner", Faction.MYXOR, "Spore 1(when this dies, gain 1 resource)", "Muxi ji maRtox raj!", 0,2,2, new AbilityList()), 1);
            deck.AddCard(new UnitCard(1, "Pod-Cap Yxuu maRroz", Faction.MYXOR, "Unique\nDeploy:Allied Infantry get +1M this turn.", "Far-Flung Mind-Spore Pod",1,0,2, new AbilityList()), 1);
            deck.AddCard(new UnitCard(5, "Undergrowth Smasher", Faction.MYXOR, "Siege 1\nDeploy: Deal 1 damage to the enemy in front of this for each Allied Myxori Unit",
               "Myxi jha maRtox lOz!", 0, 4, 4, new AbilityList()), 1);
            deck.AddCard(new UnitCard(2, "Aftergrowth Spunner", Faction.MYXOR, "Spore  3(when this dies, gain 3 resources)", "toxhMar IOz Yzhmat rho!", 0, 2, 2, new AbilityList()), 1);
            deck.AddCard(new UnitCard(2, "Mulch Pod", Faction.MYXOR, "Absorb\nDeath:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
               "Myxi jha maRtox IOz", 0, 1, 1, new AbilityList()), 2);
            deck.AddCard(new UnitCard(2, "Fecund Reflex Axon", Faction.MYXOR, "Deploy...\nSupporting Myxori: Gain 3 Resources and this gets +3R this turn.\n" +
               "Front Line:If there is no back line here, this gains Absorb until the end of turn.", "Myxi jha maRtox IOz", 0, 2, 2, new AbilityList()), 2);
            deck.AddCard(new UnitCard(1, "Grafted Fang Spunner", Faction.MYXOR, "Death:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
               "Myxi jha maRtox IOz", 0,2,2, new AbilityList()), 2);
            deck.AddCard(new UnitCard(1, "Spore Pod", Faction.MYXOR, "Spore 1(when this dies, gain 1 resource)\nDeath:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
               "Myxi jha maRtox IOz", 0, 0, 1, new AbilityList()), 2);
            deck.AddCard(new UnitCard(3, "Horde Caller", Faction.MYXOR, "Spore 2(when this dies, gain 3 resources)\nDeath: +1 Extra deploy", "MuxhRaz izt zraZxh",0,3,2, new AbilityList()), 2);
            deck.AddCard(new UnitCard(6, "Titanous Regrowth Trunk", Faction.MYXOR, "Persistent: Other allies gain Spore +1(when this dies, gain 1 resource)\n" +
               "Front Line Persistent: Whenever an adjacent Font Line Infantry dies, Beta Swarm there", "Myxi jha maRtox lOz!", 0, 3, 7, new AbilityList()), 2);

            return deck;
        }

        // return a decklist object of the deck list with the
        // given ID
        // TODO: implement
        public DeckList LoadFromID(string id){
            //TESTING IMPLEMENTATION
            switch(id){
                case "testing":
                        DeckList r = new DeckList();
                        UnitCard xsMarine = new UnitCard(3, "Exostellar Marine Squade", Faction.CARTH, "", "Bravely into the Darkness", 2, 2, 4, new AbilityList());
                        r.AddCard(xsMarine, 20);
                        return r;
                case "carthStarter":
                        return CarthStarter();
                case "myxorStarter":
                        return MyxoriStarter();
                default:
                        return new DeckList();
            }
        }


    }

}