﻿using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game {
	public class UnitAbilityDelta : UnitDelta
	{
		public enum DeltaMode { ADD, REMOVE }

		public Ability Ability { get; private set; }
		public DeltaMode Mode { get; private set; }

		public UnitAbilityDelta(Unit target, Unit source, Ability a, DeltaMode m)
			: base(target, source)
		{
			Ability = a;
			Mode = m;
		}

		public UnitAbilityDelta(XmlElement from, CardLoader cl)
			: base(from, cl)
		{
			Ability = Ability.FromXml(from);
			Mode = (from.Attributes["mode"].Value == "add" ? DeltaMode.ADD : DeltaMode.REMOVE);
		}

		public override XmlElement ToXml(XmlDocument doc)
		{
			XmlElement r = base.ToXml(doc);

			XmlAttribute aTypeAttr = doc.CreateAttribute("abilityType");
			aTypeAttr.Value = "" + Ability.ToString();
			r.SetAttributeNode(aTypeAttr);

			XmlAttribute aAmtAttr = doc.CreateAttribute("abilityAmount");
			aAmtAttr.Value = "" + Ability.Amount;
			r.SetAttributeNode(aAmtAttr);

			XmlAttribute modeAttr = doc.CreateAttribute("mode");
			modeAttr.Value = (Mode == DeltaMode.ADD ? "add" : "remove");
			r.SetAttributeNode(modeAttr);

			return r;
		}

		protected override void ApplyEffects(Unit u)
		{
			if(Mode == DeltaMode.ADD)
				Ability.ApplyTo(u, null);
			else
				Ability.RemoveFrom(u, null);
		}
	}
}