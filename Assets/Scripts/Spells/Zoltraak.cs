using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoltraak : SpellEffectBase
{
    private Transform effectTransform;
    private const float animationTime = 7.25f;

    void Start()
    {
        effectTransform = GetComponent<Transform>();
    }

    public override void Activate(Vector3 position, Quaternion quaternion)
    {
        StartCoroutine(EffectGenerater(position, quaternion));
    }
    private IEnumerator EffectGenerater(Vector3 position, Quaternion quaternion)
    {
        yield return null;
        effectTransform.position = position;
        effectTransform.rotation = quaternion;
        yield return new WaitForSeconds(animationTime);
        Destroy(gameObject);
    }
}