using UnityEngine;
using TMPro;

public class JoyConTextDisplay : MonoBehaviour
{
    public TextMeshProUGUI textUI; // Inspector で割り当てる

    void Update()
    {
        // var reader = JoyConReader.Instance;
        // if (reader == null || textUI == null) return;

        // float x = reader.DirectionX;
        // float y = reader.DirectionY;
        // float z = reader.DirectionZ;

        // textUI.text = $"Direction:\nX: {x:F2}\nY: {y:F2}\nZ: {z:F2}";
    }
}
