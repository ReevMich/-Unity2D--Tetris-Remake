using UnityEngine;

public class Score : MonoBehaviour
{
    private static int FORCED_FALL_SCORE = 1;
    private static int HARD_DROP_MULTIPLIER = 2;
    private static int LINES_CLEARED = 100;
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

    public static void IncreaseScoreLine (int linesCleared)
    {
        if (linesCleared == 0)
        {
            return;
        }

        int bonus = 0;

        switch (linesCleared)
        {
            case 1: bonus = 0; break;
            case 2: bonus = 200; break;
            case 3: bonus = 300; break;
            case 4: bonus = 400; break;
        }

        _score += (bonus + LINES_CLEARED);
    }

    public static void IncreaseScoreBlock ()
    {
        _score += 10;
    }

    public static void IncreaseScoreSoftDrop ()
    {
        _score += FORCED_FALL_SCORE;
    }

    public static void IncreaseScoreHardDrop (int dropAmount)
    {
        _score += dropAmount * HARD_DROP_MULTIPLIER;
    }

    public static void Reset ()
    {
        _score = 0;
    }
}