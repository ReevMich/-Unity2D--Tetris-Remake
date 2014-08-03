using UnityEngine;

public class MC_AutoRotate : MonoBehaviour
{
    public Vector3 rotation;

    private void Update ()
    {
        this.transform.Rotate(rotation * Time.deltaTime, Space.World);
    }
}