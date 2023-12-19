using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;

using UI;
using UnityEngine;
using UnityEngine.UI;


public class UIQuestsItem : MonoBehaviour, ISetData<ItemData>
{
    [SerializeField] private Button showTooltipBtn;
    [SerializeField] private Image icon;
    [SerializeField] private Text header;
    [SerializeField] private UITarget target;
    [SerializeField] private GameObject star;

    private UIQuestsTooltip tooltip;

    private ItemData data = null;
    private CardData cardData = null;
    private CardMeta meta = null;
    private CanvasGroup canvasGroup;

    public ItemData Data => data;

    public void SetItem(ItemData data)
    {
        if (data == null)
        {
            Hide();
            return;
        }
        if (!Services.Meta.Game.Cards.TryGetValue(data.Id, out meta))
            throw new Exception("quest should have a card meta data");

        if (!Services.Player.Profile.Cards.TryGetValue(data.Id, out cardData))
            throw new Exception("quest should have a card profile data");

        star.SetActive(Services.Player.FollowQuest == data.Id);
        target.SetItems(meta.SC, meta.ST);

        this.data = data;

        header.Localize(meta.Name, LocalizePartEnum.CardName);
        icon.LoadCardImage(meta.Image);

        showTooltipBtn.interactable = true;
        gameObject.SetActive(true);
    }

    void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    protected virtual void OnClick()
    {
        tooltip.ShowTooltip(meta, cardData);
    }

    public void SetTooltip(UIQuestsTooltip tooltip)
    {
        this.tooltip = tooltip;
        showTooltipBtn.onClick.AddListener(OnClick);
    }

    public void Hide()
    {
        canvasGroup.DOKill();
        canvasGroup.alpha = 0;

        star.SetActive(false);

        target.SetItems(null, null);

        if (showTooltipBtn)
            showTooltipBtn.interactable = false;
    }
}