//using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AssetBundleManager : MonoBehaviour
{
    private string filePrefix = "file://";
    private string bundlePath;

    public List<UnityEngine.Object> objects;
    public AssetBundle bundle;

    public void UnloadBundle()
    {
        if (bundle != null)
            bundle.Unload(true);
    }

    public IEnumerator LoadBundleFromFileAsync(string path)
    {
        CheckFileExists(path);
        FileStream fs = File.OpenRead(path);
        byte[] buffer = new byte[fs.Length];
        System.IAsyncResult result = fs.BeginRead(buffer, 0, (int)fs.Length, null, null);
        while (!result.IsCompleted)
        {
            yield return null;
        }
        fs.Dispose();
        yield return StartCoroutine(LoadBundleFromMemory(buffer));
        Debug.Log("loaded bundle: " + path);
    }

    public IEnumerator LoadBundleFromMemory(byte[] buffer)
    {
        AssetBundleCreateRequest bundleRequest = AssetBundle.CreateFromMemory(buffer);
        yield return bundleRequest;
        if (bundleRequest.isDone)
        {
            bundle = bundleRequest.assetBundle;
            CheckBundle(bundle);
        }
    }

    public IEnumerator LoadBundleFromFileUncompressed(string path)
    {
        CheckFileExists(path);
        bundle = AssetBundle.CreateFromFile(path);
        CheckBundle(bundle);
        Debug.Log("loaded bundle: " + path);
        yield return null;
    }

    public IEnumerator LoadBundleFromWWW(string path)
    {
        CheckFileExists(path);
        using (WWW www = new WWW(GetURLFromPath(path)))
        {
            yield return www;
            if (www.error != null)
                Debug.LogError("WWW download has error:" + www.error);
            else
            {
                Debug.Log("loaded bundle: " + path);
                bundle = www.assetBundle;
                CheckBundle(bundle);
            }
        }
    }

    public IEnumerator LoadAssetsFromBundle(AssetBundle bundle, string[] names)
    {
        objects = new List<Object>();

        foreach (var name in names)
        {
            Object obj = bundle.Load(name);
            TryAddAsset(name, obj, objects);
        }
        yield return null;
    }

    public IEnumerator LoadAssetsFromBundleAsync(AssetBundle bundle, string[] names)
    {
        objects = new List<Object>();

        foreach (var name in names)
        {
            AssetBundleRequest request = bundle.LoadAsync(name, typeof(UnityEngine.Object));

            yield return request;
            UnityEngine.Object obj = request.asset;
            TryAddAsset(name, obj, objects);

        }
    }

    public IEnumerator LoadAssetsFromBundleSimultaneouslyAsync(AssetBundle bundle, string[] names, int nAssets)
    {
        objects = new List<Object>();
        List<AssetBundleRequest> requests = new List<AssetBundleRequest>();

        for (int i = 0; i < names.Length; i++)
        {
            string name = names[i];
            requests.Add(bundle.LoadAsync(name, typeof(Object)));

            if (requests.Count == nAssets || i == names.Length - 1)
            {
                while (!AreAssetsLoaded(requests))
                {
                    yield return null;
                }
                foreach (var request in requests)
                {
                    Object obj = request.asset;
                    TryAddAsset(obj.name, obj, objects);
                }
                Debug.Log("Loaded " + requests.Count + " assets from the bundle");
                requests = new List<AssetBundleRequest>();
            }
        }
    }

    private bool AreAssetsLoaded(List<AssetBundleRequest> requests)
    {
        foreach (var request in requests)
        {
            if (!request.isDone)
                return false;
        }
        return true;
    }

    private string GetURLFromPath(string path)
    {
        return filePrefix + path;
    }

    private void CheckFileExists(string path)
    {
        if (!File.Exists(path))
            throw new System.Exception("Cannot find file at the path: " + path);
        else
            bundlePath = path;

    }

    private void TryAddAsset(string name, Object obj, List<Object> objects)
    {
        if (obj == null)
            Debug.LogError(name + " could not be found in bundle " + bundlePath);
        else
        {
            //Debug.Log("Added " + name);
            objects.Add(obj);
        }
    }

    private void CheckBundle(AssetBundle bundle)
    {
        if (bundle == null)
            Debug.LogError("Bundle could not be loaded");

    }


    private void CopyTo(Stream input, Stream output)
    {
        byte[] buffer = new byte[16 * 1024];
        int bytesRead;

        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, bytesRead);
        }
    }

    private void CopyToAsync(Stream input)
    {
        
    }

}