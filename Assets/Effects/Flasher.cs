using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class Flasher : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    private const int defaultIntensity = 10;
    private const int maxIntensity = 20;
    private const float defaultThreshold = 0.9f;
    private const float minThreshold = 0.3f;

    void Start()
    {
        bloom = postProcessVolume.profile.GetSetting<Bloom>();
    }

    //スクリプトから発光を制御(発射の瞬間) 
    public IEnumerator Flash1()
    {
        //0.25秒で発光最大化→0.25秒で収束
        //魔法陣だけでは明るさが足りない気がしたため最後の係数で微調整
        while (bloom.intensity < maxIntensity)
        {
            bloom.intensity.value += (maxIntensity - defaultIntensity) * Time.deltaTime * 4 * 1.25f;
            bloom.threshold.value -= (defaultThreshold - minThreshold) * Time.deltaTime * 4 * 0.75f;
            yield return null;
        }
        bloom.intensity.value = maxIntensity * 1.25f;
        bloom.threshold.value = minThreshold * 0.75f;
        while (bloom.intensity > defaultIntensity)
        {
            bloom.intensity.value -= (maxIntensity - defaultIntensity) * Time.deltaTime * 4 * 1.25f;
            bloom.threshold.value += (defaultThreshold - minThreshold) * Time.deltaTime * 4 * 0.75f;
            yield return null;
        }
        bloom.intensity.value = defaultIntensity;
        bloom.threshold.value = defaultThreshold;
    }
    //スクリプトから発光を制御(発射終了) 
    public IEnumerator Flash2()
    {
        //0.5秒で発光最大化→1秒で収束
        while (bloom.intensity < maxIntensity)
        {
            bloom.intensity.value += (maxIntensity - defaultIntensity) * Time.deltaTime * 2;
            bloom.threshold.value -= (defaultThreshold - minThreshold) * Time.deltaTime * 2;
            yield return null;
        }
        bloom.intensity.value = maxIntensity;
        bloom.threshold.value = minThreshold;
        while (bloom.intensity > defaultIntensity)
        {
            bloom.intensity.value -= (maxIntensity - defaultIntensity) * Time.deltaTime;
            bloom.threshold.value += (defaultThreshold - minThreshold) * Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = defaultIntensity;
        bloom.threshold.value = defaultThreshold;
    }
}