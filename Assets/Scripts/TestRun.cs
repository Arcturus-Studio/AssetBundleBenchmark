using UnityEngine;
using System.Collections;

public class TestRun
{

	public float maxFrameTime;
	public float meanFrameTime;
	public float duration;
	private float start;
	private FrameStatistics frameStats;

	public void Begin()
	{
		start = Time.realtimeSinceStartup;
		frameStats = Static.FrameStatistics;
		frameStats.Reset ();
	}

	public void End()
	{
		duration = Time.realtimeSinceStartup - start;
		maxFrameTime = frameStats.MaxFrameTime;
		meanFrameTime = frameStats.MeanFrameTime;
	}
}