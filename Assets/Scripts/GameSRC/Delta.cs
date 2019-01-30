using System.Xml;

namespace SFB.Game.Management{

    // a class to represent a change in the game state
    public abstract class Delta {


        public Delta(){

        }

        public Delta(XmlNode from){

        }

        public virtual XmlElement ToXml(XmlDocument doc){
            XmlElement e = doc.CreateElement("delta");
            return e;
        }

        // returns wether or not the given player would see this change
        // return false, e.g., for a card being draw into an opponent's hand
        public virtual bool VisibleTo(Player p){
            return true;
        }

        // actually enacts the will of the delta
        internal abstract void Apply();

    }


    // a class to represent changes in the gamestate to specific ID'ed objects
    public abstract class TargetedDelta<T> : Delta where T : IIDed {

        public T target; // the object the delta applies to
                  // TODO: I'm not convinced this shouldn't support N targets of different types
        public TargetedDelta(XmlNode from, IdIssuer<T> issuer) : base(from) {
            // returns the target of the action, if any
            XmlAttribute idAttr = from.Attributes["targetId"];
            if(idAttr != null){
				target = issuer.GetByID(idAttr.Value);// as T;
            }
        }

        public override XmlElement ToXml(XmlDocument doc){
            XmlElement e = base.ToXml(doc);
            if(target != null){
                XmlAttribute a = doc.CreateAttribute("targetId");
                a.Value = target.ID;
            }
            return e;
        }

    }

}