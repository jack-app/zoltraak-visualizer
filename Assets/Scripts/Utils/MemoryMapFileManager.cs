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
        //実装が完了したら書き換えてください
        return Quaternion.identity;
    }
    private Vector3 GetPosition()
    {
        return Vector3.zero;
    }
}