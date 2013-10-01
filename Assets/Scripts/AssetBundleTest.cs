using UnityEngine;
using System.Collections;
using System;

public class AssetBundleTest : MonoBehaviour{

    public Func<IEnumerator> testFunc;
    public Func<IEnumerator> initFunc;
    public Func<IEnumerator> cleanupFunc;
	public int numberOfIterationsToPerform;

    public AssetBundleStats stats;

    public IEnumerator Perform()
    {
        if(initFunc != null)
            yield return StartCoroutine(initFunc());
        stats = GetComponent<AssetBundleStats>();
        stats.Begin();
        if (testFunc != null)
            yield return StartCoroutine(testFunc());
        stats.End();
        if (cleanupFunc != null)
            yield return StartCoroutine(cleanupFunc());
    }
}
