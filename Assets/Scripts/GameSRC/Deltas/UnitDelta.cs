using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game {
	public class UnitDelta : Delta {
		private SendableTarget<Unit> sendableTarget;
		internal Unit Target {
			get { return sendableTarget.Target; }
		}

		private SendableTarget<Unit> sendableSource;
		internal Unit Source {
			get { return sendableSource.Target; }
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
			sendableTarget = new SendableTarget<Unit>("target", t);
			amount = a;
			dmgType = type;
			sendableSource = new SendableTarget<Unit>("source", s);
		}

		internal UnitDelta(XmlElement from, CardLoader cl) : base(from, cl) {
			sendableTarget = new SendableTarget<Unit>("target", from, Unit.idIssuer);
			sendableSource = new SendableTarget<Unit>("source", from, Unit.idIssuer);
			amount = Int32.Parse(from.Attributes["amount"].Value);
			dmgType = StringToDamageType(from.Attributes["type"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);

			r.SetAttributeNode(sendableTarget.ToXml(doc));

			r.SetAttributeNode(sendableSource.ToXml(doc));

			XmlAttribute amtAttr = doc.CreateAttribute("amount");
			amtAttr.Value = ""+amount;
			r.SetAttributeNode(amtAttr);

			XmlAttribute typeAttr = doc.CreateAttribute("type");
			typeAttr.Value = DamageTypeToString(dmgType);
			r.SetAttributeNode(typeAttr);

			return r;
		}

		internal override void Apply() {
			switch(dmgType) {
				case DamageType.RANGED:
					Target.takeRangedDamage(amount);
					break;
				case DamageType.MELEE:
					Target.takeMeleeDamage(amount);
					break;
				case DamageType.TOWER:
					Target.takeTowerDamage(amount);
					break;
				case DamageType.HEAL:
					Target.heal(amount);
					break;
			}
		}

		internal override void Revert() {
			if(dmgType == DamageType.HEAL) {
				Target.takeTowerDamage(amount);
			} else {
				int mod = (dmgType == DamageType.RANGED
								? Target.getTakeRangedDamageModifier()
								: (dmgType == DamageType.MELEE
									? Target.getTakeMeleeDamageModifier()
									: 0));
				Target.heal(amount >= mod ? amount - mod : 0);
			}
		}

		public enum DamageType {
			RANGED, MELEE, TOWER, HEAL
		}

		public String DamageTypeToString(DamageType type) {
			switch(type) {
				case DamageType.RANGED:
					return "R";
				case DamageType.MELEE:
					return "M";
				case DamageType.TOWER:
					return "T";
				case DamageType.HEAL:
					return "H";
				default:
					return "";
			}
		}

		public DamageType StringToDamageType(String type) {
			switch(type) {
				case "R":
					return DamageType.RANGED;
				case "M":
					return DamageType.MELEE;
				case "T":
					return DamageType.TOWER;
				case "H":
					return DamageType.HEAL;
				default:
					throw new Exception($"Type string \"{type}\" is invalid");
			}
		}
	}
}
