using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using SFB.Net.Client;

public class MainMenuController : MonoBehaviour
{
	[Header("UI References")]
	public CanvasGroup currentCanvas;
	public Text ipInputHelper;
	public Button joinMatch;

	[Header("Config")]
	public float transitionTime;
	public int defaultPort = 4011;

	public string ipKey;
	public string deckKey;

	private Coroutine currentTransition;
	private Client client => Client.instance;
	private CancellationTokenSource cancelConnect;
	private object cancelConnectLock = new object();
	private bool connected;

	public void TransitionTo(CanvasGroup newCanvas)
	{
		if (currentTransition != null)
		{
			StopCoroutine(currentTransition);
		}

		currentTransition = StartCoroutine(AnimateTransitionTo(newCanvas));
	}

	private IEnumerator AnimateTransitionTo(CanvasGroup newCanvas)
	{
		float startTime = Time.time;
		while (Time.time - startTime < transitionTime / 2)
		{
			currentCanvas.alpha = 1 - (Time.time - startTime) / (transitionTime / 2);
			yield return null;
		}
		currentCanvas.alpha = 0;
		currentCanvas.gameObject.SetActive(false);

		currentCanvas = newCanvas;
		currentCanvas.gameObject.SetActive(true);

		startTime = Time.time;
		while (Time.time - startTime < transitionTime / 2)
		{
			currentCanvas.alpha = (Time.time - startTime) / (transitionTime / 2);
			yield return null;
		}
		currentCanvas.alpha = 1;
	}

	public void OnIPInputChange(string ip)
	{
		if (ip == "" || ip.Contains(":"))
		{
			ipInputHelper.text = "";
			ipInputHelper.color = Color.white;
		}
		else
		{
			ipInputHelper.text = $"Using default port {defaultPort}";
			ipInputHelper.color = Color.white;
		}

		joinMatch.interactable = false;
	}

	public void OnIPEnter(string ip)
	{
		if (ip == "") return;
		StartCoroutine(AnimateOnIPEnter(ip));
	}

	private IEnumerator AnimateOnIPEnter(string ip)
	{
		string host;
		int port;

		if (ip.Contains(":"))
		{
			string[] parts = ip.Split(new[] { ':' }, 2);
			try
			{
				host = parts[0];
				port = int.Parse(parts[1]);
			}
			catch (Exception)
			{
				ipInputHelper.text = $"Failed to connect to {ip}\nError parsing IP input";
				ipInputHelper.color = Color.red;
				yield break;
			}
		}
		else
		{
			host = ip;
			port = defaultPort;
		}

		lock (cancelConnectLock)
		{
			if (cancelConnect != null)
			{
				cancelConnect.Cancel();
			}
			cancelConnect = new CancellationTokenSource();
		}

		ipInputHelper.text = $"Connecting to {host}:{port}...";
		ipInputHelper.color = Color.white;

		Task<bool> connectTask = client.Connect(host, port, maxAttempts: 3, cancelToken: cancelConnect.Token);
		yield return new WaitUntil(() => connectTask.IsCompleted);

		if (connectTask.IsCanceled) yield break;

		lock (cancelConnectLock)
		{
			cancelConnect = null;
		}

		if (connectTask.Result)
		{
			ipInputHelper.text = $"Connected to {host}:{port}!";
			ipInputHelper.color = Color.green;
			joinMatch.interactable = true;
		}
		else
		{
			ipInputHelper.text = $"Failed to connect to {host}:{port}";
			ipInputHelper.color = Color.red;
			joinMatch.interactable = false;
		}
	}

	public void Quit()
	{
		Application.Quit();
	}
}
