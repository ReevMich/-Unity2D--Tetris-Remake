using UnityEngine;

public class AutoRotateObject : MonoBehaviour
{
    public int dir = 1;

    private bool rotating;

    private float speed = 50.0f;

    // Use this for initialization
    private void Start ()
    {
        animation.GetClip("idle1").wrapMode = WrapMode.Loop;
        animation.Play("idle1");

        speed += Random.Range(-10.0f, 10.0f);
        rotating = true;
    }

    // Update is called once per frame
    private void Update ()
    {
        if (rotating)
        {
            transform.Rotate(Vector3.up * 60 * Time.deltaTime * dir);
        }
    }

    private void OnMouseDown ()
    {
        rotating = !rotating;
        if (rotating) dir = ((dir < 0) ? 1 : -1);
    }
}