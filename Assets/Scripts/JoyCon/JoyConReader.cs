using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using UnityEngine;

public class JoyConReader : MonoBehaviour
{
    public static JoyConReader Instance { get; private set; }

    public float RotationX { get; private set; }
    public float RotationY { get; private set; }
    public float RotationZ { get; private set; }
    public float RotationW { get; private set; }


    private const int dataSize = 16; // float * 4 = 16 bytes
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor accessor;
    private string absolutePath;

    void Awake()
    {
        // Singleton化して他のスクリプトから参照できるようにする
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        // OSによってパスを変更
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_STANDALONE_LINUX
        absolutePath = "/tmp/joycon_direction.dat";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        absolutePath = @"C:\Users\YourName\joycon_direction.dat"; // 必要に応じて書き換え
#else
        Debug.LogError("Unsupported platform for JoyConDirectionReader");
        return;
#endif

        if (!File.Exists(absolutePath))
        {
            Debug.LogError("共有ファイルが見つかりません: " + absolutePath);
            return;
        }

        try
        {
            mmf = MemoryMappedFile.CreateFromFile(absolutePath, FileMode.Open, null);
            accessor = mmf.CreateViewAccessor(0, dataSize, MemoryMappedFileAccess.Read);
        }
        catch (Exception e)
        {
            Debug.LogError("共有メモリファイルの読み込みに失敗: " + e.Message);
        }
    }

    void Update()
    {
        if (accessor == null) return;

        try
        {
            RotationX = accessor.ReadSingle(0);
            RotationY = accessor.ReadSingle(4);
            RotationZ = accessor.ReadSingle(8);
            RotationW = accessor.ReadSingle(12);
        }
        catch (Exception e)
        {
            Debug.LogWarning("共有メモリからの読み取り失敗: " + e.Message);
        }
    }

    void OnDestroy()
    {
        accessor?.Dispose();
        mmf?.Dispose();
    }
}