using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryMapFileManager : MonoBehaviour
{
    [SerializeField] SpellEffectManager spellEffectManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ゾルトラークで実験する用
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Zoltraak, GetPosition(), Quaternion.Euler(0, -45, 0)));
        }
        //レイルザイデンで実験する用
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Railzaiden, GetPosition(), GetRotation()));
        }
        //カタストラーヴィアで実験する用
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(spellEffectManager.OnSpelled(SPELL.Catastlavia, GetPosition(), Quaternion.Euler(0, -45, 0)));
        }
    }

    private Quaternion GetRotation()
    {
        //実装が完了したら書き換えてください
        return Quaternion.identity;
    }
    private Vector3 GetPosition()
    {
        return Vector3.zero;
    }
}