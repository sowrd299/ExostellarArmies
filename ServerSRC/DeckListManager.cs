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
            r.AddCard(newCardLoader.GetById( "Exostellar Marine Squad"), 2);

            r.AddCard(newCardLoader.GetById("Commercial Shipper"), 1);

            r.AddCard(newCardLoader.GetById("Commercial Coms Relay"),1);

            r.AddCard(newCardLoader.GetById("Adv. Infantry Support System"),2);

            r.AddCard(newCardLoader.GetById("Paladin-Class XS Marnines"),1);

            r.AddCard(newCardLoader.GetById("Adv. Supply Drone"),2);

            r.AddCard(newCardLoader.GetById("XS Field Technician"), 1);

            r.AddCard(newCardLoader.GetById("Lt. Mgr Tul Yorves"),1);

            r.AddCard(newCardLoader.GetById("Exostellar Snipers"),2);

            r.AddCard(newCardLoader.GetById(["Cmdr Yos Lorth"),1);

            r.AddCard(newCardLoader.GetById("Ancillary Medical OFficer"),1);

            r.AddCard(newCardLoader.GetById("Pattle-Line Trauma Medic"),1);
            
            r.AddCard(newCardLoader.GetById("Emergency Med Drop"),1);
            
            r.AddCard(newCardLoader.GetById("Cannoneer Drone"),1);

            r.AddCard(new UnitCard(2, "Autonomous Range Finder", Faction.CARTH, "Supporting Carthan Deploy: Give this front line +3R this turn.",
                    "56413", 0, 1, 3, new AbilityList()),2);
            return r;
        }

        // hardcoded Myxorit starter deck
        private DeckList MyxoriStarter()
        {
            DeckList deck = new DeckList();

            deck.AddCard(newCardLoader.GetById("Mercenary Phantasm"), 1);
            deck.AddCard(newCardLoader.GetById("Commercial Shipper"), 1);
            deck.AddCard(newCardLoader.GetById("Adaptive Protopod"), 2);
            deck.AddCard(newCardLoader.GetById("Screaming Spore Spunner"), 1);
            deck.AddCard(newCardLoader.GetById("Pod-Cap Yxuu maRroz"), 1);
            deck.AddCard(newCardLoader.GetById("Undergrowth Smasher"), 1);
            deck.AddCard(newCardLoader.GetById("Aftergrowth Spunner"), 1);
            deck.AddCard(newCardLoader.GetById("Mulch Pod"), 2);
            deck.AddCard(newCardLoader.GetById("Fecund Reflex Axon"), 2);
            deck.AddCard(newCardLoader.GetById("Grafted Fang Spunner"), 2);
            deck.AddCard(newCardLoader.GetById("Spore Pod"), 2);
            deck.AddCard(newCardLoader.GetById("Horde Caller"), 2);
            deck.AddCard(newCardLoader.GetById("Titanous Regrowth Trunk"), 2);

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