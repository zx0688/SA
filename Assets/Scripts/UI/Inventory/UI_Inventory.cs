using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private UI_InventoryTooltip _tooltip;
    [SerializeField] private PagePanel _pagePanel;

    protected UI_InventoryItem[] _items;

    protected virtual void Awake()
    {
        _items = GetComponentsInChildren<UI_InventoryItem>();

        foreach (UI_InventoryItem item in _items)
        {
            item.SetTooltip(_tooltip);
        }
    }

    void Start()
    {
        _tooltip?.HideTooltip();
    }

    private void OnPageChanged()
    {
        //int currentPage = swiper.GetCurrentPage();
    }

    void OnEnable()
    {
        UpdateList();

    }

    void OnDisable()
    {
        _pagePanel.Hide();
    }

    protected virtual void UpdateList()
    {

        if (!Services.isInited)
            return;

        _pagePanel.Show();

        //List<ItemVO> items = new List<ItemVO>(Services.Player.playerVO.items);

        List<ItemVO> items = new List<ItemVO>();
        //for(int i = 0; i < 32; i++)
        //    items.Add(new ItemVO(2, 5));

        PageSwiper p = GetComponentInChildren<PageSwiper>();
        p.UpdateData(items);
    }
}