using System.Xml;

namespace SFB.Game{

    // a class to represent a change in the game state
    public class Delta{

        public Delta(){

        }

        public Delta(XmlNode from){

        }

        public XmlElement ToXml(XmlDocument doc){
            XmlElement e = doc.CreateElement("delta");
            return e;
        }

        // returns wether or not the given player would see this change
        // return false, e.g., for a card being draw into an opponent's hand
        public bool VisibleTo(Player p){
            return true;
        }

    }

}