using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
	public CanvasGroup currentCanvas;
	public float transitionTime;

	private Coroutine currentTransition;

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

	public void Quit()
	{
		Application.Quit();
	}
}
