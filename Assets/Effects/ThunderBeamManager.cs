using System.Collections;
using UnityEngine;

public class ThunderBeamManager : MonoBehaviour
{
    private float angle = 0;
    void Start()
    {
        StartCoroutine(Rot());
    }

    private IEnumerator Rot()
    {
        while (angle < 360)
        {
            yield return null;
            angle += 60 * Time.deltaTime;
            transform.localEulerAngles = new(angle, 0, 0);
        }
        Destroy(gameObject);
    }
}
