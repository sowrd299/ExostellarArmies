using SFB.Game.Management;

namespace SFB.Game{

    // a class to represent a unit in play
    class Unit : IIDed {

        // while all the ID code is repeated, can't use a common ancestor class
        // if we want to have seporate instance of IdIssuer for different things that need id's
        private static IdIssuer<Unit> idIssuer = new IdIssuer<Unit>();

        private UnitCard card; //the card the unit is an instance of

        readonly public string id;
        public string ID{
            get{ return id; }
        }

        public Unit(UnitCard card){
            this.card = card;
            this.id = idIssuer.IssueId(this);
        }

    }

}