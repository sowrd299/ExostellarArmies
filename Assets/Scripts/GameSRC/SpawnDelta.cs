using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;

namespace SFB.Game {

    // a class for deltas that involve spawning ID's objects
    // must constrain T to class so know is nullable
    public abstract class SpawnDelta<T> : Delta where T : class, IIDed {

        private IdIssuer<T> issuer;
        protected string id;

        public SpawnDelta(IdIssuer<T> issuer) {
            this.issuer = issuer;
            id = issuer.IssueId(null as T);
        }

        public SpawnDelta(XmlElement e, IdIssuer<T> issuer, CardLoader cl)
                :base(e, cl)
        {
            this.issuer = issuer;
            this.id = e.Attributes["id"].Value;
        }

        public override XmlElement ToXml(XmlDocument doc){
            XmlElement r = base.ToXml(doc);
            XmlAttribute idAttr = doc.CreateAttribute("id");
            idAttr.Value = id;
            r.SetAttributeNode(idAttr);
            return r; 
        }

        // returns a new object to the specified type, with the spawner delta's ID
        protected abstract T spawn();

        // MUST REMEMBER TO REGISTER THE OBJECT IN THE APPLY IMPLEMENTATION

    }

}