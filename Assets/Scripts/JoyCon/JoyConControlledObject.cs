using UnityEngine;

public class JoyConControlledObject : MonoBehaviour
{
    // 回転の平滑化係数（動きが激しいときに調整）
    [Range(0f, 1f)]
    public float smoothing = 0.2f;

    private Quaternion currentRotation;

    void Start()
    {
        currentRotation = transform.rotation;
    }

    void Update()
    {
        var reader = JoyConDirectionReader.Instance;
        if (reader == null) return;

        Vector3 direction = new Vector3(reader.DirectionX, reader.DirectionY, reader.DirectionZ);


        if (direction.magnitude < 0.001f) return; // 無効な方向をスキップ

        transform.forward = direction;
        // // 単位ベクトル化して回転へ変換
        // Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

        // // スムーズに回転（滑らかさ調整）
        // currentRotation = Quaternion.Slerp(currentRotation, targetRotation, smoothing);
        // transform.rotation = currentRotation;
    }
}
