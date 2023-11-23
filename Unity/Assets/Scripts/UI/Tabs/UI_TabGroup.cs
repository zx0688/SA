using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_TabGroup : ServiceBehaviour
{
    [SerializeField] private List<UI_TabButton> _tabs;
    [SerializeField] private List<GameObject> _pages;
    [SerializeField] private Text _title;

    public Action OnPageChanged;

    [SerializeField] private UI_TabButton _defaultTab;
    private UI_TabButton _selectedTab;

    protected override void OnServicesInited()
    {
        base.OnServicesInited();

        foreach (UI_TabButton tab in _tabs)
        {
            tab.TabGroup = this;
        }

        for (int i = 0; i < _pages.Count; i++)
        {
            _pages[i].GetComponent<IPage>().GetGameObject().SetActive(false);
        }

        OnTabSelect(_defaultTab);
    }

    public void OnTabSelect(UI_TabButton tab)
    {
        if (_selectedTab == tab) return;

        if (_selectedTab != null)
        {
            _selectedTab.Deselect();
        }

        _selectedTab = tab;
        int index = _tabs.IndexOf(tab);
        for (int i = 0; i < _pages.Count; i++)
        {
            IPage p = _pages[i].GetComponent<IPage>();
            p.GetGameObject().SetActive(i == index);
            if (i == index)
            {
                p.Show();
                _title.text = p.GetName();
            }
            else
            {
                p.Hide();
            }
        }

        _selectedTab.Select();
        OnPageChanged?.Invoke();
    }
}