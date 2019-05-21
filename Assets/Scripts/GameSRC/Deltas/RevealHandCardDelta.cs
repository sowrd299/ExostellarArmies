using System.Xml;
using System;
using SFB.Game.Management;
using SFB.Game.Content;

namespace SFB.Game
{
	public abstract class RevealHandCardDelta : Delta
	{
        private int Index;

        public RevealHandCardDelta(CardList list, int i)
        {
			Index = i;
        }

        public RevealHandCardDelta(XmlElement from, CardLoader cl)
        {
            this.Index = Int32.Parse(from.Attributes["index"].Value);
        }

        public override XmlElement ToXml(XmlDocument doc)
		{
            XmlElement r = base.ToXml(doc);

            XmlAttribute indexAttr = doc.CreateAttribute("index");
            indexAttr.Value = Index.ToString();
            r.SetAttributeNode(indexAttr);

            return r;
        }
        
        internal override void Apply() {}
        internal override void Revert() {}
    }
}