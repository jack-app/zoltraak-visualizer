using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectManager : MonoBehaviour
{
    private const string spellPrefabPath = "Prefabs/Spells/";
    private Dictionary<SPELL, string> spellDict = new();
    // Start is called before the first frame update
    void Start()
    {
        spellDict.Add(SPELL.Zoltraak, "Zoltraak");
        spellDict.Add(SPELL.Railzaiden, "Railzaiden");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnSpelled(SPELL spell, Vector3 position, Quaternion quaternion)
    {
        GameObject spellObject = Instantiate(Resources.Load<GameObject>(spellPrefabPath + spellDict[spell]), position, quaternion);
        spellObject.GetComponent<SpellEffectBase>().Activate(position, quaternion);
    }
}
