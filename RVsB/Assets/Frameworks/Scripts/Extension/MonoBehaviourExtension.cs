using UnityEngine;
using System.Collections;

public static class MonoBehaviourExtension {
	public static IEnumerator DelayedCoroutine(this MonoBehaviour _monoBehaviour, FuncCallback0 callback, float delayBefore = -1)
	{
		if (delayBefore > 0) 
		{
			yield return new WaitForSeconds (delayBefore);
		}
		else if (delayBefore == 0) 
		{
			yield return new WaitForEndOfFrame ();
		}

		if (callback != null) {
			callback ();
		}
	}

	public static void StartCoroutineDelay(this MonoBehaviour _monoBehaviour, FuncCallback0 fn0, float delay = -1)
	{
		if(delay<0)
		{
			fn0 ();
		}
		else
		{
			_monoBehaviour.StartCoroutine (_monoBehaviour.DelayedCoroutine (fn0, delay));
		}
	}
}
