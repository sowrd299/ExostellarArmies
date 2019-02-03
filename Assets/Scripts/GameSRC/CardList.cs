using System.Collections.Generic;
using SFB.Game.Management;

namespace SFB.Game {

    // a parent class for decks and hands
    public abstract class CardList : List<Card>, IIDed {

        public abstract string ID{
            get;
        }

    }

}