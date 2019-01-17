namespace SFB.Game{

    // the factions cards might belong to
    public enum Faction {NONE, CARTH, MYXOR}

    // the supertype of all cards that might exist in the game
    // this is more for good practice's sake, since all cards ATM (start of project) are Units
    // card classes represent the different cards available in the game, not different copies of the same card
    abstract class Card{


        // the cost to deploy this card, in resources
        private int deployCost;
        public int DeployCost{
            get{ return cost; }
        }

        // the displayed name of the card
        private string name;
        public int Name{
            get { return name; }
        }

        // what faction the card belongs to
        private Faction faction;
        public Faction Faction{
            get{ return faction; }
        }

        // TODO: should probably add in other things, like flavor text, main test, etc.

        public Card(int deployCost, string name, Faction faction){
            this.deployCost = deployCost;
            this.name = name;
            this.faction = faction;
        }

    }

}