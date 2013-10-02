﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleTestFunctions: MonoBehaviour {
	public static AssetBundleTestFunctions Instance { get; private set; }
	
    void Awake() {
    	SingletonSetup();            
    }
	
	void SingletonSetup() {
		if (Instance != null) {
			Debug.LogWarning("Attempted to instantiate second instance of singleton AssetBundleTestManager.");
	        Destroy(gameObject);
	        return;
	    }
		Instance = this;
	}
	
    public IEnumerator IterateCoroutine(Func<IEnumerator> coroutine, int n)
    {
        for (int i = 0; i < n; i++)
        {
            yield return StartCoroutine(coroutine());
        }
    }
	
	public IEnumerator UnloadBundle()
    {
        AssetBundleManager.Instance.UnloadBundle();
        yield return null;
    }

    public IEnumerator LoadBundleFromFile(string bundleName)
    {
        var assetBundleManager = AssetBundleManager.Instance;
        string assetBundlePath = BenchmarkUtilities.GetBundlePath(bundleName);
        yield return StartCoroutine(assetBundleManager.LoadBundleFromFileUncompressed(assetBundlePath));
    }

    public IEnumerator LoadBundleFromFileAsync(string bundleName)
    {
        var assetBundleManager = AssetBundleManager.Instance;
        string assetBundlePath = BenchmarkUtilities.GetBundlePath(bundleName);
        yield return StartCoroutine(assetBundleManager.LoadBundleFromFileAsync(assetBundlePath));
    }

    public IEnumerator LoadBundleFromFileWWW(string bundleName)
    {
        var assetBundleManager = AssetBundleManager.Instance;
        string assetBundlePath = BenchmarkUtilities.GetBundlePath(bundleName);
        yield return StartCoroutine(assetBundleManager.LoadBundleFromWWW(assetBundlePath));
    }

    public IEnumerator LoadAssetsFromBundle(string bundleName)
    {
        var assetBundleManager = AssetBundleManager.Instance;
        string assetBundlePath = BenchmarkUtilities.GetBundlePath(bundleName);
        //Debug.Log(bundleName);
        //Debug.Log(GetAssetNames(assetBundlePath)[0]);
        yield return StartCoroutine(
            assetBundleManager.LoadAssetsFromBundle(assetBundleManager.bundle, BenchmarkUtilities.GetAssetNames(assetBundlePath)));
    }

    public IEnumerator LoadAssetsFromBundleAsync(string bundleName)
    {
        var assetBundleManager = AssetBundleManager.Instance;
        string assetBundlePath = BenchmarkUtilities.GetBundlePath(bundleName);
        yield return StartCoroutine(
            assetBundleManager.LoadAssetsFromBundleAsync(assetBundleManager.bundle, BenchmarkUtilities.GetAssetNames(assetBundlePath)));
    }

    public IEnumerator LoadAssetsFromBundleMultiLoad(string bundleName, int batchSize)
    {
        var assetBundleManager = AssetBundleManager.Instance;
        string assetBundlePath = BenchmarkUtilities.GetBundlePath(bundleName);
        yield return StartCoroutine(
            assetBundleManager.LoadAssetsFromBundleSimultaneouslyAsync(
            assetBundleManager.bundle, BenchmarkUtilities.GetAssetNames(assetBundlePath), batchSize));
    }
}
