namespace SFB.Game.Content {

    public class CardLoader {

        public Card LoadCard(string id){
            // TODO: this is a placeholder implementation
            // stats: exostellar marine
            return new UnitCard(3, id, Faction.CARTH, "Some rules text", "Some flavor text", 2, 2, 4);
        }

    }
    
}