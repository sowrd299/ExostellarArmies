using SFB.Game.Management;
using SFB.Game.Content;
using System.Xml;
using System;

namespace SFB.Game
{
	public abstract class UnitDelta : Delta
	{
		private SendableTarget<Unit> sendableTarget;
		internal Unit Target {
			get { return sendableTarget.Target; }
		}

		private SendableTarget<Unit> sendableSource;
		internal Unit Source {
			get { return sendableSource.Target; }
		}

		public UnitDelta(Unit t, Unit s)
		{
			sendableTarget = new SendableTarget<Unit>("target", t);
			sendableSource = new SendableTarget<Unit>("source", s);
		}

		public UnitDelta(XmlElement from, CardLoader cl)
			: base(from, cl)
		{
			sendableTarget = new SendableTarget<Unit>("target", from, Unit.IdIssuer);
			sendableSource = new SendableTarget<Unit>("source", from, Unit.IdIssuer);
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement r = base.ToXml(doc);

			r.SetAttributeNode(sendableTarget.ToXml(doc));
			r.SetAttributeNode(sendableSource.ToXml(doc));

			return r;
		}

		protected abstract void ApplyEffects(Unit u);

		internal override void Apply()
		{
			ApplyEffects(Target);
		}

		internal override void Revert()
		{
			throw new IllegalDeltaException("Don't revert a UnitDelta");
		}
	}
}
