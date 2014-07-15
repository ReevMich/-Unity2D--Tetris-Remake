using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	
	private static int _score;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public static void IncreaseScoreLine()
	{
		_score += 125;
	}
	public static void IncreaseScoreBlock()
	{
		_score += 10;
	}

	public static void Reset()
	{
		_score = 0;
	}

	public static int GetScore()
	{
		return _score;
	}
}
