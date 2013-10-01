using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

public class AssetBundleTestManager : MonoBehaviour {
	public int numberOfRunsPerTest = 5;
    public static readonly int batchSize = 3;
	
	public static readonly string outputTableName = "results_table.csv";

    public class TestFuncInfo
    {
        public AssetBundleTest bundleTest;
        public string name;
        public int i, j;
    }
	
	public IEnumerator PerformTest(AssetBundleTest test)
	{
		test.runs = new List<TestRun>();
		
		if (test.initFunc != null)
			yield return StartCoroutine(test.initFunc());
		
		for (int i = 0; i < test.numberOfRunsToPerform; i++) 
		{
			var run = new TestRun();
			run.Begin ();
			if (test.testFunc != null)
				yield return StartCoroutine(test.testFunc());
			run.End();
			test.runs.Add (run);
		}
		
		if (test.cleanupFunc != null)
			yield return StartCoroutine(test.cleanupFunc());
		
		test.AverageRunResults();
	}
	
    protected IEnumerable<TestFuncInfo> GetTestFunctions()
    {
        var assetBundles = BenchmarkUtilities.GetBundleNames();
        var bundleLoadTestInfo = new List<TestFuncInfo>();
        int colIndex = 0;
        foreach (var bundleName in assetBundles)
        {
            var bundle = bundleName;
            int rowIndex = 0;
            //bundleLoadTestInfo.Add(CreateBundleLoadTest("Loading bundle: " + bundle + " from file", () => Static.AssetBundleTestFunctions.LoadBundleFromFile(bundle), rowIndex++, colIndex));
            //bundleLoadTestInfo.Add(BundleLoadTest("Loading bundle: " + bundle + " from file async", () => Static.AssetBundleTestFunctions.LoadBundleFromFileAsync(bundle), rowIndex++, colIndex));
            //bundleLoadTestInfo.Add(BundleLoadTest("Loading bundle: " + bundle + " from file WWW", () => Static.AssetBundleTestFunctions.LoadBundleFromFileWWW(bundle), rowIndex++, colIndex));
            bundleLoadTestInfo.Add(CreateAssetLoadTest("Loading assets from bundle: " + bundle, bundle, () => Static.AssetBundleTestFunctions.LoadAssetsFromBundle(bundle), rowIndex++, colIndex));
            //bundleLoadTestInfo.Add(AssetLoadTest("Loading assets from bundle: " + bundle + " async", bundle, () => Static.AssetBundleTestFunctions.LoadAssetsFromBundleAsync(bundle), rowIndex++, colIndex));
            //bundleLoadTestInfo.Add(AssetLoadTest("Loading assets from bundle: " + bundle + " with batch size: " + batchSize, bundle, () => Static.AssetBundleTestFunctions.LoadAssetsFromBundleMultiLoad(bundle, batchSize), rowIndex++, colIndex));
            colIndex++;
        }

        return bundleLoadTestInfo;
    }

    IEnumerator Start()
    {
		
        int rowOffset = 3;
        int colOffset = 1;

        string[,] table = new string[BenchmarkUtilities.GetRowNames().Length + rowOffset, BenchmarkUtilities.GetColumnNames().Length + colOffset];

        // fill in table names
        int index = 1;
        foreach (var colName in BenchmarkUtilities.GetColumnNames())
        {
            table[0, index++] = colName;
        }
        index = 1;
        foreach (var size in BenchmarkUtilities.GetBundleSizes())
        {
            table[1, index++] = size.ToString();
        }
        index = 1;
        foreach (var nAssets in BenchmarkUtilities.GetNumberOfAssets())
        {
            table[2, index++] = nAssets.ToString();
        }
        index = 1;
        foreach (var rowName in BenchmarkUtilities.GetRowNames())
        {
            table[index++, 0] = rowName;
        }

        // fill in values
        foreach (var testFuncInfo in GetTestFunctions())
        {
            var bundleTest = testFuncInfo.bundleTest;
            yield return StartCoroutine(PerformTest(bundleTest));
            Static.GUIText.text += "Performing " + testFuncInfo.name + "...\n";
            table[testFuncInfo.i + rowOffset, testFuncInfo.j + colOffset] = bundleTest.Output();
        }
        Static.GUIText.text += "...Done!\n";
        // output
        OutputTable(table, BenchmarkUtilities.bundleDirectoryPath + outputTableName);
    }

    private void OutputTable(string[,] table, string path)
    {
        string delimiter = ",";
        StreamWriter fs = new StreamWriter(path);
        fs.Write(AssetBundleTest.OutputLegend());
        for (int i = 0; i < table.GetLength(0); i++)
        {
            for (int j = 0; j < table.GetLength(1); j++)
            {
                fs.Write(table[i, j] + delimiter);
            }
            fs.Write(Environment.NewLine);
        }
        fs.Close();
        Static.GUIText.text += "Saved output to : " + path;
    }
	
    private TestFuncInfo CreateBundleLoadTest(string name, Func<IEnumerator> testFunc, int row, int col)
    {
        var info = new TestFuncInfo();
        info.name = name;
		info.bundleTest = new AssetBundleTest(testFunc, null, Static.AssetBundleTestFunctions.UnloadBundle, numberOfRunsPerTest);
        info.i = row;
        info.j = col;
        return info;
    }

    private TestFuncInfo CreateAssetLoadTest(string name, string bundle, Func<IEnumerator> testFunc, int row, int col)
    {
        var info = new TestFuncInfo();
        info.name = name;
        info.bundleTest = new AssetBundleTest(testFunc, () => Static.AssetBundleTestFunctions.LoadBundleFromFile(bundle), Static.AssetBundleTestFunctions.UnloadBundle, numberOfRunsPerTest);
        info.i = row;
        info.j = col;
        return info;
    }
}