using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Judolazirum : SpellEffectBase
{
    private Transform myTransform;
    [SerializeField] private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    [SerializeField] private GameObject smallThunderPrefab;
    [SerializeField] private GameObject sphereThunderPrefab;
    [SerializeField] private GameObject thunderBeamPrefab;
    [SerializeField] private GameObject thunderStrikePrefab;
    [SerializeField] private GameObject additionalThunderPrefab;
    public int defaultIntensity = 10;
    public int middleIntensity = 15;
    public int maxIntensity = 30;
    public int finishIntensity = 25;
    public float defaultThreshold = 0.9f;
    public float middleThreshold = 0.7f;
    public float minThreshold = 0.3f;
    public float finishThreshold = 0.2f;
    void Start()
    {
        myTransform = GetComponent<Transform>();
        bloom = postProcessVolume.profile.GetSetting<Bloom>();
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
    }

    public override IEnumerator Activate(Vector3 position, Quaternion quaternion)
    {
        yield return null;
        myTransform.position = position;
        myTransform.rotation = quaternion;
        //画面全体を暗く
        GameObject purple = new("Purple", typeof(Image));
        Image purpleImage = purple.GetComponent<Image>();
        purpleImage.color = new(0.15f, 0, 0.3f, 0);
        purple.transform.SetParent(GameObject.Find("Canvas").transform, false);
        purpleImage.rectTransform.localScale = Vector3.one;
        purpleImage.rectTransform.anchoredPosition = Vector2.zero;
        purpleImage.rectTransform.sizeDelta = new Vector2(1920, 1080);
        StartCoroutine(SmallThunder());
        while (purpleImage.color.a < 0.65f)
        {
            Color temp = purpleImage.color;
            temp.a += 0.65f * Time.deltaTime;
            purpleImage.color = temp;
            yield return null;
        }
        //細かい雷を発生させつつ発光強化
        yield return StartCoroutine(Flash1());
        yield return new WaitForSeconds(0.75f);
        //直前の発光
        StartCoroutine(Flash2());
        GenerateThunder(sphereThunderPrefab, Vector3.zero, Vector3.zero);
        yield return new WaitForSeconds(0.25f);
        GenerateThunder(thunderBeamPrefab, Vector3.zero, Vector3.zero);
        StartCoroutine(ThunderStrikeClose());
        StartCoroutine(ThunderStrikeFar());
        StartCoroutine(AdditionalThunder());
        yield return new WaitForSeconds(3);
        //フィニッシュ
        StartCoroutine(Flash3());
        yield return new WaitForSeconds(0.6f);
        GenerateThunderGlobal(thunderStrikePrefab, new(0, 50, 5), new(0, 0, -90));
        yield return new WaitForSeconds(0.4f);
        Destroy(purple);
        StartCoroutine(SmallThunder());
        yield return new WaitForSeconds(3.2f);
        Destroy(gameObject);
    }
    private IEnumerator SmallThunder()
    {
        for (int i = 0; i < 15; i++)
        {
            int x = UnityEngine.Random.Range(-8, 8);
            int y = UnityEngine.Random.Range(-5, 5);
            int z = UnityEngine.Random.Range(-3, 10);
            GenerateThunderGlobal(smallThunderPrefab, new(x, y, z), Vector3.zero);
            yield return new WaitForSeconds(0.2f);
        }
    }
    private IEnumerator ThunderStrikeClose()
    {
        float t = 0;
        while (true)
        {
            int x = UnityEngine.Random.Range(-35, 35);
            int z = UnityEngine.Random.Range(35, 70);
            float interval = UnityEngine.Random.Range(0.5f, 0.8f);
            t += interval;
            if (t >= 3.5f)
            {
                break;
            }
            yield return new WaitForSeconds(interval);
            GenerateThunderGlobal(thunderStrikePrefab, new(x, 50, z), new(0, 0, -90));
        }
    }
    private IEnumerator ThunderStrikeFar()
    {
        float t = 0;
        while (true)
        {
            int x = UnityEngine.Random.Range(-100, 100);
            int z = UnityEngine.Random.Range(70, 95);
            float interval = UnityEngine.Random.Range(0.4f, 0.6f);
            t += interval;
            if (t >= 3.5f)
            {
                break;
            }
            yield return new WaitForSeconds(interval);
            GenerateThunderGlobal(thunderStrikePrefab, new(x, 50, z), new(0, 0, -90));
        }
    }

    private IEnumerator AdditionalThunder()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject t1 = GenerateThunder(additionalThunderPrefab, new(20, 0, -50), new(0, -90, 0));
        yield return new WaitForSeconds(0.8f);
        GameObject t2 = GenerateThunder(additionalThunderPrefab, new(20, 50, 0), new(0, 0, -90));
        yield return new WaitForSeconds(0.8f);
        GameObject t3 = GenerateThunder(additionalThunderPrefab, new(0, 40, 0), new(0, 0, -45));
        yield return new WaitForSeconds(1);
        Destroy(t1); Destroy(t2); Destroy(t3);
    }
    private IEnumerator Flash1()
    {
        //発動直後
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
        //発射の瞬間の特大フラッシュ(0.25秒*2)
        while (bloom.intensity < maxIntensity)
        {
            bloom.intensity.value += (maxIntensity - middleIntensity) * Time.deltaTime * 4;
            bloom.threshold.value -= (middleThreshold - minThreshold) * Time.deltaTime * 4;
            yield return null;
        }
        bloom.intensity.value = maxIntensity;
        bloom.threshold.value = minThreshold;
        yield return null;
        while (bloom.intensity > middleIntensity)
        {
            bloom.intensity.value -= (maxIntensity - middleIntensity) * Time.deltaTime * 4;
            bloom.threshold.value += (middleThreshold - minThreshold) * Time.deltaTime * 4;
            yield return null;
        }
        bloom.intensity.value = middleIntensity;
        bloom.threshold.value = middleThreshold;
    }
    private IEnumerator Flash3()
    {
        //最後のフラッシュ(1 + 2秒)
        while (bloom.intensity < finishIntensity)
        {
            bloom.intensity.value += (finishIntensity - middleIntensity) * Time.deltaTime;
            bloom.threshold.value -= (middleThreshold - finishThreshold) * Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = finishIntensity;
        bloom.threshold.value = finishThreshold;
        while (bloom.intensity > defaultIntensity)
        {
            bloom.intensity.value -= (finishIntensity - defaultIntensity) * Time.deltaTime / 2;
            bloom.threshold.value += (defaultThreshold - finishThreshold) * Time.deltaTime / 2;
            yield return null;
        }
        bloom.intensity.value = defaultIntensity;
        bloom.threshold.value = defaultThreshold;
    }
    private GameObject GenerateThunder(GameObject prefab, Vector3 pos, Vector3 rot)
    {
        GameObject thunder = Instantiate(prefab, myTransform);
        thunder.transform.localPosition = pos;
        thunder.transform.localRotation = Quaternion.Euler(rot);
        return thunder;
    }
    private GameObject GenerateThunderGlobal(GameObject prefab, Vector3 pos, Vector3 rot)
    {
        GameObject thunder = Instantiate(prefab);
        thunder.transform.position = pos;
        thunder.transform.rotation = Quaternion.Euler(rot);
        return thunder;
    }
}