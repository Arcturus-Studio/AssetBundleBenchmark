using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Static : MonoBehaviour {
	public static Static instance;

    void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
		DontDestroyOnLoad(gameObject);
    }
	
    public AssetBundleTestManager assetBundleTestManager;
    public static AssetBundleTestManager AssetBundleTestManager {
        get { return instance.assetBundleTestManager; }
    }
	
	public AssetBundleManager assetBundleManager;
    public static AssetBundleManager AssetBundleManager {
        get { return instance.assetBundleManager; }
    }
	
	public AssetBundleTestFunctions assetBundleTestFunctions;
    public static AssetBundleTestFunctions AssetBundleTestFunctions {
        get { return instance.assetBundleTestFunctions; }
    }
	
	public FrameStatistics frameStatistics;
    public static FrameStatistics FrameStatistics {
        get { return instance.frameStatistics; }
    }
	
	public GUIText guiText;
    public static GUIText GUIText {
        get { return instance.guiText; }
    }
}
