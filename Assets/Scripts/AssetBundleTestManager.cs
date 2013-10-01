using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class AssetBundleTestManager : MonoBehaviour {
    public static readonly int batchSize = 3;
	public static string bundleDirectoryPath;
	
	 public static readonly string outputTableName = "results_table.csv";

    public class TestFuncInfo
    {
        public AssetBundleTest bundleTest;
        public string name;
        public int i, j;
    }
	
	protected void InitializeDirectoryPath() 
	{
		bundleDirectoryPath = Application.streamingAssetsPath + "/";	
	}
	
    protected IEnumerable<TestFuncInfo> GetTestFunctions()
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

    IEnumerator Start()
    {
		InitializeDirectoryPath();
		
        int rowOffset = 3;
        int colOffset = 1;

        string[,] table = new string[GetRowNames().Length + rowOffset, GetColumnNames().Length + colOffset];

        // fill in table names
        int index = 1;
        foreach (var colName in GetColumnNames())
        {
            table[0, index++] = colName;
        }
        index = 1;
        foreach (var size in GetBundleSizes())
        {
            table[1, index++] = size.ToString();
        }
        index = 1;
        foreach (var nAssets in GetNumberOfAssets())
        {
            table[2, index++] = nAssets.ToString();
        }
        index = 1;
        foreach (var rowName in GetRowNames())
        {
            table[index++, 0] = rowName;
        }

        // fill in values
        foreach (var testFuncInfo in GetTestFunctions())
        {
            var bundleTest = testFuncInfo.bundleTest;
            yield return StartCoroutine(bundleTest.Perform());
            guiText.text += "Performing " + testFuncInfo.name + "...\n";
            table[testFuncInfo.i + rowOffset, testFuncInfo.j + colOffset] = bundleTest.stats.Output();
        }
        guiText.text += "...Done!\n";
        // output
        OutputTable(table, bundleDirectoryPath + outputTableName);
    }

    private void OutputTable(string[,] table, string path)
    {
        string delimiter = ",";
        StreamWriter fs = new StreamWriter(path);
        fs.Write(AssetBundleStats.OutputLegend());
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {
                fs.Write(table[i, j] + delimiter);
            }
            fs.Write(Environment.NewLine);
        }
        fs.Close();
        guiText.text += "Saved output to : " + path;
    }
	
	

    private TestFuncInfo BundleLoadTest(string name, Func<IEnumerator> testFunc, int row, int col)
    {
        var info = new TestFuncInfo();
        info.name = name;
        info.bundleTest = gameObject.AddComponent<AssetBundleTest>();
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
        info.bundleTest = gameObject.AddComponent<AssetBundleTest>();
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

    protected int[] GetBundleSizes()
    {
        return
            (from bundlePath in System.IO.Directory.GetFiles(bundleDirectoryPath, "*.unity3d")
             select (int)(new FileInfo(bundlePath).Length)).ToArray<int>();
    }

    protected int[] GetNumberOfAssets()
    {
        return
            (from bundlePath in GetBundlePaths()
             select GetAssetNames(bundlePath).Length).ToArray<int>();
    }

    protected string[] GetColumnNames()
    {
        return GetBundleNames();
    }

    protected string[] GetRowNames()
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
