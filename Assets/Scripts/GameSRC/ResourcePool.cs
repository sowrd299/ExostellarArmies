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
            return x <= count;
        }

        public void Add(int x){
            count += x;
        }

        public void Subtract(int x)
        {
            count -= x;
        }

        public Management.Delta[] GetAddDeltas(int x){
            int xp = x; // x prime
            if(xp + count > max){ // do not go above max
                xp = max - count;
            }else if(count + xp < 0){ // do not go bellow 0
                xp = - count;
            }
            return new Management.Delta[]{new ResourcePoolDelta(xp, this)};
        }

		public Management.Delta[] GetSubtractDeltas(int x) {
			int xp = -x; // x prime
			if(xp + count > max) { // do not go above max
				xp = max - count;
			} else if(count + xp < 0) { // do not go bellow 0
				xp = -count;
			}
			return new Management.Delta[] { new ResourcePoolDelta(xp, this) };
		}
    }

}