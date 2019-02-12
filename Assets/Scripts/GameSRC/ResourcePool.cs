using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;

namespace SFB.Game{

    public class ResourcePool : IIDed {

        private static IdIssuer<ResourcePool> idIssuer = new IdIssuer<ResourcePool>();
        public static IdIssuer<ResourcePool> IdIssuer{
            get{ return idIssuer; }
        }

        private int count; // the current number of resources
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
            return x > count;
        }

        public void Add(int x){
            count += x;
        }

        public Delta[] GetAddDeltas(int x){
            return new Delta[]{new ResourcePoolDelta(x, this)};
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
                this.rp = new SendableTarget<ResourcePool>("poolId", e, null); 
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