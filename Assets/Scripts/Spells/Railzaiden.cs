using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railzaiden : SpellEffectBase
{
    [SerializeField] private GameObject displays;
    private RectTransform displaysRect;
    [SerializeField] private RectTransform image1Rect;
    [SerializeField] private RectTransform image2Rect;
    private Animator animator;
    private const float animationTime = 3.5f;

    void Start()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        displaysRect = displays.GetComponent<RectTransform>();
        animator = displays.GetComponent<Animator>();
    }

    public override void Activate(Vector3 position, Quaternion quaternion)
    {
        StartCoroutine(Anim());
    }

    private IEnumerator Anim()
    {
        yield return null;
        transform.SetParent(GameObject.Find("Canvas").transform, false);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new(0, 0);
        int angle = UnityEngine.Random.Range(-45, 46);
        AngleSet(angle);
        if (angle <= 0)
        {
            animator.SetBool("Go", true);
        }
        else
        {
            animator.SetBool("Go2", true);
        }
        yield return new WaitForSeconds(animationTime);
        Destroy(gameObject);
    }
    private void AngleSet(int angle)
    {
        displaysRect.localRotation = Quaternion.Euler(0, 0, angle);
        image1Rect.localRotation = Quaternion.Euler(0, 0, -angle);
        image2Rect.localRotation = Quaternion.Euler(0, 0, -angle);
    }
}
