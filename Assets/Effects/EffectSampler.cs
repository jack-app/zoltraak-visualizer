using System.Collections;
using UnityEngine;

public class EffectSampler : MonoBehaviour
{
    [SerializeField] private GameObject effectParent;
    private Transform effectTransform;
    private int counter = 0;

    void Start()
    {
        effectTransform = effectParent.GetComponent<Transform>();
        effectParent.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !effectParent.activeSelf)
        {
            Transform destination = effectTransform;
            //�Ƃ肠�����蓮��ransform��^����
            switch (counter)
            {
                case 0:
                    destination.position = new(0, 0, 0);
                    destination.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 1:
                    destination.position = new(-3, 0, 2);
                    destination.rotation = Quaternion.Euler(0, 30, 0);
                    break;
                case 2:
                    destination.position = new(4, 2, 2);
                    destination.rotation = Quaternion.Euler(0, 150, -20);
                    break;
                case 3:
                    destination.position = new(0, 0, -4);
                    destination.rotation = Quaternion.Euler(0, -70, 0);
                    counter = -1;
                    break;
            }
            counter++;
            StartCoroutine(EffectGenerater(destination));
        }
    }

    //Transform���󂯎���Ă��̈ʒu�A�����ɃG�t�F�N�g�𔭐�������
    public IEnumerator EffectGenerater(Transform destination)
    {
        effectTransform = destination;
        effectParent.SetActive(true);
        yield return new WaitForSeconds(3);
        effectParent.SetActive(false);
    }
}
