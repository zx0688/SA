using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UI_EffectPanel : MonoBehaviour
{

    private UI_SkillItem[] items;

    void Awake()
    {
        items = GetComponentsInChildren<UI_SkillItem>();
    }

    public void UpdateItems(List<SkillVO> skills)
    {

        for (int i = 0; i < items.Length; i++)
        {
            items[i].Clear();
            items[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < skills.Count && i < items.Length; i++)
        {
            //items[i].gameObject.SetActive(true);
            //items[i].SetItem(skills[i]);
        }

    }

}