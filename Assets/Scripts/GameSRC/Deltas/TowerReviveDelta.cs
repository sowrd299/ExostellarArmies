using SFB.Game.Content;
using SFB.Game.Management;
using System.Xml;

namespace SFB.Game
{
	public class TowerReviveDelta : TargetedDelta<Tower> {
		public TowerReviveDelta(Tower t)
			: base(t)
		{}

		public TowerReviveDelta(XmlElement from, CardLoader loader)
			: base(from, Tower.IdIssuer, loader)
		{}

		internal override void Apply()
		{
			Target.Revive();
		}

		internal override void Revert()
		{
			throw new IllegalDeltaException("Tried to revert tower death");
		}
	}
}