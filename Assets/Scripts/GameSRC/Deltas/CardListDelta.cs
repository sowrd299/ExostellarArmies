using System.Xml;
using System;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game {
// not in management, because more likely to be needed by gamelogic
// management will just treat is as a generic Delta

    // a class to represent removing the given card from the given index the give card from the given index
    public abstract class CardListDelta<T> : TargetedDelta<T> where T : CardList {

        public enum DeltaMode { ADD, REMOVE }

        private int Index;
        private DeltaMode Mode; // WHETHER TO ADD OR REMOVE THE CARD IN QUESITON

        public CardListDelta(T list, Card c, int i, DeltaMode m)
            : base(list)
        {
            SendableCard = new SendableTarget<Card>("card", c); // TODO: shouldn't have to know what to name this...
            Index = i;
            Mode = m;
        }

        public CardListDelta(XmlElement from, IdIssuer<T> issuer, CardLoader cl)
			: base(from, issuer, cl)
        {
            this.Index = Int32.Parse(from.Attributes["index"].Value);
            this.Mode = from.Attributes["mode"].Value == "add" ? DeltaMode.ADD : DeltaMode.REMOVE; // use remove as default b/c more likely to error if chosen eroniously
        }

        public override XmlElement ToXml(XmlDocument doc)
		{
            XmlElement r = base.ToXml(doc);

            XmlAttribute indexAttr = doc.CreateAttribute("index");
            indexAttr.Value = Index.ToString();
            r.SetAttributeNode(indexAttr); 

            XmlAttribute modeAttr = doc.CreateAttribute("mode");
            modeAttr.Value = Mode == DeltaMode.ADD ? "add" : "remove";
            r.SetAttributeNode(modeAttr); 

            return r;
        }
        

        protected void AddCard()
		{
            if(Target.Count >= Index) {
                Target.Insert(Index, SendableCard.Target);
            } else {
                throw new IllegalDeltaException("The deck does not contain the index you wish to insert at");
            }
        }

        protected void RemoveCard()
		{
            if(Target.Count > Index && Target[Index] == SendableCard.Target) {
                Target.RemoveAt(Index);
            } else {
                throw new IllegalDeltaException("The card you wish to remove does not exist at that index");
            }
        }

        internal override void Apply(){
            if(Mode == DeltaMode.ADD) {
                AddCard();
            } else {
                RemoveCard();
            }
        }

        internal override void Revert(){
            if(Mode == DeltaMode.ADD) {
                RemoveCard();
            } else {
                AddCard();
            }
        }
    }
}