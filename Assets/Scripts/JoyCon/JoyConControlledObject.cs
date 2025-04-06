using UnityEngine;

public class JoyConControlledObject : MonoBehaviour
{
    [Range(0f, 1f)]
    public float smoothing = 0.2f;

    private Quaternion currentRotation;

    void Start()
    {
        currentRotation = transform.rotation;
    }

    void Update()
    {
        var reader = JoyConReader.Instance;
        if (reader == null) return;

        // ラジアン → 度に変換
        float xDeg = Mathf.Rad2Deg * reader.RotationX;
        float yDeg = Mathf.Rad2Deg * reader.RotationY;
        float zDeg = Mathf.Rad2Deg * -reader.RotationZ;  // 左手系変換：Zを反転

        Quaternion targetRotation = Quaternion.Euler(xDeg, yDeg, zDeg);
        currentRotation = Quaternion.Slerp(currentRotation, targetRotation, smoothing);

        transform.rotation = targetRotation;
    }
}
