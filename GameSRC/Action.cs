using System.Xml;

namespace Game{

    // a class to represent a move taken by a player
    public class Action{
        public Action(XmlNode from){

        }

        // gets an XML representation of the move
        public XmlDocument ToXml(){
            XmlDocument r = new XmlDocument();
            XmlElement e = r.CreateElement("action");
            r.AppendChild(e);
            return r;
        }
    }
}