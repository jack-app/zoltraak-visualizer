using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private ScreenShotManager screenShotManagerScript;

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
        }
    }

}

