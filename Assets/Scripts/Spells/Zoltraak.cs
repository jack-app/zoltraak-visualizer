using System;
using System.Collections;
using UnityEngine;

public class Zoltraak : SpellEffectBase
{
    private Transform effectTransform;
    private const float animationTime = 7.25f;

    void Start()
    {
        effectTransform = GetComponent<Transform>();
    }

    public override IEnumerator Activate(Vector3 position, Quaternion quaternion)
    {
        yield return null;
        StartCoroutine(ScreenShot());
        effectTransform.position = position;
        effectTransform.rotation = quaternion;
        yield return new WaitForSeconds(animationTime);
        Destroy(gameObject);
    }
    private IEnumerator ScreenShot()
    {
        if (screenShotManager != null)
        {
            yield return new WaitForSeconds(2f);
            screenShotManager.TakeScreenshot();
            yield return new WaitForSeconds(1.5f);
            screenShotManager.TakeScreenshot();
        }
    }
}