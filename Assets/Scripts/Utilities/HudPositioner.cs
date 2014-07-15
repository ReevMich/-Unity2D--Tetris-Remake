using System.Collections;
using UnityEngine;

public class HudPositioner : MonoBehaviour
{
    public enum Alignment
    {
        UpperLeft = 0,
        UpperCenter = 1,
        UpperRight = 2,
        MiddleLeft = 3,
        MiddleCenter = 4,
        MiddleRight = 5,
        LowerLeft = 6,
        LowerCenter = 7,
        LowerRight = 8
    }

    public Alignment alignment = 0;
    public Vector2 offset = Vector2.zero;

    private void Start ()
    {
        Align((int)alignment, offset);
    }

    private void Align (int i, Vector2 off)
    {
        Transform camTransform = Camera.main.transform;
        float dist = Vector3.Project((camTransform.position - transform.position), camTransform.forward).magnitude;
        Vector3 screenPos = new Vector3(0, 0, dist);

        switch (i)
        {
            case 0:
                screenPos.x = 0 + off.x;
                screenPos.y = Screen.height + off.y;
                break;

            case 1:
                screenPos.x = (float)Screen.width / 2f + off.x;
                screenPos.y = Screen.height + off.y;
                break;

            case 2:
                screenPos.x = Screen.width + off.x;
                screenPos.y = Screen.height + off.y;
                break;

            case 3:
                screenPos.x = 0 + off.x;
                screenPos.y = (float)Screen.height / 2f + off.y;
                break;

            case 4:
                screenPos.x = (float)Screen.width / 2f + off.x;
                screenPos.y = (float)Screen.height / 2f + off.y;
                break;

            case 5:
                screenPos.x = Screen.width + off.x;
                screenPos.y = (float)Screen.height / 2f + off.y;
                break;

            case 6:
                screenPos.x = 0 + off.x;
                screenPos.y = 0 + off.y;
                break;

            case 7:
                screenPos.x = (float)Screen.width / 2f + off.x;
                screenPos.y = 0 + off.y;
                break;

            case 8:
                screenPos.x = Screen.width + off.x;
                screenPos.y = 0 + off.y;
                break;
        }
        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
    }
}