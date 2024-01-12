using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UIInventory : MonoBehaviour, IPage
{
    [SerializeField] private UIInventoryTooltip tooltip;
    [SerializeField] private PageSwiper swiper;

    protected UIInventoryItem[] items;


    void Awake()
    {
        items = GetComponentsInChildren<UIInventoryItem>();

        foreach (UIInventoryItem item in items)
        {
            item.SetTooltip(tooltip);
        }

        gameObject.SetActive(false);

        //if (tooltip != null)
        //    tooltip.HideTooltip();
    }

    void UpdateList()
    {
        if (!Services.isInited)
            return;

        //List<ItemData> items = Services.Player.Profile.Items;

        swiper.UpdateData(Services.Player.Profile.Items.Values.ToList());
    }

    public void Show()
    {
        //tooltip.HideTooltip();
        UpdateList();
    }

    public string GetName() => "Инвентарь";
    public GameObject GetGameObject() => gameObject;

    public void Hide()
    {

    }
}