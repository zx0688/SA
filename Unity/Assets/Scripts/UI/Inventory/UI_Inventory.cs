using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UI_Inventory : MonoBehaviour, IPage
{
    [SerializeField] private UI_InventoryTooltip _tooltip;

    protected UI_InventoryItem[] _items;
    private PageSwiper _swiper;

    void Awake()
    {
        _items = GetComponentsInChildren<UI_InventoryItem>();

        foreach (UI_InventoryItem item in _items)
        {
            item.SetTooltip(_tooltip);
        }

        _swiper = GetComponentInChildren<PageSwiper>();
        gameObject.SetActive(false);
        _tooltip.HideTooltip();
    }

    void UpdateList()
    {
        if (!Services.isInited)
            return;

        List<ItemVO> items = Services.Player.GetPlayerVO.Items;

        _swiper.UpdateData(items);
    }

    public void Show()
    {
        _tooltip.HideTooltip();
        UpdateList();
    }

    public string GetName() => "Инвентарь";
    public GameObject GetGameObject() => gameObject;

    public void Hide()
    {

    }
}