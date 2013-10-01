using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class AssetBundleTest
{

	public Func<IEnumerator> testFunc;
	public Func<IEnumerator> initFunc;
	public Func<IEnumerator> cleanupFunc;
	public int numberOfRunsToPerform;
	
	public List<TestRun> runs;
	public float averageMaxFrameTime;
	public float averageMeanFrameTime;
	public float averageDuration;
	
	public AssetBundleTest(Func<IEnumerator> testFunc, Func<IEnumerator> initFunc, Func<IEnumerator> cleanupFunc, int numberOfRunsToPerform) {
		this.testFunc = testFunc;
		this.initFunc = initFunc;
		this.cleanupFunc = cleanupFunc;
		this.numberOfRunsToPerform = numberOfRunsToPerform;
	}
	
	public void AverageRunResults()
	{
		averageMaxFrameTime = (from run in runs select run.maxFrameTime).Average ();
		averageMeanFrameTime = (from run in runs select run.meanFrameTime).Average ();
		averageDuration = (from run in runs select run.duration).Average ();
	}
	
	public override string ToString()
	{
		return "Duration : " + averageDuration + " ." +
                "Max frame time : " + averageMaxFrameTime + ". " +
                "Mean frame time : " + averageMeanFrameTime; 
	}

	public string Output()
	{
		return averageDuration + "|" + averageMaxFrameTime + "|" + averageMeanFrameTime;
	}

	public static string OutputLegend()
	{
		return "Duration | Max Frame Time | Mean Frame Time";
	}
}
