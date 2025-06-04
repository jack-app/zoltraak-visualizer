using UnityEngine.Networking;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScreenShotManager : MonoBehaviour
{
    private List<string> screenshotUuids = new List<string>();
    private string uploadUrl = "https://zoltraak-img.nac-39.com/images";
    public string ScreenshotSavePath
    {
        get
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // Windows 環境では persistentDataPath の下に "zoltraak" フォルダを作成
            string winPath = Path.Combine(Application.persistentDataPath, "zoltraak");
            return winPath + Path.DirectorySeparatorChar;
#else
            // macOS / Linux 環境では /tmp/zoltraak/ を使用
            return "/tmp/zoltraak/";
#endif
        }
    }

    // Update is called once per frame
    private float flashDuration = 0.1f;
    private float flashTimer = 0f;
    private bool isFlashing = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CaptureAndFlash();
        }

        if (IsUploadShortcutPressed())
        {
            Debug.Log("Uploading all screenshots...");
            StartCoroutine(UploadAllScreenshotsAsync());
        }

        if (isFlashing)
        {
            flashTimer -= Time.unscaledDeltaTime;
            if (flashTimer <= 0f)
            {
                isFlashing = false;
            }
        }
    }

    IEnumerator FlashAfterDelay()
    {
        yield return new WaitForEndOfFrame(); // スクショが終わるまで待つ
        flashTimer = flashDuration;
        isFlashing = true;
    }

    void OnGUI()
    {
        if (isFlashing)
        {
            Color prevColor = GUI.color;
            GUI.color = new Color(1, 1, 1, 0.3f); // semi-transparent white
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = prevColor;
        }
    }

    IEnumerator CaptureAndRegister()
    {
        string uuid = Guid.NewGuid().ToString();
        string path = ScreenshotSavePath + uuid + ".png";
        Directory.CreateDirectory(ScreenshotSavePath);
        ScreenCapture.CaptureScreenshot(path);

        // 次フレーム末尾まで待ってファイル書き込み完了を期待
        yield return new WaitForEndOfFrame();
        screenshotUuids.Add(uuid);
    }

    void TakeScreenshot()
    {
        StartCoroutine(CaptureAndRegister());
    }
    public void ClearScreenshotUuids()
    {
        screenshotUuids.Clear();
    }
    public void CaptureAndFlash()
    {
        TakeScreenshot();
        StartCoroutine(FlashAfterDelay());
    }

    public IEnumerator UploadAllScreenshotsAsync()
    {
        if (screenshotUuids.Count == 0)
        {
            Debug.LogWarning("No screenshots to upload.");
            yield break;
        }
        string userId = Guid.NewGuid().ToString();
        Debug.Log("Uploading screenshots for user: " + userId);
        foreach (string uuid in screenshotUuids)
        {
            string filePath = Path.Combine(ScreenshotSavePath, uuid + ".png");
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("Screenshot file not found: " + filePath);
                continue;
            }

            byte[] fileData = File.ReadAllBytes(filePath);
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", fileData, uuid + ".png", "image/png");
            form.AddField("userid", userId);

            using (UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Upload failed for " + uuid + ": " + www.error);
                }
                else
                {
                    Debug.Log("Upload succeeded for " + uuid + ": " + www.downloadHandler.text);
                }
            }
        }
        screenshotUuids.Clear(); // Clear the list after upload
        Debug.Log("All screenshots uploaded and cleared from the list.");
    }
    private bool IsUploadShortcutPressed()
    {
        // macOS: Command + LeftShift + U
        if ((Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            && Input.GetKey(KeyCode.LeftCommand) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
        {
            return true;
        }
        // Windows: Control + LeftShift + U
        if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
        {
            return true;
        }
        return false;
    }
}
