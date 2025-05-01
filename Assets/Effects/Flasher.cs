using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class Flasher : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    public int defaultIntensity = 10;
    public int circleIntensity = 30;
    public int beamIntensity = 15;
    public int finishIntensity = 25;
    public float defaultThreshold = 0.9f;
    public float circleThreshold = 0.2f;
    public float beamThreshold = 0.7f;
    public float finishThreshold = 0.3f;

    void Start()
    {
        bloom = postProcessVolume.profile.GetSetting<Bloom>();
    }

    //スクリプトから発光を制御(発射の瞬間) 
    public IEnumerator Flash1()
    {
        //0.25秒で発光最大化→0.25秒で収束
        while (bloom.intensity < circleIntensity)
        {
            bloom.intensity.value += (circleIntensity - defaultIntensity) * Time.deltaTime * 4;
            bloom.threshold.value -= (defaultThreshold - circleThreshold) * Time.deltaTime * 4;
            yield return null;
        }
        bloom.intensity.value = circleIntensity;
        bloom.threshold.value = circleThreshold;
        while (bloom.intensity > beamIntensity)
        {
            bloom.intensity.value -= (circleIntensity - beamIntensity) * Time.deltaTime * 4;
            bloom.threshold.value += (beamThreshold - circleThreshold) * Time.deltaTime * 4;
            yield return null;
        }
        bloom.intensity.value = beamIntensity;
        bloom.threshold.value = beamThreshold;
    }
    //スクリプトから発光を制御(発射終了) 
    public IEnumerator Flash2()
    {
        //0.5秒で発光最大化→1秒で収束
        while (bloom.intensity < finishIntensity)
        {
            bloom.intensity.value += (finishIntensity - beamIntensity) * Time.deltaTime * 2;
            bloom.threshold.value -= (beamThreshold - finishThreshold) * Time.deltaTime * 2;
            yield return null;
        }
        bloom.intensity.value = finishIntensity;
        bloom.threshold.value = finishThreshold;
        while (bloom.intensity > defaultIntensity)
        {
            bloom.intensity.value -= (finishIntensity - defaultIntensity) * Time.deltaTime;
            bloom.threshold.value += (defaultThreshold - finishThreshold) * Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = defaultIntensity;
        bloom.threshold.value = defaultThreshold;
    }
}