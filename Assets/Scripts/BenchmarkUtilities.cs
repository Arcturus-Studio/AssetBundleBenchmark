using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Linq;

public class BenchmarkUtilities {
	public static string bundleDirectoryPath = Application.streamingAssetsPath + "/";
	
  	public static string GetBundlePath(string bundleName)
    {
        return bundleDirectoryPath + bundleName + ".unity3d";
    }

    public static int[] GetBundleSizes()
    {
        return
            (from bundlePath in System.IO.Directory.GetFiles(bundleDirectoryPath, "*.unity3d")
             select (int)(new FileInfo(bundlePath).Length)).ToArray<int>();
    }

    public static int[] GetNumberOfAssets()
    {
        return
            (from bundlePath in GetBundlePaths()
             select GetAssetNames(bundlePath).Length).ToArray<int>();
    }

    public static string[] GetColumnNames()
    {
        return GetBundleNames();
    }

    public static string[] GetRowNames()
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
    
    public static string[] GetBundleNames()
    {
        return
            (from bundlePath in GetBundlePaths()
             select (Path.GetFileName(bundlePath)).Replace(".unity3d", "")).ToArray<string>();
    }

    public static string[] GetBundlePaths()
    {
        return System.IO.Directory.GetFiles(bundleDirectoryPath, "*.unity3d");
    }

    public static string[] GetAssetNames(string path)
    {
        string namesPath = path.Replace(".unity3d", ".txt");
        return File.ReadAllLines(namesPath);
    }
}
