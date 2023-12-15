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

        if (data != null)
            star.SetActive(Services.Player.FollowQuest == data.Id);

        if ((this.data != null && this.data.Id == data.Id) || !Services.Meta.Game.Cards.TryGetValue(data.Id, out meta))
            return;

        if (meta == null)
            throw new Exception("Meta card ");

        this.data = data;
        cardData = Services.Player.Profile.Cards[data.Id];

        header.Localize(meta.Name, LocalizePartEnum.CardName);
        icon.LoadCardImage(meta.Image);
        target.SetItems(meta.SC, meta.ST);

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