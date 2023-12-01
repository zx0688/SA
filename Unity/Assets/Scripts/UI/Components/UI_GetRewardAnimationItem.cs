using System;
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class UI_GetRewardAnimationItem : ServiceBehaviour
{
    // Start is called before the first frame update

    //public event Action OnComplete;
    private Text itemName;
    private Text itemCount;
    private Text itemAction;
    private Image icon;
    private ItemMeta data;

    [SerializeField]
    public float duration = 1.5f;

    int current;
    int newValue;

    private CanvasGroup group;

    private List<RewardMeta> waiting;

    protected override void Awake()
    {
        base.Awake();

        itemName = transform.Find("Name").GetComponent<Text>();
        itemCount = transform.Find("Count").GetComponent<Text>();
        itemAction = transform.Find("Action").GetComponent<Text>();
        icon = GetComponentInChildren<Image>();
        group = GetComponent<CanvasGroup>();
    }

    protected override void OnServicesInited()
    {
        base.OnServicesInited();

        //Services.Player.OnGetReward += OnItemReceived;
        //ResourceItem r = new ResourceItem ();
        //r.id = 1;
        //r.count = 34;
        //Add (r);
    }

    private void OnItemReceived(List<RewardMeta> items)
    {
        foreach (RewardMeta i in items)
        {
            Add(i);
        }

    }

    void Start()
    {
        waiting = new List<RewardMeta>();
        gameObject.SetActive(false);
    }

    private void CheckWaiting()
    {
        if (waiting.Count == 0)
            return;

        RewardMeta i = waiting[0];
        waiting.RemoveAt(0);

        Add(i);
    }

    public void Add(RewardMeta reward)
    {

        if (gameObject.activeSelf)
        {
            waiting.Add(reward);
            return;
        }
        gameObject.SetActive(true);
        Show(reward);
    }

    public void Punch()
    {
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 1f, 0, 0.01f);
    }

    public void Hide()
    {

        // resourceCount.gameObject.transform.DOPunchScale(new Vector3(1f, 1f, 1f), 0.2f, 1, 1);//.SetDelay(duration * 0.5f);

        group.DOFade(0f, 0.1f * duration).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            gameObject.SetActive(false);
            this.DOKill();
            CheckWaiting();
        });
    }


    private void Show(RewardMeta reward)
    {
        data = null;//Services.Meta.Game.Items[];//[reward.Id.ToString()];

        //current = Services.Player.itemHandler.AvailableItem(reward.id);
        newValue = current + reward.Count;

        itemName.text = data.Name;
        itemCount.text = current.ToString();

        itemAction.text = GetActionText(data, current, newValue);

        icon.LoadItemIcon(reward.Id);

        gameObject.SetActive(true);

        group.alpha = 0f;
        group.DOFade(1f, duration * 0.1f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            group.alpha = 1f;

        });

        DOTween.To(() => current, x => current = x, newValue, duration * 0.65f).SetDelay(duration * 0.1f).SetEase(Ease.OutExpo).OnComplete(Hide);

    }

    private string GetActionText(ItemMeta data, int current, int newValue)
    {
        string key = "Reward.GetAdd";
        if (current == 0 && newValue > 0)
        {
            //if (data.tags.Equals(DataService.BUILDING))
            //    key = "Reward.GetBuild";
            //else if (data.tags.Equals(DataService.CRAFT))
            //    key = "Reward.GetCraft";
        }
        else if (newValue <= 0)
        {
            /*if (data.tags.Equals(DataService.BUILDING) || data.tags.Equals(DataService.CRAFT))
                key = "Reward.GetDestroy";
            else if (data.tags.Equals(DataService.BUILDING))
                key = "Reward.GetCraft";*/
        }
        else if (newValue > current)
        {
            /*if (data.tags.Equals(DataService.BUILDING) || data.tags.Equals(DataService.CRAFT))
                key = "Reward.GetUpgrade";
            else
                key = "Reward.GetAdd";*/
        }
        else if (newValue < current)
        {
            //if (data.ags.Equals(DataService.BUILDING) || data.tags.Equals(DataService.CRAFT))
            //    key = "Reward.GetDowngrade";
            //else
            //    key = "Reward.GetSub";
        }

        return key.Localize();
    }

    // Update is called once per frame
    void Update()
    {
        itemCount.text = current.ToString();
    }
}