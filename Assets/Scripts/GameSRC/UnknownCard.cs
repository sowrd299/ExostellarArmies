namespace SFB.Game{

    // for use whereever you need a card but you don't know which one
    // it will always evaluate true against another card
    //      (including but not limited to the unknown card it is standing in for)
    // this should be used to represent cards on a local copy of the game that the local player doesn't know about
    // e.g. cards in their deck or opponent's hand
    // TODO: this really should just a singleton to save memory
    public class UnknownCard : Card {

        public UnknownCard() : base(20, "Unkown", Faction.NONE, "You do not know what this card is.", "Now stop peaking.") {}


        // IMPLEMENTING EQUALITY
        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType().IsSubclassOf(typeof(Card));
        }

        // because of changes to card equality implementation, the bellow aren't actually needed anymore, but, eh
        public static bool operator == (UnknownCard a, Card b){
            return true;
        }

        public static bool operator == (Card b, UnknownCard a){
            return true;
        }
        public static bool operator != (UnknownCard a, Card b){
            return false;
        }

        public static bool operator != (Card b, UnknownCard a){
            return false;
        }


    }

}