using System.Net.Http.Headers;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIQuests : ServiceBehaviour, IPage
{
    [SerializeField] private UIQuestsTooltip tooltip;
    [SerializeField] private Text empty;

    private UIQuestsItem[] items;
    private PageSwiper swiper;

    protected override void Awake()
    {
        base.Awake();

        items = GetComponentsInChildren<UIQuestsItem>();

        swiper = GetComponentInChildren<PageSwiper>();
        gameObject.SetActive(false);

        tooltip.HideTooltip();

        foreach (UIQuestsItem item in items)
        {
            item.SetTooltip(tooltip);
        }
    }

    override protected void OnServicesInited()
    {
        base.OnServicesInited();

    }

    void UpdateList()
    {

        List<string> activeQuest = Services.Player.Profile.ActiveQuests;
        string currentQuest = Services.Player.FollowQuest;
        List<ItemData> data = activeQuest.Select(s => new ItemData(s, 0)).ToList();
        empty.gameObject.SetActive(activeQuest.Count == 0);
        swiper.gameObject.SetActive(activeQuest.Count != 0);
        swiper.UpdateData(data);

        if (data.Count > 0 && currentQuest != null)
            data[0].Count = 1;

        for (int i = 0; i < items.Length; i++)
        {
            UIQuestsItem slot = items[i];

            if (i < data.Count)
            {
                slot.SetItem(data[i]);
            }
            else
            {
                slot.Hide();
            }
        }
    }

    public void Show()
    {
        tooltip?.HideTooltip();
        UpdateList();

        Services.Player.OnFollowQuestChanged += UpdateList;
    }

    public string GetName() => "Menu.Quests".Localize(LocalizePartEnum.GUI);
    public GameObject GetGameObject() => gameObject;

    public void Hide()
    {
        tooltip?.HideTooltip();

        Services.Player.OnFollowQuestChanged -= UpdateList;
    }
}