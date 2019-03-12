using System.Xml;

namespace SFB.Game.Management{

    // a class to request input needed from a player
    // usually, this mean a card's ability gives the player a choice
    public abstract class InputRequest<T> : Sendable, IIDed where T : IIDed{

        public static new string XmlNodeName{
            get{ return "inputRequest"; }
        }

        protected static IdIssuer<InputRequest<T>> idIssuer = new IdIssuer<InputRequest<T>>();

        private string id;
        public string ID{
            get{ return id; }
        }

        private Player player; // the player who needs to make the 
        public Player Player{
            get{ return player; }
        }

        // TODO: add in some conception of what are the legal choices?

        SendableTarget<T> chosen;  // the choice the player made

        // returns whether or not the player has made their choice
        public bool Made{
            get{ return chosen.Target != null; }
        }

        public InputRequest(Player player, string id = null){
            chosen = new SendableTarget<T>("chosen", default(T));
            this.player = player;
            // ID it
            if(id != null){
                this.id = id;
                idIssuer.RegisterId(id, this);
            }else{
                this.id = idIssuer.IssueId(this);
            }
        }

        public InputRequest(Player player, XmlElement e, IdIssuer<T> targetIdIssuer) {
            this.player = player;
            // get the choice, if there is one
            chosen = new SendableTarget<T>("chosen", e, targetIdIssuer);
            // find in there is already an Input Request with the same ID
            string id = e.Attributes["id"].Value;
            InputRequest<T> sharedId = idIssuer.GetByID(id);
            // ...if there is, just migrate data to that one
            if(sharedId != null && !object.Equals(chosen.Target, default(T))){
                sharedId.MakeChoice(chosen.Target);
            }else{
                idIssuer.RegisterId(id, this);
            }
        }

        // TODO: Implement "FromXml"
        // public new void 
        
        // make the choice
        public void MakeChoice(T chosen){
            this.chosen = new SendableTarget<T>("chosen", chosen);
        }

        public abstract Delta[] getDeltas(); // get the deltas after the choice has been made

    }

}