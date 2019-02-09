using System.Xml;

namespace SFB.Game.Management{

    // a superclass for a target of a action or delta
    // that can be converted to and from Xml
    public class SendableTarget<T> where T : IIDed {

        // the name used for representing this in XML
        private string name;

        // the object being targeted
        private T target;
        public T Target{
            get { return target; }
        }

        public SendableTarget(string name, T target){
            this.name = name;
            this.target = target;
        }

        public SendableTarget(string name, XmlElement from, IdIssuer<T> issuer){
            this.name = name;
            if(from.Attributes[name] != null && from.Attributes[name].Value != ""){
                this.target = issuer.GetByID(from.Attributes[name].Value);
            }
        }

        // returns the target represented as an XmlAttribute
        public XmlAttribute ToXml(XmlDocument doc){
            XmlAttribute attr = doc.CreateAttribute(name);
            if(target != null){
                attr.Value = target.ID;
            }else{
                attr.Value = "";
            }
            return attr;
        }

    }

}