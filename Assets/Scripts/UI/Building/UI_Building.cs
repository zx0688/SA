using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

public class UI_Building : MonoBehaviour
{

    public UI_BuildingTooltip tooltip;
    protected UI_BuildingItem[] items;

    void Awake()
    {

        items = GetComponentsInChildren<UI_BuildingItem>();

        foreach (UI_BuildingItem item in items)
        {
            item.SetTooltip(tooltip);
        }
    }

    void Start()
    {
        tooltip.HideTooltip();
    }

    private void OnPageChanged()
    {
        //int currentPage = swiper.GetCurrentPage();

    }

    void OnEnable()
    {
        UpdateList();
    }


    protected void UpdateList()
    {

        if (!Services.isInited)
            return;

        List<BuildingVO> items = Services.Player.playerVO.buildings;
        PageSwiper p = GetComponentInChildren<PageSwiper>();
        p.UpdateData(new List<ItemVO>(items));
    }
}