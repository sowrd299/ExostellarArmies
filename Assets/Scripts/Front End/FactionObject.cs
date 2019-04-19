using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB.Game;

[CreateAssetMenu(menuName="Faction", fileName="New Faction")]
public class FactionObject : ScriptableObject
{
	public Faction faction;

	public Sprite unitFrame;

	public Sprite towerFrame;
}
