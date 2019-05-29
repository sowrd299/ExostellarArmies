using SFB.Game.Content;
using SFB.Game;
using System.Collections.Generic;
using System;

// this is part of Server, and not Game.Deck, because it (in theory) needs contact to
// databases, account information, etc.
// also the client shouldn't have access to it
// at minimum it is not part of the pure abstract gamelogic
namespace SFB.Net.Server{
    
    public class DeckListManager {
        CardLoader newCardLoader = new CardLoader();

		public DeckListManager() {
			Console.WriteLine("wow");
		}


        // hardcode carthan starter deck
        private DeckList CarthStarter(){
            DeckList r = new DeckList();
            r.AddCard(newCardLoader.GetByID("Exostellar Marine Squad"), 2);
            
            r.AddCard(newCardLoader.GetByID("Commercial Shipper"), 1); // no system

            r.AddCard(newCardLoader.GetByID("Commercial Coms Relay"),1); // no system

            r.AddCard(newCardLoader.GetByID("Adv. Infantry Support System"),2);

            r.AddCard(newCardLoader.GetByID("Paladin-Class XS Marines"),1);

            r.AddCard(newCardLoader.GetByID("Adv. Supply Drone"),2);

            r.AddCard(newCardLoader.GetByID("XS Field Technician"), 1);

            r.AddCard(newCardLoader.GetByID("Lt. Mgr. Tul Yorves"),1); // no system yet

            r.AddCard(newCardLoader.GetByID("Exostellar Snipers"),2);

            r.AddCard(newCardLoader.GetByID("Cmdr. Yos Lorth"),1);

            r.AddCard(newCardLoader.GetByID("Ancillary Medical Officer"),1);

            r.AddCard(newCardLoader.GetByID("Battle-Line Trauma Medic"),1); // not yet
            
            r.AddCard(newCardLoader.GetByID("Emergency Med Drop"),1);
            
            r.AddCard(newCardLoader.GetByID("Cannoneer Drone"),1); // not yet

            r.AddCard(newCardLoader.GetByID("Autonomous Range Finder"), 2);
            return r;
        }

        // hardcoded Myxorit starter deck
        private DeckList MyxoriStarter()
        {
            DeckList deck = new DeckList();

            deck.AddCard(newCardLoader.GetByID("Mercenary Phantasm"), 1); // no system
            deck.AddCard(newCardLoader.GetByID("Commercial Shipper"), 1); // no system
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
            Console.WriteLine("Loading deck: {0}...", id);
			if(id.StartsWith("TEST ")) {
				DeckList d = new DeckList();
				d.AddCard(newCardLoader.GetByID(id.Substring(5)), 8);
				d.AddCard(newCardLoader.GetByID("Resist Token"), 6);
				d.AddCard(newCardLoader.GetByID("Mana Token"), 6);
				Console.WriteLine("ok");
				return d;
			} else {
				switch(id) {
					case "carthStarter":
						return CarthStarter();
					case "myxoriStarter":
						return MyxoriStarter();
					case "tokensAtk":
						DeckList a = new DeckList();
						a.AddCard(newCardLoader.GetByID("Attack Token 1"), 4);
						a.AddCard(newCardLoader.GetByID("Attack Token 2"), 4);
						a.AddCard(newCardLoader.GetByID("Attack Token 3"), 4);
						a.AddCard(newCardLoader.GetByID("Attack Token 4"), 4);
						a.AddCard(newCardLoader.GetByID("Attack Token 5"), 4);
						return a;
					case "testing":
					default:
						DeckList d = new DeckList();
						d.AddCard(newCardLoader.GetByID("Resist Token"), 20);
						return d;
				}
			}
        }
    }
}