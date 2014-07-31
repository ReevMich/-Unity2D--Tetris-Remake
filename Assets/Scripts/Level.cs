using System.Collections;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int level;
    public int lines;
    private static int linesLeft;

    public float fallspeed;
    private const float fallSpeed = .15f;

    // Use this for initialization
    private void Start ()
    {
        level = 1;
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
    }

    public static void UpdateLinesLeft ()
    {
        linesLeft--;
    }

    private void NextLevel ()
    {
        level++;
        Tetris.BlockFallSpeed -= fallSpeed;
        SetLines();
    }

    private void SetLines ()
    {
        linesLeft = 0;
        linesLeft = level * 1;

        fallspeed = Tetris.BlockFallSpeed;
    }
}