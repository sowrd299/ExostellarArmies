using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;

namespace SFB.Game {
	public class UnitDelta : Delta {
		private Unit target;
		internal Unit Target {
			get { return target; }
		}

		private Unit source;
		internal Unit Source {
			get { return source; }
		}

		private int amount;
		public int Amount {
			get { return amount; }
		}

		private DamageType dmgType;
		public DamageType DmgType {
			get { return dmgType; }
		}

		internal UnitDelta(Unit t, int a, DamageType type, Unit s) {
			target = t;
			amount = a;
			dmgType = type;
			source = s;
		}

		public UnitDelta(XmlElement from, CardLoader cl) : base(from, cl) {
			this.target = Int32.Parse(from.Attributes["index"].Value);
			this.mode = from.Attributes["mode"].Value == "add" ? Mode.ADD : Mode.REMOVE; // use remove as default b/c more likely to error if chosen eroniously
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);
			XmlAttribute targetAttr = doc.CreateAttribute("target");
			targetAttr.Value = target.ToString();
			r.SetAttributeNode(targetAttr);
			XmlAttribute sourceAttr = doc.CreateAttribute("source");
			sourceAttr.Value = source.ToString();
			r.SetAttributeNode(sourceAttr);
			XmlAttribute amtAttr = doc.CreateAttribute("amount");
			amtAttr.Value = ""+amount;
			r.SetAttributeNode(amtAttr);
			XmlAttribute typeAttr = doc.CreateAttribute("type");
			typeAttr.Value = dmgType.ToString();
			r.SetAttributeNode(typeAttr);
			return r;
		}

		internal override void Apply() {
			switch(dmgType) {
				case DamageType.RANGED:
					target.takeRangedDamage(amount);
					break;
				case DamageType.MELEE:
					target.takeMeleeDamage(amount);
					break;
				case DamageType.TOWER:
					target.takeTowerDamage(amount);
					break;
				case DamageType.HEAL:
					target.heal(amount);
					break;
			}
		}

		internal override void Revert() {
			if(dmgType == DamageType.HEAL) {
				target.takeTowerDamage(amount);
			} else {
				int mod = (dmgType == DamageType.RANGED
								? target.getTakeRangedDamageModifier()
								: (dmgType == DamageType.MELEE
									? target.getTakeMeleeDamageModifier()
									: 0));
				target.heal(amount >= mod ? amount - mod : 0);
			}
		}

		public enum DamageType {
			RANGED, MELEE, TOWER, HEAL
		}
	}
}
