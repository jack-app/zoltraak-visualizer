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

    private const long dataSize = sizeof(double) * 6;
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor accessor;
    private string joyconAbsolutePath;
    private float imageWidth = 1920f;
    private float imageHeight = 1080f;
    private float pixelToUnit = 0.01f;
    public string positionMmapPath = "C:/Users/{ユーザー名}/mmap.txt"; // パスは適宜変更する。
    private MemoryMappedFile positionMmf;
    private MemoryMappedViewAccessor positionAccessor;
    private Vector3 currentPosition = Vector3.zero;

    //呪文検出関係
    public string spellDetectMmapPath = "\"C:/Users/{ユーザー名}/detect.txt\"";
    public string spellsMmapPath = "\"C:/Users/{ユーザー名}/spells.txt\"";
    private MemoryMappedFile detectMmf;
    private MemoryMappedViewAccessor detectAccessor;
    private MemoryMappedFile spellsMmf;
    private MemoryMappedViewAccessor spellsAccessor;
    private int detectInterval = 5;

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
            Debug.LogError("共有メモリファイルが見つかりません: " + positionMmapPath);
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

        //呪文検出関係
        if (!File.Exists(spellDetectMmapPath))
        {
            Debug.LogError("共有メモリファイルが見つかりません: " + spellDetectMmapPath);
            return;
        }

        try
        {
            detectMmf = MemoryMappedFile.CreateFromFile(spellDetectMmapPath, FileMode.Open, null);
            detectAccessor = detectMmf.CreateViewAccessor(0, 1, MemoryMappedFileAccess.ReadWrite);
        }
        catch (Exception e)
        {
            Debug.LogError("共有メモリファイルの読み込みに失敗: " + e.Message);
        }

        if (!File.Exists(spellsMmapPath))
        {
            Debug.LogError("共有メモリファイルが見つかりません: " + spellsMmapPath);
            return;
        }

        try
        {
            spellsMmf = MemoryMappedFile.CreateFromFile(spellsMmapPath, FileMode.Open, null);
            spellsAccessor = spellsMmf.CreateViewAccessor(0, 2, MemoryMappedFileAccess.ReadWrite);
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
        detectInterval--;
        if (detectInterval == 0)
        {
            SpellDetect();
            detectInterval = 5;
        }
        //ゾルトラークデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Zoltraak, GetPosition(), GetRotation()));
        }
        //レイルザイデンデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Railzaiden, GetPosition(), GetRotation()));
        }
        //カタストラーヴィアデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Catastlavia, GetPosition(), GetRotation()));
        }
        //ヴォルザンベルデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Volzanbel, GetPosition(), GetRotation()));
        }
        //ジュドラジルムデバッグ用
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Judolazirum, GetPosition(), GetRotation()));
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
            return currentPosition;
        }

        // もしクリックされ呼ばれた場合は、そのクリック場所をcurrentPositionとして保存する
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            float zDistance = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zDistance));
            currentPosition = new Vector3(worldPoint.x, worldPoint.y, 0f);
            Debug.Log("クリック位置を currentPosition に設定: " + currentPosition);
            return currentPosition;
        }

        try
        {
            double[] imageCoords = new double[6];
            for (int i = 0; i < 6; i++)
            {
                imageCoords[i] = positionAccessor.ReadDouble(i * sizeof(double));
            }

            // currentPosition はすでに Unity 座標系で保持されている
            Vector3 bestCandidate = currentPosition;
            double bestDist = double.MaxValue;
            // Python 座標系 (ピクセル単位) を Unity 座標系に変換する関数
            System.Func<double, double, Vector3> toUnity = (px, py) =>
            {
                float uX = (float)((px - (imageWidth / 2.0)) * pixelToUnit);
                float uY = (float)(((imageHeight / 2.0) - py) * pixelToUnit);
                return new Vector3(uX, uY, 0f);
            };

            for (int idx = 0; idx < 3; idx++)
            {
                double ix = imageCoords[2 * idx];
                double iy = imageCoords[2 * idx + 1];

                // (0,0) は候補点ではなく、mmapのメモリが決まっている関係上、枠が足りなかった場合に補填する為の値なため、(0,0)は除外して考える
                if (ix == 0.0 && iy == 0.0)
                    continue;

                Vector3 candidateUnity = toUnity(ix, iy);
                double d = Vector3.SqrMagnitude(candidateUnity - currentPosition);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestCandidate = candidateUnity;
                }
            }
            if (bestDist < double.MaxValue)
            {
                currentPosition = bestCandidate;
            }

            Debug.Log("座標系: " + currentPosition);
            return currentPosition;
        }
        catch (Exception e)
        {
            Debug.LogWarning("共有メモリからの位置情報読み取り失敗: " + e.Message);
            return currentPosition;
        }
    }

    //呪文検出関係
    private void SpellDetect()
    {
        if (detectAccessor == null || spellsAccessor == null) return;
        byte detect = 0;
        try
        {
            detect = detectAccessor.ReadByte(0);
        }
        catch (Exception e)
        {
            Debug.LogWarning("共有メモリからの読み取り失敗: " + e.Message);
        }
        //何かしらの呪文を検知
        if (detect == 1)
        {
            short spell = 0;
            try
            {
                spell = spellsAccessor.ReadInt16(0);
            }
            catch (Exception e)
            {
                Debug.LogWarning("共有メモリからの読み取り失敗: " + e.Message);
            }
            //どの呪文かを選択
            switch (spell)
            {
                case 1:
                    StartCoroutine(spellEffectManager.OnSpelled(SPELL.Zoltraak, GetPosition(), GetRotation()));
                    break;
                case 2:
                    StartCoroutine(spellEffectManager.OnSpelled(SPELL.Railzaiden, GetPosition(), GetRotation()));
                    break;
                case 3:
                    StartCoroutine(spellEffectManager.OnSpelled(SPELL.Catastlavia, GetPosition(), GetRotation()));
                    break;
                case 4:
                    StartCoroutine(spellEffectManager.OnSpelled(SPELL.Volzanbel, GetPosition(), GetRotation()));
                    break;
                case 5:
                    StartCoroutine(spellEffectManager.OnSpelled(SPELL.Judolazirum, GetPosition(), GetRotation()));
                    break;
                default:
                    break;
            }
            //mmapの値を0へ戻す
            try
            {
                detectAccessor.Write(0, (byte)0);
            }
            catch (Exception e)
            {
                Debug.LogWarning("共有メモリへの書き込み失敗: " + e.Message);
            }
            try
            {
                spellsAccessor.Write(0, (short)0);
            }
            catch (Exception e)
            {
                Debug.LogWarning("共有メモリへの書き込み失敗: " + e.Message);
            }
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
        if (detectAccessor != null)
        {
            detectAccessor.Dispose();
            detectAccessor = null;
        }
        if (spellsAccessor != null)
        {
            spellsAccessor.Dispose();
            spellsAccessor = null;
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
        if (detectAccessor != null)
        {
            detectAccessor.Dispose();
            detectAccessor = null;
        }
        if (spellsAccessor != null)
        {
            spellsAccessor.Dispose();
            spellsAccessor = null;
        }
    }
}
