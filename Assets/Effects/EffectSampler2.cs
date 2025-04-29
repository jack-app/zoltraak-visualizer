using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSampler2 : MonoBehaviour
{
    [SerializeField] private GameObject displays;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = displays.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("Go", true);
        }
    }
    public void AnimationFinished()
    {
        animator.SetBool("Go", false);
    }
}
