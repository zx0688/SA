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


    void Awake()
    {
        foreach (UISkillItem item in skills)
            item.SetTooltip(tooltip);
        tooltip.HideTooltip();
    }

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

        skills.ToList().ForEach(s => s.UpdateItem(s.Slot));
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