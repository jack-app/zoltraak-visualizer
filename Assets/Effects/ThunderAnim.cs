using System.Collections;
using UnityEngine;

public class ThunderAnim : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private int length = 100;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float speed = 2;
    [SerializeField] private int zDelta = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Anim());
    }

    private IEnumerator Anim()
    {
        while (endPoint.localPosition.x < length)
        {
            Vector3 temp = endPoint.localPosition;
            temp.x += length * Time.deltaTime * speed;
            temp.z -= zDelta * Time.deltaTime * speed;
            endPoint.localPosition = temp;
            yield return null;
        }
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
