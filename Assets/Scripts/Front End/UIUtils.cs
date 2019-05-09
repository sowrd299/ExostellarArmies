using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtils
{
	public static IEnumerator ParallelCoroutine(params Coroutine[] coroutines)
	{
		foreach (Coroutine coroutine in coroutines)
		{
			yield return coroutine;
		}
	}
}
