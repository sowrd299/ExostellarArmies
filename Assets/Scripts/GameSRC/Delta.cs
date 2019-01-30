using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace SFB.Game.Management{

    // a class to represent a change in the game state
    // the changes represented by a single delta should be small and atomic
    public abstract class Delta {

        // the string presentation of the type, for use in making XML's
        protected /*virtual*/ string type{
            get{
                return this.GetType().ToString();
            }
        }

        // non-Xml constructor: for use when originating a delta (server side)
        // every child class needs one
        public Delta(){

        }

        // Xml constructor: for use when getting an XML representatino based  on Xml for a network message (client side)
        // every child class needs one THAT TAKES EXACTLY ONE XML ELEMENT
        // THESE CONSTRUCTORS ARE PUBLIC FOR REFLECTION; THEY ARE NOT MEANT TO BE CALLED EXTERNALLY
        public Delta(XmlElement from){

        }

        public virtual XmlElement ToXml(XmlDocument doc){
            XmlElement e = doc.CreateElement("delta");
            // attach the type
            XmlAttribute typeAttr = doc.CreateAttribute("type");
            typeAttr.Value = type;
            e.SetAttributeNode(typeAttr);
            return e;
        }

        // this will return a new instance of the Delta type specified in the XML
        // and return it
        // DO THIS INSTEAD OF CALLING ANY SUBCLASSES XML CONSTRUCTOR
        public static Delta FromXml(XmlElement from){
            string t = from.Attributes["type"].Value;
            Type type = Type.GetType(t);
            ConstructorInfo con = type.GetConstructor(new Type[]{typeof(XmlElement)});
            return con?.Invoke(new object[]{from}) as Delta;
        }

        // returns wether or not the given player would see this change
        // return false, e.g., for a card being draw into an opponent's hand
        public virtual bool VisibleTo(Player p){
            return true;
        }

        // actually enacts the will of the delta
        internal abstract void Apply();

        // undoes what the apply does exactly
        // if cannot be undone, should through an error
        internal abstract void Revert();

    }


    // a class to represent changes in the gamestate to specific ID'ed objects
    public abstract class TargetedDelta<T> : Delta where T : IIDed {

        public T target; // the object the delta applies to
                  // TODO: I'm not convinced this shouldn't support N targets of different types
        public TargetedDelta(XmlElement from, IdIssuer<T> issuer) : base(from) {
            // returns the target of the action, if any
            XmlAttribute idAttr = from.Attributes["targetId"];
            if(idAttr != null){
				target = issuer.GetByID(idAttr.Value);// as T;
            }
        }

        public TargetedDelta(T target) : base() {
            this.target = target;
        }

        public override XmlElement ToXml(XmlDocument doc){
            XmlElement e = base.ToXml(doc);
            if(target != null){
                XmlAttribute a = doc.CreateAttribute("targetId");
                a.Value = target.ID;
                e.SetAttributeNode(a);
            }
            return e;
        }

    }

}