using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;

public class GamepadManager : MonoBehaviour
{
    private Gamepad gamepad;
    private List<string> inputList = new List<string>
    {
        "Take SS",
        "Send SS",
        "Zoltraak",
        "Railzaiden",
        "Catastlavia",
        "Volzanbel",
        "Judolazirum",
    };
    private Dictionary<int, bool> isButtonPressKept = new Dictionary<int, bool> {};

    private GameObject ScreenShotManager;
    private GameObject MemoryMapFileHandler;

    private ScreenShotManager screenShotManagerScript;
    private MemoryMapFileManager memoryMapFileManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < inputList.Count; i++)
        {
            isButtonPressKept.Add(i, false);
        }

        ScreenShotManager = GameObject.Find("ScreenShotManager");
        if (ScreenShotManager == null)
        {
            Debug.LogError("ScreenShotManager not found in the scene.");
        } else {
            screenShotManagerScript = ScreenShotManager.GetComponent<ScreenShotManager>();
        }
        Debug.Log("GamepadManager started");

        MemoryMapFileHandler = GameObject.Find("MemoryMapFileHandler");
        if (MemoryMapFileHandler == null)
        {
            Debug.LogError("MemoryMapFileHandler not found in the scene.");
        } else {
            memoryMapFileManagerScript = MemoryMapFileHandler.GetComponent<MemoryMapFileManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // gamepad = Gamepad.current;
        // if (gamepad == null) return;

        // if (gamepad.button2.wasPressedThisFrame)
        // {
        //     Debug.Log("A button pressed");
        // }
        int pressedButton = -1;
        for (int i = 0; i < inputList.Count; i++)
        {
            if (Input.GetAxis(inputList[i]) > 0 && !isButtonPressKept[i])
            {
                isButtonPressKept[i] = true;
                pressedButton = i;
                break;
            }
            else if (Input.GetAxis(inputList[i]) == 0)
            {
                isButtonPressKept[i] = false;
            }
        }
        if (pressedButton != -1)
        {
            Debug.Log($"Button {inputList[pressedButton]} pressed");
            if (pressedButton == 0)
            {
                screenShotManagerScript.CaptureAndFlash();
            } else if (pressedButton == 1)
            {
                StartCoroutine(screenShotManagerScript.UploadAllScreenshotsAsync());
            }
            else {  // spells
                try
                {
                    using (var mmf = MemoryMappedFile.CreateFromFile(memoryMapFileManagerScript.spellsMmapPath, FileMode.Open, null))
                    {
                        using (var accessor = mmf.CreateViewAccessor(0, 2, MemoryMappedFileAccess.ReadWrite))
                        {
                            accessor.Write(0, (byte)(pressedButton - 1));  // button 2 -> Zoltraak (id 1)
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"mmap への書き込みに失敗: {ex.Message}");
                }
                try
                {
                    using (var mmf = MemoryMappedFile.CreateFromFile(memoryMapFileManagerScript.spellDetectMmapPath, FileMode.Open, null))
                    {
                        using (var accessor = mmf.CreateViewAccessor(0, 1, MemoryMappedFileAccess.ReadWrite))
                        {
                            accessor.Write(0, (byte)1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"mmap への書き込みに失敗: {ex.Message}");
                }
            }
        }
    }

}

