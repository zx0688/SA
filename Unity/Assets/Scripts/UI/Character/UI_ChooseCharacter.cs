using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChooseCharacter : ServiceBehaviour, IPage
{
    [SerializeField] private UITabGroup group;
    [SerializeField] private ClickButton clickButton;

    [SerializeField] private PageSwiper swiperPage;

    private List<string> Ids => Services.Meta.Game.Heroes.Keys.ToList();

    public void Show()
    {
        gameObject.SetActive(true);
        this.enabled = true;

        clickButton.OnClick += OnClick;
        swiperPage.UpdateData(Ids);
    }

    private void OnClick()
    {
        Services.Player.SelfHeroChoose(Ids[swiperPage.GetPage()]);
        group.OnTabSelect(1);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        this.enabled = false;

        swiperPage.Hide();
        clickButton.OnClick -= OnClick;
    }

    public string GetName() => null;

    public GameObject GetGameObject() => gameObject;
}