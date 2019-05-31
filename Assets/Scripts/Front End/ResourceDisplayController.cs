using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SFB.Game;
using SFB.Net.Client;

public class ResourceDisplayController : MonoBehaviour
{
	private Player player => Driver.instance.gameManager.Players[sideIndex];

	public GameObject[] pipContainers;

	public float pulseTime;
	public float zoomTime;
	public HandManager hand;
	[HideInInspector]
	public int sideIndex;

	private List<Image> pips = new List<Image>();

	private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();

	private IEnumerator Start()
	{
		foreach (GameObject container in pipContainers)
		{
			foreach (Transform child in container.transform)
			{
				pips.Add(child.GetComponent<Image>());
				child.transform.localScale = Vector3.zero;
			}
		}

		coroutineQueue.Enqueue(AnimateWaitForGame());
		coroutineQueue.Enqueue(AnimateMainLoop());

		while (true)
		{
			if (coroutineQueue.Count > 0)
			{
				yield return StartCoroutine(coroutineQueue.Dequeue());
			}
			else
			{
				yield return null;
			}
		}
	}

	private IEnumerator AnimateWaitForGame()
	{
		yield return new WaitUntil(() => Driver.instance.inGame);
	}

	private IEnumerator AnimateMainLoop()
	{
		int resourceCount = player.Mana;
		int deploymentCost = hand.deploymentCost;

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

		coroutineQueue.Enqueue(AnimateMainLoop());
	}

	public void UpdateResourceDisplay(Action callback)
	{
		coroutineQueue.Enqueue(AnimateUpdateResourceDisplay(callback));
	}

	private IEnumerator AnimateUpdateResourceDisplay(Action callback)
	{
		int resourceCount = player.Mana;

		yield return StartCoroutine(UIManager.ParallelCoroutine(
			pips.Select<Image, Func<Coroutine>>((pip, index) =>
			{
				Transform pipTransform = pip.transform;

				return () => UIManager.instance.LerpTime(
					Vector3.Lerp,
					pipTransform.localScale,
					index < resourceCount ? Vector3.one : Vector3.zero,
					zoomTime,
					scale => pipTransform.localScale = scale
				);
			}).ToArray()
		));

		callback();
	}
}
