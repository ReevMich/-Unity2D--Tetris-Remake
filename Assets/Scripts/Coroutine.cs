using System.Collections;
using UnityEngine;

public class Coroutine : MonoBehaviour
{
    private IEnumerator Start ()
    {
        print("Starting " + Time.time);
        yield return StartCoroutine(WaitAndPrint(2.0F));
        print("Done " + Time.time);
    }

    private IEnumerator WaitAndPrint (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        print("WaitAndPrint " + Time.time);
    }
}