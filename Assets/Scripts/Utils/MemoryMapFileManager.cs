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
        //実装が完了したら書き換えてください
        return Quaternion.identity;
    }
    private Vector3 GetPosition()
    {
        return Vector3.zero;
    }
}