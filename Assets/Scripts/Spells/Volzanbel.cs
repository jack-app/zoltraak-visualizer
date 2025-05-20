using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Volzanbel : SpellEffectBase
{
    private Transform myTransform;
    [SerializeField] private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    [SerializeField] private GameObject beam;
    [SerializeField] private GameObject globalParticle;
    [SerializeField] private GameObject flashPrefab;
    [SerializeField] private GameObject firePrefab;
    public int defaultIntensity = 10;
    public int middleIntensity = 20;
    public int maxIntensity = 30;
    public int finishIntensity = 25;
    public float defaultThreshold = 0.9f;
    public float middleThreshold = 0.6f;
    public float minThreshold = 0.3f;
    public float finishThreshold = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        bloom = postProcessVolume.profile.GetSetting<Bloom>();
        beam.SetActive(false);
        globalParticle.SetActive(false);
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
    }

    public override IEnumerator Activate(Vector3 position, Quaternion quaternion)
    {
        yield return null;
        myTransform.position = position;
        myTransform.rotation = quaternion;
        //画面全体を赤黒く
        GameObject red = new("Red", typeof(Image));
        Image redImage = red.GetComponent<Image>();
        redImage.color = new(0.5f, 0, 0, 0);
        red.transform.SetParent(GameObject.Find("Canvas").transform, false);
        redImage.rectTransform.localScale = Vector3.one;
        redImage.rectTransform.anchoredPosition = Vector2.zero;
        redImage.rectTransform.sizeDelta = new Vector2(1920, 1080);
        globalParticle.SetActive(true);
        while (redImage.color.a < 0.5f)
        {
            Color temp = redImage.color;
            temp.a += 0.5f * Time.deltaTime;
            redImage.color = temp;
            yield return null;
        }
        //粒子を漂わせつつ発光強化
        yield return StartCoroutine(Flash1());
        yield return new WaitForSeconds(0.75f);
        //直前の発光
        StartCoroutine(Flash2());
        GameObject flash = Instantiate(flashPrefab, myTransform);
        flash.transform.position = new(1, -1, 0);
        flash.transform.rotation = Quaternion.identity;
        flash.transform.localScale = new(2, 1, 1);
        StartCoroutine(ManagePrefab(flash, 0.5f));
        yield return new WaitForSeconds(0.25f);
        //ビーム&更に画面を赤黒く
        beam.SetActive(true);
        redImage.color = new(0.35f, 0, 0, 0.65f);
        //継続的に射線上に噴火
        StartCoroutine(OnLineFire());
        //更にランダム位置でも噴火
        //StartCoroutine(RandomFire());
        yield return new WaitForSeconds(3);
        //フィニッシュ
        StartCoroutine(Flash3());
        yield return new WaitForSeconds(1);
        Destroy(red);
        yield return new WaitForSeconds(3);
        beam.SetActive(false);
        globalParticle.SetActive(false);
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
    private IEnumerator OnLineFire()
    {
        for (int i = 0; i < 11; i++)
        {
            yield return new WaitForSeconds(0.25f);
            int x = UnityEngine.Random.Range(0, 50);
            if (x >= 2)
            {
                GenerateFire(new(x, 0, 0));
            }
        }
        yield return new WaitForSeconds(0.25f);
        GenerateFireGlobal(new(1, 0, -1));
        yield return new WaitForSeconds(0.75f);
        GenerateFlashGlobal(new(1, -1.5f, -1));
    }
    //private IEnumerator RandomFire()
    //{
    //    float t = 0;
    //    while (true)
    //    {
    //        float interval = UnityEngine.Random.Range(0.3f, 0.5f);
    //        t += interval;
    //        if (t > 2.75f)
    //        {
    //            break;
    //        }
    //        yield return new WaitForSeconds(interval);
    //        int x = UnityEngine.Random.Range(-8, 10);
    //        int z = UnityEngine.Random.Range(4, 8);
    //        GenerateFireGlobal(new(x, 0, z));
    //    }
    //}
    private void GenerateFire(Vector3 pos)
    {
        GameObject fire = Instantiate(firePrefab, myTransform);
        fire.transform.localPosition = pos;
        fire.transform.rotation = Quaternion.Euler(-90, 0, 0);
        StartCoroutine(ManagePrefab(fire, 1));
    }
    private void GenerateFireGlobal(Vector3 pos)
    {
        GameObject fire = Instantiate(firePrefab);
        fire.transform.position = pos;
        fire.transform.rotation = Quaternion.Euler(-90, 0, 0);
        StartCoroutine(ManagePrefab(fire, 1));
    }
    //private void GenerateFlash(Vector3 pos)
    //{
    //    GameObject flash = Instantiate(flashPrefab, myTransform);
    //    flash.transform.localPosition = pos;
    //    flash.transform.rotation = Quaternion.identity;
    //    StartCoroutine(ManagePrefab(flash, 0.5f));
    //}
    private void GenerateFlashGlobal(Vector3 pos)
    {
        GameObject flash = Instantiate(flashPrefab);
        flash.transform.position = pos;
        flash.transform.rotation = Quaternion.identity;
        StartCoroutine(ManagePrefab(flash, 0.5f));
    }
    private IEnumerator ManagePrefab(GameObject prefab, float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(prefab);
    }
}