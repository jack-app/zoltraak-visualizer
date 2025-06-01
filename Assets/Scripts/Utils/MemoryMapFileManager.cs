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

    private const int dataSize = 16; // float * 4 = 16 bytes
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor accessor;
    private string joyconAbsolutePath;


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
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //ゾルトラークデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Zoltraak, GetPosition(), Quaternion.Euler(0, -45, 0)));
        }
        //レイルザイデンデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Railzaiden, GetPosition(), GetRotation()));
        }
        //カタストラーヴィアデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Catastlavia, GetPosition(), Quaternion.Euler(0, -45, 0)));
        }
        //ヴォルザンベルデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Volzanbel, GetPosition(), Quaternion.Euler(0, -45, 0)));
        }
        //ジュドラジルムデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Judolazirum, GetPosition(), Quaternion.Euler(0, -45, 0)));
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
    private Vector3 GetPosition()
    {
        // 位置は固定値で返す
        return new Vector3(0, 0, 0);
    }
}