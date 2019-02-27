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

    }

}