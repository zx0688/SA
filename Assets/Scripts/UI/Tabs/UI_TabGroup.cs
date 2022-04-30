using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class UI_TabGroup : ServiceBehaviour
{
    // Start is called before the first frame update
    public List<UI_TabButton> tabs;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;

    private UI_TabButton selectedTab;
    public List<GameObject> pages;

    public UnityEvent OnPageChanged;

    public UI_TabButton defaultTab;

    protected override void OnServicesInited()
    {
        base.OnServicesInited();

        if (defaultTab != null)
            OnTabSelector(defaultTab);
    }
    public void Add(UI_TabButton tab)
    {
        if (tabs == null)
        {
            tabs = new List<UI_TabButton>();
        }

        tabs.Add(tab);

    }

    public void OnTabEnter(UI_TabButton tab)
    {
        if (selectedTab == null || selectedTab != tab)
        {
            //tab.backgroud.sprite = tabHover;
        }
    }
    public void OnTabExit(UI_TabButton tab)
    {
        Reset();
    }
    public void OnTabSelector(UI_TabButton tab)
    {
        if (selectedTab != null) { selectedTab.Deselect(); }

        selectedTab = tab;

        Reset();
        selectedTab.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
        //selectedTab.backgroud.sprite = tabActive;

        int index = tabs.IndexOf(tab);//tab.transform.GetSiblingIndex ();
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);
        }

        selectedTab.Select();
        OnPageChanged?.Invoke();
    }

    public void Reset()
    {
        foreach (UI_TabButton tab in tabs)
        {
            if (selectedTab != null && selectedTab == tab)
                continue;

            // tab.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            // tab.backgroud.sprite = tabIdle;
        }
    }
}