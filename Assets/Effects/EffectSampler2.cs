using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSampler2 : MonoBehaviour
{
    [SerializeField] private GameObject displays;
    private RectTransform displaysRect;
    [SerializeField] private RectTransform image1Rect;
    [SerializeField] private RectTransform image2Rect;
    private Animator animator;
    private bool isAnimation = false;
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        displaysRect = displays.GetComponent<RectTransform>();
        animator = displays.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAnimation)
        {
            int angle = UnityEngine.Random.Range(-45, 46);
            AngleSet(angle);
            isAnimation = true;
            if (angle <= 0)
            {
                animator.SetBool("Go", true);
            }
            else
            {
                animator.SetBool("Go2", true);
            }
        }
    }
    //画面の角度調整
    private void AngleSet(int angle)
    {
        displaysRect.localRotation = Quaternion.Euler(0, 0, angle);
        image1Rect.localRotation = Quaternion.Euler(0, 0, -angle);
        image2Rect.localRotation = Quaternion.Euler(0, 0, -angle);
    }
    public void AnimationFinished()
    {
        animator.SetBool("Go", false);
        animator.SetBool("Go2", false);
        isAnimation = false;
    }
}