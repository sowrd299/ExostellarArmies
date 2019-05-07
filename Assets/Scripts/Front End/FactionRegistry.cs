using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SFB.Game;

[CreateAssetMenu(fileName = "Faction Registry", menuName = "Faction Registry")]
public class FactionRegistry : ScriptableObject
{
	[Serializable]
	public class FactionData
	{
		public Faction faction;

		public Sprite cardBackground;

		public Sprite unitFrame;

		public Sprite towerFrame;
	}

	public FactionData[] factions;

	private Dictionary<Faction, FactionData> _factionRegistry;
	private Dictionary<Faction, FactionData> factionRegistry =>
		_factionRegistry ?? (_factionRegistry = factions.ToDictionary(
			factionObject => factionObject.faction,
			factionObject => factionObject
		));

	public FactionData this[Faction faction]
	{
		get { return this.factionRegistry[faction]; }
	}
}
