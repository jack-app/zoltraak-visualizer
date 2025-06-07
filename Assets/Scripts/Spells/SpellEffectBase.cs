using System;
using System.Collections;
using UnityEngine;

public abstract class SpellEffectBase : MonoBehaviour
{
    protected ScreenShotManager screenShotManager;
    private void Awake()
    {
        try
        {
            screenShotManager = GameObject.Find("ScreenShotManager").GetComponent<ScreenShotManager>();
        }
        catch (Exception e)
        {
            Debug.LogError("There is no ScreenShotManager: " + e.Message);
        }
    }
    public abstract IEnumerator Activate(Vector3 position, Quaternion quaternion);
}
