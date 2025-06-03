using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Catastlavia : SpellEffectBase
{
    [SerializeField] private GameObject laviaPrefab;
    [SerializeField] private GameObject laviaPrefab2;
    [SerializeField] private GameObject particle1;
    [SerializeField] private GameObject particle2;
    [SerializeField] private GameObject particle3;
    private Transform effectTransform;
    [SerializeField] private Transform effectParent;
    private const int x = -1;
    private const float minY = -1;
    private const float maxY = 2;
    private const float minZ = -1.5f;
    private const float maxZ = 3;
    private const int posNumber = 8;
    private readonly List<Vector3> pos = new() { new(x, 0, 0), new(x, -1, -1.5f), new(x, -1, 2), new(x, 2, -1), new(x, 2, 2.5f), new(x, 1, 0.5f), new(x, 0, 3), new(x, 0.5f, -1) };
    [SerializeField] private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    public int defaultIntensity = 10;
    public int middleIntensity = 12;
    public int maxIntensity = 25;
    public int finishIntensity = 20;
    public float defaultThreshold = 0.9f;
    public float middleThreshold = 0.8f;
    public float minThreshold = 0.5f;
    public float finishThreshold = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        effectTransform = GetComponent<Transform>();
        bloom = postProcessVolume.profile.GetSetting<Bloom>();
        particle1.SetActive(false);
        particle2.SetActive(false);
        particle3.SetActive(false);
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
    }

    public override IEnumerator Activate(Vector3 position, Quaternion quaternion)
    {
        yield return null;
        effectTransform.position = position;
        effectTransform.rotation = quaternion;
        for (int i = 0; i < posNumber; i++)
        {
            float angleX = Mathf.Atan2(pos[i].y, -pos[i].z);
            GameObject arrow = Instantiate(laviaPrefab, effectParent);
            arrow.transform.localPosition = pos[i];
            arrow.transform.localRotation = Quaternion.Euler(new(angleX * Mathf.Rad2Deg, 0, 0));
            StartCoroutine(ArrowManage(arrow, 2.99f));
            yield return new WaitForSeconds(0.1f);
        }
        particle1.SetActive(true);
        yield return new WaitForSeconds(0.45f);
        StartCoroutine(Flash1());
        yield return new WaitForSeconds(1.25f);
        StartCoroutine(Flash2());
        yield return new WaitForSeconds(0.5f);
        particle3.SetActive(true);
        for (int i = 0; i < 25; i++)
        {
            float y = UnityEngine.Random.Range(minY, maxY);
            float z = UnityEngine.Random.Range(minZ, maxZ);
            float angleX = Mathf.Atan2(y, -z);
            GameObject arrow = Instantiate(laviaPrefab2, effectParent);
            arrow.transform.localPosition = new(x, y, z);
            arrow.transform.localRotation = Quaternion.Euler(new(angleX * Mathf.Rad2Deg, 0, 0));
            StartCoroutine(ArrowManage(arrow, 0.49f));
            if (i % 2 == 0)
            {
                float y2 = UnityEngine.Random.Range(minY, maxY);
                float z2 = UnityEngine.Random.Range(minZ, maxZ);
                float angleX2 = Mathf.Atan2(y2, -z2);
                GameObject arrow2 = Instantiate(laviaPrefab2, effectParent);
                arrow2.transform.localPosition = new(x, y2, z2);
                arrow2.transform.localRotation = Quaternion.Euler(new(angleX2 * Mathf.Rad2Deg, 0, 0));
                StartCoroutine(ArrowManage(arrow2, 0.49f));
            }
            if (i == 19)
            {
                StartCoroutine(Flash3());
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject);
    }
    private IEnumerator Flash1()
    {
        //溜めの時の発光増加(1秒)
        while (bloom.intensity < middleIntensity)
        {
            bloom.intensity.value += (middleIntensity - defaultIntensity) * Time.deltaTime;
            bloom.threshold.value -= (defaultThreshold - middleThreshold) * Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = middleIntensity;
        bloom.threshold.value = middleThreshold;
    }
    private IEnumerator Flash2()
    {
        //発射の瞬間の特大フラッシュ(0.1秒*2)
        while (bloom.intensity < maxIntensity)
        {
            bloom.intensity.value += (maxIntensity - middleIntensity) * Time.deltaTime * 10;
            bloom.threshold.value -= (middleThreshold - minThreshold) * Time.deltaTime * 10;
            yield return null;
        }
        bloom.intensity.value = maxIntensity;
        bloom.threshold.value = minThreshold;
        yield return null;
        while (bloom.intensity > middleIntensity)
        {
            bloom.intensity.value -= (maxIntensity - middleIntensity) * Time.deltaTime * 10f;
            bloom.threshold.value += (middleThreshold - minThreshold) * Time.deltaTime * 10f;
            yield return null;
        }
        bloom.intensity.value = middleIntensity;
        bloom.threshold.value = middleThreshold;
    }
    private IEnumerator Flash3()
    {
        //最後のフラッシュ(1秒 * 2)
        while (bloom.intensity < finishIntensity)
        {
            bloom.intensity.value += (finishIntensity - middleIntensity) * Time.deltaTime;
            bloom.threshold.value -= (middleThreshold - finishThreshold) * Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = finishIntensity;
        bloom.threshold.value = finishThreshold;
        yield return null;
        particle2.SetActive(true);
        while (bloom.intensity > defaultIntensity)
        {
            bloom.intensity.value -= (finishIntensity - defaultIntensity) * Time.deltaTime;
            bloom.threshold.value += (defaultThreshold - finishThreshold) * Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = defaultIntensity;
        bloom.threshold.value = defaultThreshold;
    }
    private IEnumerator ArrowManage(GameObject arrow, float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(arrow);
    }
}