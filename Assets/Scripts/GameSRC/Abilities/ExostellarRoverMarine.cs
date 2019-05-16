using SFB.Game.Management;
using SFB.Game.Content;
using System;

namespace SFB.Game
{
	// Front Line: If this has a Back Line, this has +2M, else this has +1R.

	public class ExostellarRoverMarine : PersistentFieldAbility
	{
		protected override Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			Unit target = lanes[lane].Units[side, pos];
			if(pos == 0) {
				Delta[] deltas = new Delta[1];
				if(lanes[lane].Units[side, 1] == null) {
					deltas[0] = new UnitDamageAmountDelta(target, 1, Damage.Type.RANGED, source);
				} else {
					deltas[0] = new UnitDamageAmountDelta(target, 2, Damage.Type.MELEE, source);
				}
				return deltas;
			} else {
				return new Delta[] {};
			}
		}

		protected override Delta[] GetRemoveDeltas(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			Unit target = lanes[lane].Units[side, pos];
			return new Delta[]
			{
				// TODO Front Line: If this has a Back Line, this has +2M, else this has +1R.
			};
		}

		protected override bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			int? sourceSide = Lane.GetLaneSidePosOf(source, lanes)?.Item2;
			if(sourceSide == null)
				throw new Exception($"Source Unit \"{source.ID}\" not found");
			return false; // TODO Front Line: If this has a Back Line, this has +2M, else this has +1R.
		}
	}
}
