using System;
using UnityEngine;


public class JoyConControlledObject : MonoBehaviour
{
    [Range(0f, 1f)]
    public float smoothing = 0.2f;

    private Quaternion currentRotation = new Quaternion(0, 0, 0, 1);

    void Start()
    {
        currentRotation = transform.rotation;
    }

    void Update()
    {
        var reader = MemoryMapFileManager.Instance;
        if (reader == null)
        {
            Debug.LogWarning("MemoryMapFileManagerが見つかりません。MemoryMapFileManagerコンポーネントがシーンに配置されているか確認してください。");
            return;
        }

        // ラジアン → 度に変換
        float xDeg = Mathf.Rad2Deg * -reader.RotationX;
        float yDeg = Mathf.Rad2Deg * -reader.RotationZ;
        float zDeg = Mathf.Rad2Deg * reader.RotationY;
        float wDeg = Mathf.Rad2Deg * reader.RotationW;

        Quaternion targetRotation = new Quaternion(xDeg, yDeg, zDeg, wDeg);
        currentRotation = Quaternion.Slerp(currentRotation, targetRotation, smoothing);

        transform.rotation = currentRotation;
    }
}