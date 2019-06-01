using UnityEngine;
using UnityEngine.UI;

public class DiscardIndicatorManager : MonoBehaviour
{
	public Text discardText;

	[HideInInspector]
	public int sideIndex;

	public void RenderDiscardCount()
	{
		discardText.text = Driver.instance.gameManager.Players[sideIndex].Discard.Count.ToString();
	}
}
