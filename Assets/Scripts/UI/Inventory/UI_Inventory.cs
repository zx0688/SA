using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{

    public UI_InventoryTooltip tooltip;
    protected UI_InventoryItem[] items;

    protected virtual void Awake()
    {

        items = GetComponentsInChildren<UI_InventoryItem>();

        foreach (UI_InventoryItem item in items)
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

    protected virtual void UpdateList()
    {

        if (!Services.isInited)
            return;

        List<ItemVO> items = new List<ItemVO>(Services.Player.playerVO.items);
        //List<ItemVO> items = new List<ItemVO>();
        //for(int i = 0; i < 32; i++)
        //    items.Add(new ItemVO(2, 5));

        PageSwiper p = GetComponentInChildren<PageSwiper>();
        p.UpdateData(items);
    }
}