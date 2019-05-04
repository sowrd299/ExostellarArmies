using System.Xml;
using System.Reflection;
using System;
using SFB.Game.Content;

namespace SFB.Game.Management {

    // for when an impossible delta is applied or reverted
    public class IllegalDeltaException : Exception {
        public IllegalDeltaException(){}
        public IllegalDeltaException(string message) : base(message){}
        public IllegalDeltaException(string message, Exception inner): base(message, inner){}
    }

    // a class to represent a change in the game state
    // the changes represented by a single delta should be small and atomic
    public abstract class Delta {

        // I am assuming here enough different deltas will want this
        // that it is worth just sharing the code
        protected SendableTarget<Card> SendableCard;
        public Card Card {
            get{ return SendableCard.Target; }
        }

        // the string presentation of the type, for use in making XML's
        protected /*virtual*/ string type {
            get{
				return this.GetType().ToString();
            }
        }

        // non-Xml constructor: for use when originating a delta (server side)
        // every child class needs one
        public Delta(){
            SendableCard = new SendableTarget<Card>("card", null);
        }

        // an alternate non-Xml constructor
        public Delta(Card c){
            SendableCard = new SendableTarget<Card>("card", c);
        }

        // Xml constructor: for use when getting an XML representatino based  on Xml for a network message (client side)
        // every child class needs one THAT TAKES EXACTLY ONE XML ELEMENT AND A CARD LOADER
        // THESE CONSTRUCTORS ARE PUBLIC FOR REFLECTION; THEY ARE NOT MEANT TO BE CALLED EXTERNALLY
        public Delta(XmlElement from, CardLoader cardLoader){
            SendableCard = new SendableTarget<Card>("card", from, cardLoader);
        }

        public virtual XmlElement ToXml(XmlDocument doc){
            XmlElement e = doc.CreateElement("delta");
            // attach the type
            XmlAttribute typeAttr = doc.CreateAttribute("type");
            typeAttr.Value = type;
            e.SetAttributeNode(typeAttr);
            // attach the card
            e.SetAttributeNode(SendableCard.ToXml(doc));
            return e;
        }

        // this will return a new instance of the Delta type specified in the XML
        // and return it
        // DO THIS INSTEAD OF CALLING ANY SUBCLASSES XML CONSTRUCTOR
        public static Delta FromXml(XmlElement from, CardLoader cl){
            string t = from.Attributes["type"].Value;
            Type type = Type.GetType(t);
            if(type.IsSubclassOf(typeof(Delta))){ //refuse to construct types that aren't Deltas; do this for safety
                ConstructorInfo con = type.GetConstructor(new Type[]{typeof(XmlElement), typeof(CardLoader)});
                return con?.Invoke(new object[]{from, cl}) as Delta;
            }else{
                return null;
            }
        }

        // returns wether or not the given player would see this change
        // return false, e.g., for a card being draw into an opponent's hand
        public virtual bool VisibleTo(Player p){
            return true;
        }

        // actually enacts the will of the delta
        internal abstract void Apply();

        // undoes what the apply does exactly
        // if cannot be undone, should throw an error
        internal abstract void Revert();

    }
}