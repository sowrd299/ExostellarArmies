using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game;
using SFB.Net.Client;

public class ResourceDisplayController : MonoBehaviour
{
	private Player player => Driver.instance.gameManager.Players[Driver.instance.sideIndex];

	public GameObject[] pipContainers;

	public float pulseTime;
	public HandManager hand;

	private IEnumerator Start()
	{
		List<Image> pips = new List<Image>();
		foreach (GameObject container in pipContainers)
		{
			foreach (Transform child in container.transform)
			{
				pips.Add(child.GetComponent<Image>());
			}
		}

		while (true)
		{
			yield return new WaitUntil(() => Driver.instance.inGame);

			int resourceCount = player.Mana;
			int deploymentCost = hand.deploymentCost;

			for (int i = 0; i < Mathf.Min(resourceCount - deploymentCost, pips.Count); i++)
			{
				pips[i].CrossFadeAlpha(1, 0, true);
			}
			for (int i = Mathf.Max(resourceCount, 0); i < pips.Count; i++)
			{
				pips[i].CrossFadeAlpha(0, 0, true);
			}

			for (int i = Mathf.Max(resourceCount - deploymentCost, 0); i < Mathf.Min(resourceCount, pips.Count); i++)
			{
				pips[i].CrossFadeAlpha(0, pulseTime / 2, true);
			}

			yield return new WaitForSeconds(pulseTime / 2);

			for (int i = Mathf.Max(resourceCount - deploymentCost, 0); i < Mathf.Min(resourceCount, pips.Count); i++)
			{
				pips[i].CrossFadeAlpha(1, pulseTime / 2, true);
			}

			yield return new WaitForSeconds(pulseTime / 2);
		}
	}
}
