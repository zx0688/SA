using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITabGroup : ServiceBehaviour
{
    [SerializeField] private List<UITabButton> tabs;
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private Text title;

    public Action OnPageChanged;

    [SerializeField] private UITabButton defaultTab;
    private UITabButton selectedTab;

    protected override void OnServicesInited()
    {
        base.OnServicesInited();

        foreach (UITabButton tab in tabs)
        {
            tab.TabGroup = this;
        }

        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].GetComponent<IPage>().GetGameObject().SetActive(false);
        }

        OnTabSelect(defaultTab);
    }

    public void OnTabSelect(UITabButton tab)
    {
        if (selectedTab == tab) return;

        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = tab;
        int index = tabs.IndexOf(tab);
        for (int i = 0; i < pages.Count; i++)
        {
            IPage p = pages[i].GetComponent<IPage>();
            p.GetGameObject().SetActive(i == index);
            if (i == index)
            {
                p.Show();
                title.text = p.GetName();
            }
            else
            {
                p.Hide();
            }
        }

        selectedTab.Select();
        OnPageChanged?.Invoke();
    }
}