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

		private Damage.Type dmgType;
		public Damage.Type DmgType {
			get { return dmgType; }
		}

		public UnitDelta(Unit t, int a, Damage.Type type, Unit s) {
			sendableTarget = new SendableTarget<Unit>("target", t);
			amount = a;
			dmgType = type;
			sendableSource = new SendableTarget<Unit>("source", s);
		}

		public UnitDelta(XmlElement from, CardLoader cl) : base(from, cl) {
			sendableTarget = new SendableTarget<Unit>("target", from, Unit.idIssuer);
			sendableSource = new SendableTarget<Unit>("source", from, Unit.idIssuer);
			amount = Int32.Parse(from.Attributes["amount"].Value);
			dmgType = Damage.StringToDamageType(from.Attributes["dmgType"].Value);
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);

			r.SetAttributeNode(sendableTarget.ToXml(doc));

			r.SetAttributeNode(sendableSource.ToXml(doc));

			XmlAttribute amtAttr = doc.CreateAttribute("amount");
			amtAttr.Value = ""+amount;
			r.SetAttributeNode(amtAttr);

			XmlAttribute typeAttr = doc.CreateAttribute("dmgType");
			typeAttr.Value = Damage.DamageTypeToString(dmgType);
			r.SetAttributeNode(typeAttr);

			return r;
		}

		internal override void Apply() {
			switch(dmgType) {
				case Damage.Type.RANGED:
					Target.takeRangedDamage(amount);
					break;
				case Damage.Type.MELEE:
					Target.takeMeleeDamage(amount);
					break;
				case Damage.Type.TOWER:
					Target.takeTowerDamage(amount);
					break;
				case Damage.Type.HEAL:
					Target.heal(amount);
					break;
			}
		}

		internal override void Revert() {
			if(dmgType == Damage.Type.HEAL) {
				Target.takeTowerDamage(amount);
			} else {
				int mod = (dmgType == Damage.Type.RANGED
								? Target.getTakeRangedDamageModifier()
								: (dmgType == Damage.Type.MELEE
									? Target.getTakeMeleeDamageModifier()
									: 0));
				Target.heal(amount >= mod ? amount - mod : 0);
			}
		}
	}
}
