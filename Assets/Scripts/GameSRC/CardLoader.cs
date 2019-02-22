using SFB.Game.Management;

namespace SFB.Game.Content {

    public class CardLoader : IdIssuer<Card>{ // maybe this doesn't want to be an IdIssuerr... sure feels like it does though

        protected override Card handleMiss(string id){
            // TODO: dummy implmentation
            return new UnitCard(3, id, Faction.CARTH, "Some rules text", "Some flavor text", 2, 2, 4, new AbilityList());
        }

    }
    
}