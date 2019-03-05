using SFB.Game.Content;
using SFB.Game;

// this is part of Server, and not Game.Deck, because it (in theory) needs contact to
// databases, account information, etc.
// also the client shouldn't have access to it
// at minimum it is not part of the pure abstract gamelogic
namespace SFB.Net.Server{
    
    class DeckListManager{

        // return a decklist object of the deck list with the
        // given ID
        // TODO: implement
        public DeckList LoadFromID(string id){
            DeckList r = new DeckList();
            //TESTING IMPLEMENTATION
            if(id == "testing"){
                UnitCard xsMarine = new UnitCard(3, "Exostellar Marine Squade", Faction.CARTH, "", "Bravely into the Darkness", 2, 2, 4, new AbilityList());
                r.AddCard(xsMarine, 20);
            }
            return r;
        }

        private MyxoriStarter()
        {
            DeckListManager Mdeck = new DeckList();
            Mdeck.AddCard(new UnitCard(2, "Mercenary Phatasm",Faction.NONE, "Deploy: Your opponent reveals a card from their hand", "What do you want to know?", 1,2,3, new AbilityList()), 1);
            Mdeck.AddCard(new UnitCard(2, "Commercial Shipper",Faction.NONE, "Deploy: Draw a card, then put a card from your hand on the bottom of your deck", "Whatever you need, we got", 0,3,3, new AbilityList()), 1);
            Mdeck.AddCard(new UnitCard(2, "Adaptive Protopod", Faction.MYXOR, "Support Infantry: This front line gains Spore 3 until end of turn.\nSupport Artillery: This gains Siege 1 until end of turn", 
                "MyxI jha maRtox lOz!", 2, 0, 2, new AbilityList()), 2);
            Mdeck.AddCard(new UnitCard(1, "Screaming Spore Spunner", Faction.MYXOR, "Spore 1(when this dies, gain 1 resource)", "Muxi ji maRtox raj!", 0,2,2, new AbilityList()), 1);
            Mdeck.AddCard(new UnitCard(1, "Pod-Cap Yxuu maRroz", Faction.MYXOR, "Unique\nDeploy:Allied Infantry get +1M this turn.", "Far-Flung Mind-Spore Pod",1,0,2, new AbilityList()), 1);
            Mdeck.AddCard(new UnitCard(5, "Undergrowth Smasher", Faction.MYXOR, "Siege 1\nDeploy: Deal 1 damage to the enemy in front of this for each Allied Myxori Unit",
                "Myxi jha maRtox lOz!", 0, 4, 4, new AbilityList()), 1);
            Mdeck.AddCard(new UnitCard(2, "Aftergrowth Spunner", Faction.MYXOR, "Spore  3(when this dies, gain 3 resources)", "toxhMar IOz Yzhmat rho!", 0, 2, 2, new AbilityList()), 1);
            Mdeck.AddCard(new UnitCard(2, "Mulch Pod", Faction.MYXOR, "Absorb\nDeath:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
                "Myxi jha maRtox IOz", 0, 1, 1, new AbilityList()), 2);
            Mdeck.AddCard(new UnitCard(2, "Fecund Reflex Axon", Faction.MYXOR, "Deploy...\nSupporting Myxori: Gain 3 Resources and this gets +3R this turn.\n" +
                "Front Line:If there is no back line here, this gains Absorb until the end of turn.", "Myxi jha maRtox IOz", 0, 2, 2, new AbilityList()), 2);
            Mdeck.AddCard(new UnitCard(1, "Grafted Fang Spunner", Faction.MYXOR, "Death:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
                "Myxi jha maRtox IOz", 0,2,2, new AbilityList()), 2);
            Mdeck.AddCard(new UnitCard(1, "Spore Pod", Faction.MYXOR, "Spore 1(when this dies, gain 1 resource)\nDeath:Beta Swarm (if there is nothing in this back row, reveal the top card of your deck and play it at -1 Cost. If you can't, put it in your graveyard)",
                "Myxi jha maRtox IOz", 0, 0, 1, new AbilityList()), 2);
            Mdeck.AddCard(new UnitCard(3, "Horde Caller", Faction.MYXOR, "Spore 2(when this dies, gain 3 resources)\nDeath: +1 Extra deploy", "MuxhRaz izt zraZxh",0,3,2, new AbilityList()), 2);
            Mdeck.AddCard(new UnitCard(6, "Titanous Regrowth Trunk", Faction.MYXOR, "Persistent: Other allies gain Spore +1(when this dies, gain 1 resource)\n" +
                "Front Line Persistent: Whenever an adjacent Font Line Infantry dies, Beta Swarm there", "Myxi jha maRtox lOz!", 0, 3, 7, new AbilityList()), 2);

            return Mdeck;
        }

    }

}