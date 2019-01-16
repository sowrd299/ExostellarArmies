using System.Xml;

namespace Game{

    // a class to represent a move taken by a player
    public class Action{
        public Action(XmlNode from){

        }

        // gets an XML representation of the move
        public XmlElement ToXml(XmlDocument doc){
            XmlElement e = doc.CreateElement("action");
            return e;
        }
    }
}