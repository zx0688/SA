using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

public class UI_Character : MonoBehaviour
{
    // Start is called before the first frame update

    //public UI_TickChildren tick;
    public UI_SkillItem[] skillItems;
    public UI_SkillItem[] effectItems;

    void Awake()
    {
        skillItems = transform.Find("SpecialPanel").GetComponentsInChildren<UI_SkillItem>();
        effectItems = transform.Find("EffectPanel").GetComponentsInChildren<UI_SkillItem>();
    }

    void Start()
    {
        //tooltip.HideTooltip ();
    }
    void OnEnable()
    {
        UpdateList();
    }

    public void UpdateList()
    {

        if (!Services.isInited)
            return;

        //List<SkillVO> skills = Services.Player.playerVO.skills;
        //int time = GameTime.Get ();

        for (int i = 0; i < skillItems.Length; i++)
        {
            UI_SkillItem skillItem = skillItems[i];
            SkillVO skillVO = null;//Services.Player.skillHandler.GetVO(skillItem.id, skillItem.type);
            skillItem.SetItem(skillVO);
        }

        List<SkillVO> effects = null;//Services.Player.skillHandler.GetListVOByType(2);
        effects = effects.OrderBy(x => x.activated).ToList();
        for (int i = 0; i < effectItems.Length; i++)
        {
            UI_SkillItem effectItem = effectItems[i];
            effectItem.SetItem(i < effects.Count ? effects[i] : null);
        }

        //tick.UpdateTickList ();

    }
}