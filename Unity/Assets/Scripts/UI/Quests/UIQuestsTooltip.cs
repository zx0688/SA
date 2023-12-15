using System;
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestsTooltip : MonoBehaviour
{
    [SerializeField] private TooltipBack background;
    [SerializeField] private Image icon;
    [SerializeField] private UITarget target;
    [SerializeField] private UIReward reward;
    [SerializeField] private Text description;
    [SerializeField] private Button Hidebutton;
    [SerializeField] private ClickButton Activebutton;
    [SerializeField] private Image star;
    [SerializeField] private Text activeText;


    private CardMeta meta;
    private CardData data;

    void Awake()
    {
        Hidebutton.onClick.AddListener(HideTooltip);
        Activebutton.OnClick += ActiveQuest;
        Activebutton.SetAsToggled = true;
    }

    private void ActiveQuest()
    {
        Services.Player.FollowQuest = meta.Id;

        star.gameObject.SetActive(true);
        star.rectTransform.localScale = new Vector3(0f, 0f, 0f);
        star.DOKill();
        star.rectTransform.DOScale(1f, 0.6f).SetEase(Ease.OutElastic);
    }

    public void HideTooltip()
    {
        background.Hide();
        background.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }


    public void ShowTooltip(CardMeta meta, CardData data)
    {
        background.Show("green", meta.Name.Localize(LocalizePartEnum.CardName));
        background.gameObject.SetActive(true);
        gameObject.SetActive(true);

        this.meta = meta;
        this.data = data;

        target.SetItems(meta.SC, meta.ST);
        reward.SetItems(meta.SR);

        star.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        star.gameObject.SetActive(Services.Player.FollowQuest == meta.Id);

        Activebutton.SetAsDisabled = Services.Player.FollowQuest == meta.Id;
        activeText.Localize(Services.Player.FollowQuest != meta.Id ? "Quest.MakeActive" : "Quest.IsActived", LocalizePartEnum.GUI);

        description.text = meta.Desc.Localize(LocalizePartEnum.CardDescription);
        icon.LoadCardImage(meta.Image);
    }

}