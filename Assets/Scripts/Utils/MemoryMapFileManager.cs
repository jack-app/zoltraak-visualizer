using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.IO.MemoryMappedFiles;

public class MemoryMapFileManager : MonoBehaviour
{
    [SerializeField] SpellEffectManager spellEffectManager;
    // Start is called before the first frame update
    public static MemoryMapFileManager Instance { get; private set; }
    public bool isDebug = false;

    public float RotationX { get; private set; }
    public float RotationY { get; private set; }
    public float RotationZ { get; private set; }
    public float RotationW { get; private set; }
    public double px { get; private set; }
    public double py { get; private set; }

    private const int dataSize = 16; // float * 4 = 16 bytes
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor accessor;
    private string joyconAbsolutePath;
    private float imageWidth = 1920f;
    private float imageHeight = 1080f;
    private float pixelToUnit = 0.01f;
    private string positionMmapPath = "C:/Users/{ユーザー名}/mmap.txt";
    private MemoryMappedFile positionMmf;
    private MemoryMappedViewAccessor positionAccessor;
    private Vector3 currentPosition = Vector3.zero;


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
        joyconAbsolutePath = "/tmp/joycon_direction.dat";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        joyconAbsolutePath = @"C:\Users\YourName\joycon_direction.dat"; // 必要に応じて書き換え
#else
        Debug.LogError("Unsupported platform for JoyConDirectionReader");
        return;
#endif

        if (!File.Exists(joyconAbsolutePath))
        {
            Debug.LogError("共有ファイルが見つかりません: " + joyconAbsolutePath);
            return;
        }

        try
        {
            mmf = MemoryMappedFile.CreateFromFile(joyconAbsolutePath, FileMode.Open, null);
            accessor = mmf.CreateViewAccessor(0, dataSize, MemoryMappedFileAccess.Read);
        }
        catch (Exception e)
        {
            Debug.LogError("共有メモリファイルの読み込みに失敗: " + e.Message);
        }

        if (!File.Exists(positionMmapPath))
        {
            Debug.LogError("共有ファイルが見つかりません: " + positionMmapPath);
            return;
        }

        try
        {
            positionMmf = MemoryMappedFile.CreateFromFile(positionMmapPath, FileMode.Open, null);
            positionAccessor = positionMmf.CreateViewAccessor(0, dataSize, MemoryMappedFileAccess.Read);
        }
        catch (Exception e)
        {
            Debug.LogError("共有メモリファイルの読み込みに失敗: " + e.Message);
        }
    }
    void Start()
    {
        StartCoroutine(PollPositionCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        //ゾルトラークで実験する用
        if (Input.GetMouseButtonDown(0))
        {
            spellEffectManager.OnSpelled(SPELL.Zoltraak, GetPosition(), GetRotation());
        }
        //レイルザイデンで実験する用
        if (Input.GetMouseButtonDown(1))
        {
            spellEffectManager.OnSpelled(SPELL.Railzaiden, GetPosition(), GetRotation());
        }
    }


    private Quaternion GetRotation()
    {
        if (isDebug) return Quaternion.identity;
        if (accessor == null) return Quaternion.identity;

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
        // RotationX/Y/Z/Wは生データなのでUnity上のQuaternionに変換してから使う
        float xDeg = Mathf.Rad2Deg * -RotationX;
        float yDeg = Mathf.Rad2Deg * -RotationZ;
        float zDeg = Mathf.Rad2Deg * RotationY;
        float wDeg = Mathf.Rad2Deg * RotationW;

        Quaternion targetRotation = new Quaternion(xDeg, yDeg, zDeg, wDeg);
        return targetRotation;
    }
    private IEnumerator PollPositionCoroutine()
    {
        while (true)
        {
            Vector3 pos = GetPosition();
            // 5秒ごとに実行
            yield return new WaitForSeconds(5f);
        }
    }
    private Vector3 GetPosition()
    {
        if (positionAccessor == null)
        {
            currentPosition = Vector3.zero;
            return currentPosition;
        }

        try
        {
            px = positionAccessor.ReadDouble(0);
            py = positionAccessor.ReadDouble(8);

            // unityの座標系に合わせる
            float uX = (float)((px - (imageWidth / 2.0)) * pixelToUnit);
            float uY = (float)(((imageHeight / 2.0) - py) * pixelToUnit);
            float uZ = 0f;

            currentPosition = new Vector3(uX, uY, uZ);
            Debug.Log("座標系: " + currentPosition);
            return currentPosition;
        }
        catch (Exception e)
        {
            Debug.LogWarning("共有メモリからの位置情報読み取り失敗: " + e.Message);
            currentPosition = Vector3.zero;
            return currentPosition;
        }
    }

    void OnDisable()
    {
        // 既存の OnDestroy と同じく、アクセサを必ず解放
        if (accessor != null)
        {
            accessor.Dispose();
            accessor = null;
        }
        if (positionAccessor != null)
        {
            positionAccessor.Dispose();
            positionAccessor = null;
        }
    }

    void OnDestroy()
    {
        // 既存の OnDestroy と同じく、アクセサを必ず解放
        if (accessor != null)
        {
            accessor.Dispose();
            accessor = null;
        }
        if (positionAccessor != null)
        {
            positionAccessor.Dispose();
            positionAccessor = null;
        }
    }
}