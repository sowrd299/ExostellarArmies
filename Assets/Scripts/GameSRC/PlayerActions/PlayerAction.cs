using System.Xml;
using SFB.Game.Content;

namespace SFB.Game.Management{

    // a class to represent a move taken by a player
    // does NOT represent which player is attempting to take that action
    public abstract class PlayerAction : Sendable{

        protected override string XmlNodeName{
            get{ return "action"; }
        }

        public PlayerAction(){

        }

        public PlayerAction(XmlElement from){

        }

        public static PlayerAction FromXml(XmlElement e, IdIssuer<Card> cl, IdIssuer<Lane> lanes){
            object[] args = new object[]{e, cl, lanes};
            PlayerAction pa = SendableFactory<PlayerAction>.FromXml(e, args);
            return pa;
        }

        // returns if it is legal in the current gamestate...
        // takes the player specifically trying to do the action
        // this is allows us to discociate 
        internal abstract bool IsLegalAction(Player p);

        // returns the changes to the current gamestate the changes would enact
        // takes the player doing the action
        internal abstract Delta[] GetDeltas(Player p, GameState gameState);

    }
}