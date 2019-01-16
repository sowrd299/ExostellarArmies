using System.Xml;

namespace Game{

    // a class to represent a change in the game state
    public class Delta{

        public Delta(XmlNode from){

        }

        public XmlDocument ToXml(){
            XmlDocument r = new XmlDocument();
            XmlElement e = r.CreateElement("action");
            r.AppendChild(e);
            return r;
        }

    }

}