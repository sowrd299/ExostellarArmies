using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System.Text;
using System;

namespace SFB.Game{

    public class ResourcePool : IIDed {

        private static IdIssuer<ResourcePool> idIssuer = new IdIssuer<ResourcePool>();
        public static IdIssuer<ResourcePool> IdIssuer{
            get{ return idIssuer; }
        }

        private int count; // the current number of resources
        public int Count{
            get{ return count; }
        }

        private int max; // the most resources the pool can have

        private string id;
        public string ID{
            get{ return id; }
        }

        public ResourcePool(int max, string id = ""){
            this.max = max;
            this.count = 0;
            if(id == ""){
                this.id = idIssuer.IssueId(this);
            }else{
                this.id = id;
                idIssuer.RegisterId(id, this);
            }
        }

        public bool CanAfford(int x){
            return x >= count;
        }

        public void Add(int x){
            count += x;
        }

        public Delta[] GetAddDeltas(int x){
            int xp = x; // x prime
            if(xp + count > max){ // do not go above max
                xp = max - count;
            }else if(count + xp < 0){ // do not go bellow 0
                xp = - count;
            }
            return new Delta[]{new ResourcePoolDelta(xp, this)};
        }


        private class ResourcePoolDelta : Delta {

            private int amount;
            private SendableTarget<ResourcePool> rp;

            public ResourcePoolDelta(int amount, ResourcePool rp){
                this.amount = amount;
                this.rp = new SendableTarget<ResourcePool>("poolId", rp);
            }

            public ResourcePoolDelta(XmlElement e, CardLoader cl)
                    : base(e, cl) 
            {
                this.rp = new SendableTarget<ResourcePool>("poolId", e, ResourcePool.IdIssuer); 
                this.amount = Int32.Parse(e.Attributes["amount"].Value);
            }

            public override XmlElement ToXml(XmlDocument doc){
                XmlElement r = base.ToXml(doc);
                r.SetAttributeNode(rp.ToXml(doc));
                XmlAttribute amount = doc.CreateAttribute("amount");
                amount.Value = this.amount.ToString();
                r.SetAttributeNode(amount);
                return r;
            }

            internal override void Apply(){
                rp.Target.Add(amount);
            }

            internal override void Revert(){
                rp.Target.Add(-amount);
            }

        }


    }

}