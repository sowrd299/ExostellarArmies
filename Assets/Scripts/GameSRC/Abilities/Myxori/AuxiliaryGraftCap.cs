using SFB.Game.Management;
using SFB.Game.Content;
using System;

namespace SFB.Game
{
	public class AuxiliaryGraftCap : PersistentFieldUnitAbility
	{
		public override string GetMainText() {
			return "Supporting Myxori Persistent...\nCenter: The supported unit gains Absorb.\nFlank: When the supported Unit dies, Beta Swarm.";
		}

		protected override Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source, GameManager gm)
		{
			Unit target = lanes[lane].Units[side, pos];
			return new Delta[]
			{
				// TODO Supporting Myxori Persistent…Center: The supported unit gains Absorb. Flank: When the supported Unit dies, Beta Swarm.
			};
		}

		protected override Delta[] GetRemoveDeltas(Unit target, Unit source, GameManager gm)
		{
			return new Delta[]
			{
				// TODO Supporting Myxori Persistent…Center: The supported unit gains Absorb. Flank: When the supported Unit dies, Beta Swarm.
			};
		}

		protected override bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			int? sourceSide = Lane.GetLaneSidePosOf(source, lanes)?.Item2;
			if(sourceSide == null)
				throw new Exception($"Source Unit \"{source.ID}\" not found");
			return false; // TODO Supporting Myxori Persistent…Center: The supported unit gains Absorb. Flank: When the supported Unit dies, Beta Swarm.
		}
	}
}
