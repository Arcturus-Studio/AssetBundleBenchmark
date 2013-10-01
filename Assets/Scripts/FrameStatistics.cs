using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FrameStatistics : MonoBehaviour {

    private int frameCounter;
    private Queue<float> frameDeltas;

    public float MeanFrameTime { get { return frameDeltas.Sum() / (float)frameDeltas.Count; } }
    public float MinFrameTime { get { return frameDeltas.Min(); } }
    public float MaxFrameTime { get { return frameDeltas.Max(); } }

    private string meanFrameDelta;
    private string maxFrameDelta;
    private string minFrameDelta;

	void Start () 
	{
        Reset();
	}
	
	void Update () 
	{

        frameDeltas.Enqueue(Time.deltaTime);

        float meanFrames = frameDeltas.Sum() / (float)frameDeltas.Count;
        meanFrameDelta = "\n Mean frame delta : " + meanFrames.ToString();
        maxFrameDelta = "\n Max frame delta : " + frameDeltas.Max().ToString();
        minFrameDelta = "\n Min frame delta : " + frameDeltas.Min().ToString();

        //guiText.text = meanFrameDelta + maxFrameDelta + minFrameDelta;
	}

    public void Reset()
    {
        frameDeltas = new Queue<float>();
    }

}
