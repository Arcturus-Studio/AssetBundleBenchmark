using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class BundleLoadingManager : BundleTestManager {
    public static readonly int batchSize = 3;
	public static string bundleDirectoryPath;
	
	protected override void InitializeDirectoryPath() {
		bundleDirectoryPath = Application.streamingAssetsPath + "/";	
	}
	
    protected override IEnumerable<TestFuncInfo> GetTestFunctions()
    {
        var assetBundles = GetBundleNames();
        var bundleLoadTestInfo = new List<TestFuncInfo>();
        int colIndex = 0;
        foreach (var bundleName in assetBundles)
        {
            var bundle = bundleName;
            int rowIndex = 0;
            bundleLoadTestInfo.Add(BundleLoadTest("Loading bundle: " + bundle + " from file", () => LoadBundleFromFile(bundle), rowIndex++, colIndex));
            //bundleLoadTestInfo.Add(BundleLoadTest("Loading bundle: " + bundle + " from file async", () => LoadBundleFromFileAsync(bundle), rowIndex++, colIndex));
            //bundleLoadTestInfo.Add(BundleLoadTest("Loading bundle: " + bundle + " from file WWW", () => LoadBundleFromFileWWW(bundle), rowIndex++, colIndex));
            bundleLoadTestInfo.Add(AssetLoadTest("Loading assets from bundle: " + bundle, bundle, () => LoadAssetsFromBundle(bundle), rowIndex++, colIndex));
            //bundleLoadTestInfo.Add(AssetLoadTest("Loading assets from bundle: " + bundle + " async", bundle, () => LoadAssetsFromBundleAsync(bundle), rowIndex++, colIndex));
            //bundleLoadTestInfo.Add(AssetLoadTest("Loading assets from bundle: " + bundle + " with batch size: " + batchSize, bundle, () => LoadAssetsFromBundleMultiLoad(bundle, batchSize), rowIndex++, colIndex));
            colIndex++;
        }

        return bundleLoadTestInfo;
    }

    private TestFuncInfo BundleLoadTest(string name, Func<IEnumerator> testFunc, int row, int col)
    {
        var info = new TestFuncInfo();
        info.name = name;
        info.bundleTest = gameObject.AddComponent<BundleTest>();
        info.bundleTest.testFunc = testFunc;
        info.bundleTest.cleanupFunc = UnloadBundle;
        info.i = row;
        info.j = col;
        return info;
    }

    private TestFuncInfo AssetLoadTest(string name, string bundle, Func<IEnumerator> testFunc, int row, int col)
    {
        var info = new TestFuncInfo();
        info.name = name;
        info.bundleTest = gameObject.AddComponent<BundleTest>();
        info.bundleTest.testFunc = testFunc;
        info.bundleTest.cleanupFunc = UnloadBundle;
        info.bundleTest.initFunc = () => LoadBundleFromFile(bundle);
        info.i = row;
        info.j = col;
        return info;
    }

    private IEnumerator UnloadBundle()
    {
        GetComponent<AssetBundleManager>().UnloadBundle();
        Debug.Log("Bundle unloaded");
        yield return null;
    }

    private IEnumerator IterateCoroutine(Func<IEnumerator> coroutine, int n)
    {
        for (int i = 0; i < n; i++)
        {
            yield return StartCoroutine(coroutine());
        }
    }

    private IEnumerator LoadBundleFromFile(string bundleName)
    {
        var assetBundleManager = GetComponent<AssetBundleManager>();
        string assetBundlePath = GetBundlePath(bundleName);
        yield return StartCoroutine(assetBundleManager.LoadBundleFromFileUncompressed(assetBundlePath));
    }

    private IEnumerator LoadBundleFromFileAsync(string bundleName)
    {
        var assetBundleManager = GetComponent<AssetBundleManager>();
        string assetBundlePath = GetBundlePath(bundleName);
        yield return StartCoroutine(assetBundleManager.LoadBundleFromFileAsync(assetBundlePath));
    }

    private IEnumerator LoadBundleFromFileWWW(string bundleName)
    {
        var assetBundleManager = GetComponent<AssetBundleManager>();
        string assetBundlePath = GetBundlePath(bundleName);
        yield return StartCoroutine(assetBundleManager.LoadBundleFromWWW(assetBundlePath));
    }

    private IEnumerator LoadAssetsFromBundle(string bundleName)
    {
        var assetBundleManager = GetComponent<AssetBundleManager>();
        string assetBundlePath = GetBundlePath(bundleName);
        //Debug.Log(bundleName);
        //Debug.Log(GetAssetNames(assetBundlePath)[0]);
        yield return StartCoroutine(
            assetBundleManager.LoadAssetsFromBundle(assetBundleManager.bundle, GetAssetNames(assetBundlePath)));
    }

    private IEnumerator LoadAssetsFromBundleAsync(string bundleName)
    {
        var assetBundleManager = GetComponent<AssetBundleManager>();
        string assetBundlePath = GetBundlePath(bundleName);
        yield return StartCoroutine(
            assetBundleManager.LoadAssetsFromBundleAsync(assetBundleManager.bundle, GetAssetNames(assetBundlePath)));
    }

    private IEnumerator LoadAssetsFromBundleMultiLoad(string bundleName, int batchSize)
    {
        var assetBundleManager = GetComponent<AssetBundleManager>();
        string assetBundlePath = GetBundlePath(bundleName);
        yield return StartCoroutine(
            assetBundleManager.LoadAssetsFromBundleSimultaneouslyAsync(
            assetBundleManager.bundle, GetAssetNames(assetBundlePath), batchSize));
    }

    private string GetBundlePath(string bundleName)
    {
        //return Application.dataPath + Path.AltDirectorySeparatorChar +
         //       bundleDirectoryName + Path.AltDirectorySeparatorChar + bundleName + ".unity3d";
        return bundleDirectoryPath + bundleName + ".unity3d";
    }

    protected override int[] GetBundleSizes()
    {
        return
            (from bundlePath in System.IO.Directory.GetFiles(bundleDirectoryPath, "*.unity3d")
             select (int)(new FileInfo(bundlePath).Length)).ToArray<int>();
    }

    protected override int[] GetNumberOfAssets()
    {
        return
            (from bundlePath in GetBundlePaths()
             select GetAssetNames(bundlePath).Length).ToArray<int>();
    }

    protected override string[] GetColumnNames()
    {
        return GetBundleNames();
    }

    protected override string[] GetRowNames()
    {
        return new string[] {
            "Size",
            "# Assets",
            "Load Bundle", 
            "Load Bundle Async", 
            "Load Bundle WWW" ,
            "Load Asset",
            "Load Asset Async",
            "Load Asset Multi Load"
        };
    }
    

    private string[] GetBundleNames()
    {
        return
            (from bundlePath in GetBundlePaths()
             select (Path.GetFileName(bundlePath)).Replace(".unity3d", "")).ToArray<string>();
    }

    private string[] GetBundlePaths()
    {
        return System.IO.Directory.GetFiles(bundleDirectoryPath, "*.unity3d");
    }

    private string[] GetAssetNames(string path)
    {
        string namesPath = path.Replace(".unity3d", ".txt");
        return File.ReadAllLines(namesPath);
    }
}
