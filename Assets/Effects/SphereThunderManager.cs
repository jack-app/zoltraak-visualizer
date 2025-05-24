using System.Collections;
using UnityEngine;

public class SphereThunderManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Expand());
    }
    private IEnumerator Expand()
    {
        while (transform.localScale.x < 2)
        {
            Vector3 temp = transform.localScale;
            temp += new Vector3(1,1,1) * Time.deltaTime * 4;
            transform.localScale = temp;
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }
}
