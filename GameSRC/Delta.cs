using System.Xml;

namespace Game{

    // a class to represent a change in the game state
    public class Delta{

        // for testing only
        public Delta(){

        }

        public Delta(XmlNode from){

        }

        public XmlElement ToXml(XmlDocument doc){
            XmlElement e = doc.CreateElement("delta");
            return e;
        }

    }

}