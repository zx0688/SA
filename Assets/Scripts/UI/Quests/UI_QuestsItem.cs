﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Data;
using UnityEngine;
using UnityEngine.UI;


public class UI_QuestsItem : MonoBehaviour, ITick, ISetItem
{
    [SerializeField] private Image[] _rewards;
    [SerializeField] private Button _showTooltipBtn;

    private UI_QuestsTooltip _tooltip;

    public Image Icon;
    public Text Header;
    public Text Timer;
    public UI_Target _target;

    private CardMeta _data;
    private QuestVO _vo;
    private bool _isEmpty;
    private CanvasGroup _canvasGroup;

    public void SetItem(ItemVO item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (this._vo != null && this._vo.Id == item.Id)
            return;

        _isEmpty = false;

        _vo = (QuestVO)item;

        //questData = Services.Data.QuestInfo(data.Id);
        _data = new CardMeta();
        _data.Act = new ActionMeta();
        _data.Act.Time = 0;
        _data.Name = "Муки и спасение";
        _data.Id = item.Id;
        _data.Act.Con = new List<ConditionData>();
        _data.Act.Con.Add(new ConditionData());
        //_data.Act.Con.Add(new ConditionData());
        //_data.Act.Con.Add(new ConditionData());
        _data.Act.Con[0].Id = 2;
        _data.Act.Con[0].Count = 3;
        _data.Act.Text = "Соберите пять кристаллов";
        //_data.Act.Con[1].Id = 3;
        //_data.Act.Con[1].Count = 4;
        //_data.Act.Con[2].Id = 4;
        //_data.Act.Con[2].Count = 4;
        _data.Act.Reward = new List<RewardData>();
        RewardData r = new RewardData();
        r.Count = 2;
        r.Tp = Meta.;
        r.Id = 3;
        _data.Act.Reward.Add(r);
        _data.Act.Reward.Add(r);

        _canvasGroup.DOKill();
        _canvasGroup.alpha = 1;

        Header.text = _data.Name;
        Icon.enabled = true;
        _target.SetItems(_data.Act.Con);

        Services.Assets.SetSpriteIntoImage(Icon, "Quests/" + item.Id + "/image", true).Forget();

        _showTooltipBtn.interactable = true;

        foreach (Image img in _rewards)
        {
            img.gameObject.SetActive(false);
        }

        for (int i = 0; i < _data.Act.Reward.Count && i < _rewards.Length; i++)
        {
            _rewards[i].gameObject.SetActive(true);
            Services.Assets.SetSpriteIntoImage(_rewards[i], "Items/" + _data.Act.Reward[i].Id + "/icon", true).Forget();
        }

        if (IsTickble()) { Tick(GameTime.Current); }

        gameObject.SetActive(true);
    }

    public bool IsEmpty()
    {
        return _isEmpty;
    }

    public void Clear()
    {
        _data = null;
        _isEmpty = true;
        Icon.enabled = false;
        Icon.sprite = null;

        _canvasGroup.DOKill();
        _canvasGroup.alpha = 0;

        if (_showTooltipBtn)
            _showTooltipBtn.interactable = false;
    }

    void Start()
    {

    }

    void Awake()
    {
        _isEmpty = true;
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    protected virtual void OnClick()
    {
        _tooltip.ShowTooltip(_data, _vo);
    }

    public void Tick(int timestamp)
    {
        int i = GameTime.Left(timestamp, _vo.Activated, _data.Act.Time);

        if (i <= 0)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
            {
                Clear();
                gameObject.SetActive(false);
            });
        }
        else
            Timer.text = TimeFormat.ONE_CELL_FULLNAME(i);
    }

    public bool IsTickble()
    {
        return gameObject.activeSelf && _vo != null && _data != null && _data.Act.Time > 0;
    }

    public void SetTooltip(UI_QuestsTooltip tooltip)
    {
        this._tooltip = tooltip;
        _showTooltipBtn.onClick.AddListener(OnClick);
    }

}