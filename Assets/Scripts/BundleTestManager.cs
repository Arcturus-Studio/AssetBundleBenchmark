using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public abstract class BundleTestManager : MonoBehaviour {

    public static readonly string outputTableName = "results_table.csv";

    public class TestFuncInfo
    {
        public BundleTest bundleTest;
        public string name;
        public int i, j;
    }

    protected abstract IEnumerable<TestFuncInfo> GetTestFunctions();
    protected abstract string[] GetColumnNames();
    protected abstract string[] GetRowNames();
    protected abstract int[] GetBundleSizes();
    protected abstract int[] GetNumberOfAssets();
	protected abstract void InitializeDirectoryPath();

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
        OutputTable(table, BundleLoadingManager.bundleDirectoryPath + outputTableName);
    }

    private void OutputTable(string[,] table, string path)
    {
        string delimiter = ",";
        StreamWriter fs = new StreamWriter(path);
        fs.Write(BundleStats.OutputLegend());
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
}
