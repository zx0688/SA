using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

public class UI_Building : MonoBehaviour, IPage
{

    [SerializeField] private UI_BuildingTooltip _tooltip;
    protected UI_BuildingItem[] items;
    private PageSwiper _swiper;
    private DungeonBuilder _builder;

    void Awake()
    {

        items = GetComponentsInChildren<UI_BuildingItem>();

        foreach (UI_BuildingItem item in items)
        {
            item.SetTooltip(_tooltip);
        }

        _swiper = GetComponentInChildren<PageSwiper>();
        _builder = GetComponentInChildren<DungeonBuilder>();

        gameObject.SetActive(false);
        //_tooltip.HideTooltip();
    }

    void UpdateList()
    {
        if (!Services.isInited)
            return;

        List<ItemVO> items = new List<ItemVO>();//Services.Player.GetPlayerVO.Items;
        for (int i = 0; i < 10; i++)
        {
            items.Add(new ItemVO(1, 2));
        }

        _swiper.UpdateData(items);
        _builder.BuildDungeon();
    }

    public void Show()
    {
        //_tooltip.HideTooltip();
        UpdateList();
    }

    public string GetName() => "Город";
    public GameObject GetGameObject() => gameObject;

    public void Hide()
    {

    }
}