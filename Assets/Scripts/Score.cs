using UnityEngine;

public class Score : MonoBehaviour
{
    private static int FORCED_FALL_SCORE = 1;
    private static int _score;

    private GUIText scoreText;

    // Use this for initialization
    private void Start ()
    {
        scoreText = GameObject.Find("GUI_Text_Score").transform.GetChild(0).GetComponent<GUIText>();
    }

    // Update is called once per frame
    private void Update ()
    {
        scoreText.text = _score.ToString();
    }

    public static void IncreaseScoreLine ()
    {
        _score += 125;
    }

    public static void IncreaseScoreBlock ()
    {
        _score += 10;
    }

    public static void IncreaseScoreForceFall ()
    {
        _score += FORCED_FALL_SCORE;
    }

    public static void Reset ()
    {
        _score = 0;
    }
}