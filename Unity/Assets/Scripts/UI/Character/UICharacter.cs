using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UICharacter : ServiceBehaviour, IPage
{
    [SerializeField] private UISkillItem[] skills;
    [SerializeField] private UI_SkillTooltip tooltip;
    [SerializeField] private Text swipeCount;
    [SerializeField] private Image energyIcon;


    //     void Awake()
    //     {
    //         // foreach (UI_SkillItem item in skills)
    //         // {
    //         //     item.SetTooltip(tooltip);
    //         // }
    // 
    //         //        gameObject.SetActive(false);
    //         //      tooltip.HideTooltip();
    //     }

    //     protected override void OnServicesInited()
    //     {
    //         base.OnServicesInited();
    // 
    //     }

    public void UpdateData()
    {
        if (!Services.isInited)
            return;

        energyIcon.LoadItemIcon("6");
        swipeCount.text = Services.Player.Profile.SwipeCount.ToString();

        for (int i = 0; i < skills.Length; i++)
        {
            UISkillItem item = skills[i];
            string id = Services.Player.Profile.Skills[item.Slot];

            if (id != null && Services.Meta.Game.Skills.TryGetValue(id, out SkillMeta meta))
                item.SetItem(meta);
            else
                item.SetItem(null);
        }
    }

    public void Show()
    {
        //tooltip?.HideTooltip();
        UpdateData();
    }

    public string GetName() => "Персонаж";
    public GameObject GetGameObject() => gameObject;

    public void Hide()
    {

    }
}