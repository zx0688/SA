using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meta;
using UnityEngine;
using UnityEngine.UI;


public class UI_QuestsItem : MonoBehaviour, IItem, ITick
{
    public UI_QuestsTooltip tooltip;
    public Button showTooltipBtn;
    public Image icon;
    public Text timeLeft;
    public Text header;

    private CardData questData;
    private QuestVO questItem;
    private bool isEmpty;
    private CanvasGroup canvasGroup;

    public void SetItem(QuestVO quest)
    {

        isEmpty = false;

        if (questData != null && questData.Id == quest.id)
            return;

        questItem = quest;

        questData = Services.Data.QuestInfo(questItem.id);

        if (questData == null)
            return;

        canvasGroup.DOKill();
        canvasGroup.alpha = 1;

        gameObject.SetActive(true);
        header.text = questData.Name;

        icon.enabled = true;

        showTooltipBtn.interactable = true;

        Services.Assets.SetSpriteIntoImage(icon, "Items/1/icon", true).Forget();

        if (IsTickble()) { Tick(GameTime.Current); }

    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public int GetId()
    {
        return questData != null ? questData.Id : 0;
    }

    public void Clear()
    {
        questData = null;
        isEmpty = true;
        icon.enabled = false;
        icon.sprite = null;
        showTooltipBtn.interactable = false;
        gameObject.SetActive(false);
    }

    void Start()
    {

        showTooltipBtn.onClick.AddListener(OnShowTooltip);
        Clear();
    }
    void Awake()
    {
        isEmpty = true;
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    void OnShowTooltip()
    {
        //tooltip.ShowTooltip (questItem, questData);
    }

    public void Tick(int timestamp)
    {

        int i = GameTime.Left(timestamp, questItem.activated, questData.Act.Time);

        if (i <= 0)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
            {
                Clear();
            });
        }
        else
            timeLeft.text = TimeFormat.ONE_CELL_FULLNAME(i);
    }

    public bool IsTickble()
    {
        return gameObject.activeSelf && questItem != null && questData != null && questData.Act.Time > 0;
    }

    public void SetTooltip(UI_QuestsTooltip tooltip)
    {
        this.tooltip = tooltip;
    }
}