using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextManager : MonoBehaviour
{
	public float duration;
	public float moveDistance;
	public AnimationCurve moveCurve;
	public AnimationCurve opacityCurve;

	public GameObject damageTextPrefab;

	public Coroutine DamageTextPopup(Vector3 position, string text)
	{
		return StartCoroutine(AnimateDamageTextPopup(position, text));
	}

	private IEnumerator AnimateDamageTextPopup(Vector3 position, string text)
	{
		GameObject damageTextObject = Instantiate(damageTextPrefab, position, Quaternion.identity, transform);
		Text damageText = damageTextObject.GetComponent<Text>();
		damageText.text = text;

		Vector3 startPostion = position;
		Vector3 endPosition = startPostion + Vector3.up * moveDistance;

		float startTime = Time.time;
		while (Time.time - startTime < duration)
		{
			float deltaTime = Time.time - startTime;
			damageTextObject.transform.position = Vector3.Lerp(
				startPostion, endPosition, moveCurve.Evaluate(deltaTime / duration)
			);
			ChangeColor(damageText, color => color.a = opacityCurve.Evaluate(deltaTime / duration));
			yield return null;
		}

		Destroy(damageTextObject);
	}

	private void ChangeColor(Graphic graphic, Action<Color> callback)
	{
		Color color = graphic.color;
		callback(color);
		graphic.color = color;
	}
}
