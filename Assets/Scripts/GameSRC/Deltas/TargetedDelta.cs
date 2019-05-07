using SFB.Game.Content;
using System.Xml;

namespace SFB.Game.Management {
	// a class to represent changes in the gamestate to specific ID'ed objects
	public abstract class TargetedDelta<T> : Delta where T : IIDed {
		private SendableTarget<T> _target; // the object the delta applies to
		public T Target {
			get { return _target.Target; }
			set { _target = new SendableTarget<T>("targetId", value); }
		}
		public TargetedDelta(XmlElement from, IdIssuer<T> issuer, CardLoader loader)
				: base(from, loader) {
			_target = new SendableTarget<T>("targetId", from, issuer);
		}

		public TargetedDelta(T target) : base() {
			this.Target = target;
		}

		public override XmlElement ToXml(XmlDocument doc) {
			XmlElement e = base.ToXml(doc);
			if(Target != null) {
				e.SetAttributeNode(_target.ToXml(doc));
			}
			return e;
		}
	}
}