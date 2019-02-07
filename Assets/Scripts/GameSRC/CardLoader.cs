using SFB.Game.Management;

namespace SFB.Game.Content {

    public class CardLoader : IdIssuer<Card>{ // maybe this doesn't want to be an IdIssuerr... sure feels like it does though

        public new Card GetById(string id){
            return new UnitCard(3, id, Faction.CARTH, "Some rules text", "Some flavor text", 2, 2, 4);
        }

    }
    
}