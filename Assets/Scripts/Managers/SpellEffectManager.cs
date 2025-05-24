using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectManager : MonoBehaviour
{
    private const string spellPrefabPath = "Prefabs/Spells/";
    private Dictionary<SPELL, string> spellDict = new();
    private bool isSomeSpellActive = false;
    // Start is called before the first frame update
    void Start()
    {
        spellDict.Add(SPELL.Zoltraak, "Zoltraak");
        spellDict.Add(SPELL.Railzaiden, "Railzaiden");
        spellDict.Add(SPELL.Catastlavia, "Catastlavia");
        spellDict.Add(SPELL.Volzanbel, "Volzanbel");
        spellDict.Add(SPELL.Judolazirum, "Judolazirum");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator OnSpelled(SPELL spell, Vector3 position, Quaternion quaternion)
    {
        if (!isSomeSpellActive)
        {
            GameObject spellObject = Instantiate(Resources.Load<GameObject>(spellPrefabPath + spellDict[spell]), position, quaternion);
            isSomeSpellActive = true;
            yield return StartCoroutine(spellObject.GetComponent<SpellEffectBase>().Activate(position, quaternion));
            isSomeSpellActive = false;
        }
    }
}
