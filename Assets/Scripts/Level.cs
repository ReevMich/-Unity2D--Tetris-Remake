using UnityEngine;

public class Level : MonoBehaviour
{
    public int level;
    public int lines;
    private static int linesLeft;
    private GUIText linesText;
    private GUIText levelText;

    private const float START_FALL_SPEED = 1f;
    private static float gameSpeed;
    private const float speedDecriment = .062f;

    // Use this for initialization
    private void Start ()
    {
        linesText = GameObject.Find("GUI_Text_Lines").transform.GetChild(0).GetComponent<GUIText>();
        levelText = GameObject.Find("GUI_Text_Level").transform.GetChild(0).GetComponent<GUIText>();
        level = 1;
        gameSpeed = START_FALL_SPEED;
        SetLines();
    }

    // Update is called once per frame
    private void Update ()
    {
        lines = linesLeft;

        if (linesLeft <= 0)
        {
            NextLevel();
        }

        UpdateText();
    }

    public static void UpdateLinesLeft ()
    {
        linesLeft--;
    }

    private void NextLevel ()
    {
        level++;
        gameSpeed -= speedDecriment;
        SetLines();
    }

    private void SetLines ()
    {
        if (level <= 5)
        {
            linesLeft = 5;
        }
        else
        {
            int levelOver = level - 5;
            linesLeft = 5 + levelOver;
        }
    }

    private void UpdateText ()
    {
        linesText.text = linesLeft.ToString();
        levelText.text = level.ToString();
    }

    public static float GetGameSpeed ()
    {
        return gameSpeed;
    }

    private void Reset ()
    {
        Start();
    }
}