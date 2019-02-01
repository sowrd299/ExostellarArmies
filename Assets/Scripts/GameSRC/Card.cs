namespace SFB.Game{

    // the factions cards might belong to
    public enum Faction {NONE, CARTH, MYXOR, JIRNOR}

    // the supertype of all cards that might exist in the game
    // this is more for good practice's sake, since all cards ATM (start of project) are Units
    // this repesents cards in the game, in decks and in hands
    // multiple ref's to the same card obj should be able to be used as different copies of the same card
    // given that a card in the deck, hand, etc. is stateless
    public abstract class Card {


        // the cost to deploy this card, in resources
        private int deployCost;
        public int DeployCost{
            get{ return deployCost; }
        }

        // the displayed name of the card
        private string name;
        public string Name{
            get { return name; }
        }

        // what faction the card belongs to
        private Faction faction;
        public Faction Faction{
            get{ return faction; }
        }

		private string mainText;
		public string MainText {
			get { return mainText; }
		}
		
		private string flavorText;
		public string FlavorText {
			get { return flavorText; }
		}

		public Card(int deployCost, string name, Faction faction, string mainText, string flavorText){
            this.deployCost = deployCost;
            this.name = name;
            this.faction = faction;
			this.mainText = mainText;
			this.flavorText = flavorText;
        }

		override public string ToString() {
			return name;
		}

    }

}