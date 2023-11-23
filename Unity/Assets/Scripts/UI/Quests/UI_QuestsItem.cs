using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Meta;
using UI;
using UnityEngine;
using UnityEngine.UI;


public class UI_QuestsItem : MonoBehaviour, ITick, ISetData<QuestVO>
{
    [SerializeField] private Image[] rewards;
    [SerializeField] private Button showTooltipBtn;

    private UI_QuestsTooltip tooltip;

    public Image Icon;
    public Text Header;
    public Text Timer;
    public UI_Target target;

    private CardMeta data;
    private QuestVO vo;
    private bool isEmpty;
    private CanvasGroup canvasGroup;

    public QuestVO Data => throw new NotImplementedException();

    public void SetItem(ItemVO item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (this.vo != null && this.vo.Id == item.Id)
            return;

        isEmpty = false;

        vo = (QuestVO)item;

        //questData = Services.Data.QuestInfo(data.Id);
        data = new CardMeta();
        data.Act = new ActionMeta();
        //_data.Act.Time = 0;
        data.Name = "Муки и спасение";
        data.Id = item.Id;
        data.Act.Con = new List<ConditionMeta>();
        data.Act.Con.Add(new ConditionMeta());
        //_data.Act.Con.Add(new ConditionData());
        //_data.Act.Con.Add(new ConditionData());
        data.Act.Con[0].Id = 2;
        data.Act.Con[0].Count = 3;
        data.Des = "Соберите пять кристаллов";

        //_data.Act.Con[1].Id = 3;
        //_data.Act.Con[1].Count = 4;
        //_data.Act.Con[2].Id = 4;
        //_data.Act.Con[2].Count = 4;
        data.Act.Reward = new List<RewardMeta>();
        RewardMeta r = new RewardMeta();
        r.Count = 2;
        r.Tp = GameMeta.ITEM;
        r.Id = 3;
        data.Act.Reward.Add(r);
        data.Act.Reward.Add(r);

        canvasGroup.DOKill();
        canvasGroup.alpha = 1;

        Header.text = data.Name;
        Icon.enabled = true;
        target.SetItems(data.Act.Con);

        Services.Assets.SetSpriteIntoImage(Icon, "Quests/" + item.Id + "/image", true).Forget();

        showTooltipBtn.interactable = true;

        foreach (Image img in rewards)
        {
            img.gameObject.SetActive(false);
        }

        for (int i = 0; i < data.Act.Reward.Count && i < rewards.Length; i++)
        {
            rewards[i].gameObject.SetActive(true);
            Services.Assets.SetSpriteIntoImage(rewards[i], "Items/" + data.Act.Reward[i].Id + "/icon", true).Forget();
        }

        if (IsTickble()) { Tick(GameTime.Current); }

        gameObject.SetActive(true);
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public void Clear()
    {
        data = null;
        isEmpty = true;
        Icon.enabled = false;
        Icon.sprite = null;

        canvasGroup.DOKill();
        canvasGroup.alpha = 0;

        if (showTooltipBtn)
            showTooltipBtn.interactable = false;

    }

    void Start()
    {

    }

    void Awake()
    {
        isEmpty = true;
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    protected virtual void OnClick()
    {
        tooltip.ShowTooltip(data, vo);
    }

    public void Tick(int timestamp)
    {
        int i = 0;//GameTime.Left(timestamp, _vo.Activated, _data.Act.Time);

        if (i <= 0)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
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
        return false;//gameObject.activeSelf && _vo != null && _data != null && _data.Act.Time > 0;
    }

    public void SetTooltip(UI_QuestsTooltip tooltip)
    {
        this.tooltip = tooltip;
        showTooltipBtn.onClick.AddListener(OnClick);
    }

    public void SetItem(QuestVO data)
    {
        throw new NotImplementedException();
    }

    public void Hide()
    {
        throw new NotImplementedException();
    }
}