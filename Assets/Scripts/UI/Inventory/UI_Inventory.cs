using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private UI_InventoryTooltip _tooltip;

    protected UI_InventoryItem[] _items;
    private PageSwiper _swiper;

    protected virtual void Awake()
    {
        _items = GetComponentsInChildren<UI_InventoryItem>();

        foreach (UI_InventoryItem item in _items)
        {
            item.SetTooltip(_tooltip);
        }

        _swiper = GetComponentInChildren<PageSwiper>();
    }

    void OnEnable()
    {
        _tooltip?.HideTooltip();
        UpdateList();
    }

    protected virtual void UpdateList()
    {
        if (!Services.isInited)
            return;

        List<ItemVO> items = new List<ItemVO>();

        for (int i = 0; i < 32; i++)
            items.Add(new ItemVO(2, i));

        _swiper.UpdateData(items);
    }
}