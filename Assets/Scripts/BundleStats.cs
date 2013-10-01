using UnityEngine;
using System.Collections;

public class BundleStats : MonoBehaviour{

    public float maxFrameTime;
    public float meanFrameTime;
    public float duration;

    private float start;
    private FrameStatistics frameStats;

    public void Begin()
    {
        start = Time.realtimeSinceStartup;
        frameStats = GetComponent<FrameStatistics>();
        frameStats.Initialize();
    }

    public void End()
    {
        duration = Time.realtimeSinceStartup - start;
        maxFrameTime = frameStats.MaxFrameTime;
        meanFrameTime = frameStats.MeanFrameTime;
    }

    public override string ToString()
    {
        return "Duration : " + duration + " ." +
                "Max frame time : " + maxFrameTime + ". " +
                "Mean frame time : " + meanFrameTime; 
    }

    public string Output()
    {
        return duration + "|" + maxFrameTime + "|" + meanFrameTime;
    }

    public static string OutputLegend()
    {
        return "Duration | Max Frame Time | Mean Frame Time";
    }
}
