﻿using SFB.Game.Management;
using SFB.Game.Content;
using System;

namespace SFB.Game
{
	public class CmdrYosLorth : PersistentFieldUnitAbility
	{
		// Unique; Ranged Shield 1; Front Line Persistent: Allied front line Elites have +1 and Ranged Shield +1

		protected override Delta[] GetAddDeltas(int lane, int side, int pos, Lane[] lanes, Unit source, GameManager gm)
		{
			Unit target = lanes[lane].Units[side, pos];
			return new Delta[]
			{
				new UnitDamageAmountDelta(target, 1, Damage.Type.MELEE, source),
				new UnitAbilityDelta(target, source, new RangedShield(1), UnitAbilityDelta.DeltaMode.ADD, gm)
			};
		}

		protected override Delta[] GetRemoveDeltas(Unit target, Unit source, GameManager gm)
		{
			return new Delta[]
			{
				new UnitDamageAmountDelta(target, -1, Damage.Type.MELEE, source),
				new UnitAbilityDelta(target, source, new RangedShield(1), UnitAbilityDelta.DeltaMode.REMOVE, gm)
			};
		}

		protected override bool ApplyTo(int lane, int side, int pos, Lane[] lanes, Unit source)
		{
			int? sourceSide = Lane.GetLaneSidePosOf(source, lanes)?.Item2;
			if(sourceSide == null)
				throw new Exception($"Source Unit \"{source.ID}\" not found");
			return pos == 0 &&
				lanes[lane].Units[side, pos].Card.UnitType.Contains("Elite") &&
				sourceSide == side &&
				lanes[lane].Units[side, pos] != source;
		}
	}
}
