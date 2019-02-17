using System.Xml;

namespace SFB.Game.Management{

    // a class to represent a move taken by a player
    // does NOT represent which player is attempting to take that action
    public abstract class PlayerAction{

        public PlayerAction(){

        }

        public PlayerAction(XmlNode from){

        }

        // gets an XML representation of the move
        public XmlElement ToXml(XmlDocument doc){
            XmlElement e = doc.CreateElement("action");
            return e;
        }

        public static PlayerAction FromXml(XmlElement e){
            // TODO: generalize implementation from Delta
            // dummy implmenation
            return null;
        }

        // returns if it is legal in the current gamestate...
        // takes the player specifically trying to do the action
        // this is allows us to discociate 
        internal abstract bool IsLegalAction(Player p);

        // returns the changes to the current gamestate the changes would enact
        // takes the player doing the action
        internal abstract Delta[] GetDeltas(Player p);

    }
}