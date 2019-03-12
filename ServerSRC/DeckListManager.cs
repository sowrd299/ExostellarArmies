using SFB.Game.Content;
using SFB.Game;
using System.Collections.Generic;

// this is part of Server, and not Game.Deck, because it (in theory) needs contact to
// databases, account information, etc.
// also the client shouldn't have access to it
// at minimum it is not part of the pure abstract gamelogic
namespace SFB.Net.Server{
    
    class DeckListManager{
        CardLoader newCardLoader = new CardLoader();


        // hardcode carthan starter deck
        private DeckList CarthStarter(){
            DeckList r = new DeckList();
            r.AddCard(newCardLoader.GetByID( "Exostellar Marine Squad"), 2);

            r.AddCard(newCardLoader.GetByID("Commercial Shipper"), 1);

            r.AddCard(newCardLoader.GetByID("Commercial Coms Relay"),1);

            r.AddCard(newCardLoader.GetByID("Adv. Infantry Support System"),2);

            r.AddCard(newCardLoader.GetByID("Paladin-Class XS Marnines"),1);

            r.AddCard(newCardLoader.GetByID("Adv. Supply Drone"),2);

            r.AddCard(newCardLoader.GetByID("XS Field Technician"), 1);

            r.AddCard(newCardLoader.GetByID("Lt. Mgr Tul Yorves"),1);

            r.AddCard(newCardLoader.GetByID("Exostellar Snipers"),2);

            r.AddCard(newCardLoader.GetByID(["Cmdr Yos Lorth"),1);

            r.AddCard(newCardLoader.GetByID("Ancillary Medical OFficer"),1);

            r.AddCard(newCardLoader.GetByID("Pattle-Line Trauma Medic"),1);
            
            r.AddCard(newCardLoader.GetByID("Emergency Med Drop"),1);
            
            r.AddCard(newCardLoader.GetByID("Cannoneer Drone"),1);

            r.AddCard(new UnitCard(2, "Autonomous Range Finder", Faction.CARTH, "Supporting Carthan Deploy: Give this front line +3R this turn.",
                    "56413", 0, 1, 3, new AbilityList()),2);
            return r;
        }

        // hardcoded Myxorit starter deck
        private DeckList MyxoriStarter()
        {
            DeckList deck = new DeckList();

            deck.AddCard(newCardLoader.GetByID("Mercenary Phantasm"), 1);
            deck.AddCard(newCardLoader.GetByID("Commercial Shipper"), 1);
            deck.AddCard(newCardLoader.GetByID("Adaptive Protopod"), 2);
            deck.AddCard(newCardLoader.GetByID("Screaming Spore Spunner"), 1);
            deck.AddCard(newCardLoader.GetByID("Pod-Cap Yxuu maRroz"), 1);
            deck.AddCard(newCardLoader.GetByID("Undergrowth Smasher"), 1);
            deck.AddCard(newCardLoader.GetByID("Aftergrowth Spunner"), 1);
            deck.AddCard(newCardLoader.GetByID("Mulch Pod"), 2);
            deck.AddCard(newCardLoader.GetByID("Fecund Reflex Axon"), 2);
            deck.AddCard(newCardLoader.GetByID("Grafted Fang Spunner"), 2);
            deck.AddCard(newCardLoader.GetByID("Spore Pod"), 2);
            deck.AddCard(newCardLoader.GetByID("Horde Caller"), 2);
            deck.AddCard(newCardLoader.GetByID("Titanous Regrowth Trunk"), 2);

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