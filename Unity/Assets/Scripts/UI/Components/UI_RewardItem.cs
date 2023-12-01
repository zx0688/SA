using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class UI_RewardItem : MonoBehaviour, ITick
{

    [SerializeField] private Text count;
    [SerializeField] private Image icon;

    [SerializeField] private Image up;
    [SerializeField] private Image down;


    private RewardMeta data;
    private bool isEmpty;

    //protected IShowTooltip<ItemMeta> tooltip;
    //public Button showTooltipBtn;

    // [HideInInspector]
    // protected Text timer;
    // [HideInInspector]
    // protected GameObject coin;
    // [HideInInspector]
    // protected GameObject timerPanel;

    //protected Image back;

    public virtual void SetItem(RewardMeta reward)
    {
        if (reward == null)
        {
            Clear();
            return;
        }

        //_up.gameObject.SetActive(reward.Count > 0);
        //_down.gameObject.SetActive(reward.Count < 0);

        if (reward.Type == ConditionMeta.ITEM)
        {
            if (reward.Random != null && reward.Random.Length > 0)
            {
                count.text = "  ?";
                count.color = Color.yellow;
            }
            else
            {
                count.text = reward.Count > 0 ? $"+{Math.Abs(reward.Count)}" : $"-{Math.Abs(reward.Count)}";
                count.color = reward.Count > 0 ? Color.green : Color.red;
            }
        }
        else if (reward.Type == ConditionMeta.CARD)
        {
            count.text = reward.Chance > 0 ? $"{reward.Chance}%" : "100%";
            count.color = Color.white;
        }

        isEmpty = false;

        if (this.data != null && this.data.Id == reward.Id && this.data.Type == reward.Type)
            return;

        this.data = reward;
        this.gameObject.SetActive(true);

        //if (tooltip != null)
        //    showTooltipBtn.interactable = true;
        if (reward.Type == ConditionMeta.ITEM)
        {
            if (reward.Random != null && reward.Random.Length > 0)
            {
                Services.Assets.SetSpriteIntoImage(icon, "UI/randomItem", true).Forget();
            }
            else
            {
                Services.Assets.SetSpriteIntoImage(icon, "Items/" + reward.Id, true).Forget();
            }
        }
        else if (reward.Type == ConditionMeta.CARD)
        {
            Services.Assets.SetSpriteIntoImage(icon, "UI/eventPic", true).Forget();
        }

    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public virtual void Clear()
    {
        data = null;
        isEmpty = true;

        icon.sprite = null;

        this.gameObject.SetActive(false);

        //if (tooltip != null)
        //    showTooltipBtn.interactable = false;
    }

    void Start()
    {
        // if (tooltip != null)
        //     showTooltipBtn.onClick.AddListener(OnClick);
    }
    protected virtual void Awake()
    {
        Clear();
    }

    protected virtual void UpdateView(int timestamp)
    {

    }

    protected virtual void OnClick()
    {
        //tooltip.ShowTooltip (data);
    }

    public virtual void Tick(int timestamp)
    {

    }

    public virtual bool IsTickble()
    {
        return false;
    }

    public void SetTooltip(UI_InventoryTooltip tooltip)
    {
        //this.tooltip = tooltip;
    }
}