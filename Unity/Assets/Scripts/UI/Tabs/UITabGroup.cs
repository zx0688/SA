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
    [SerializeField] private List<GameObject> hiddenPages;
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private Text title;
    [SerializeField] private GameObject topPanel;
    [SerializeField] private GameObject titlePanel;

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
            pages[i].GetComponent<IPage>().Hide();
        }

        if (Services.Meta.Game.Config.DisableTutorial)
        {
            Services.Player.SelfHeroChoose("1");
            OnTabSelect(defaultTab);
        }
        else
        {
            ShowHiddenTab(0);
        }

        //Services.Player.Profile.Hero = "1";
        //OnTabSelect(defaultTab);

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
        titlePanel.SetActive(true);
        topPanel.SetActive(true);

        foreach (var page in hiddenPages)
            page.GetComponent<IPage>().Hide();

        selectedTab.Select();
        OnPageChanged?.Invoke();
    }

    public void ShowHiddenTab(int tabIndex)
    {
        var pageObject = hiddenPages[tabIndex];
        tabs.ForEach(t => t.Hide());
        IPage p = pageObject.GetComponent<IPage>();
        p.Show();
        topPanel.SetActive(false);

        titlePanel.SetActive(p.GetName() != null);
        if (p.GetName() != null)
            title.text = p.GetName();

        OnPageChanged?.Invoke();
    }

    public void OnTabSelect(int tabIndex)
    {
        selectedTab = null;
        tabs.ForEach(t => t.Show());
        OnTabSelect(tabs[tabIndex]);
    }
}