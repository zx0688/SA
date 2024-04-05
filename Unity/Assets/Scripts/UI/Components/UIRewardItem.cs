using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class UIRewardItem : MonoBehaviour
{
    [SerializeField] private Text value;
    [SerializeField] private Image icon;
    [SerializeField] private Image randomIcon;


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

    // public bool SetCountVisible
    // {
    //     get
    //     {
    //         return count.gameObject.activeInHierarchy && count.gameObject.activeSelf;
    //     }
    //     set
    //     {
    //         count.gameObject.SetActive(value);
    //     }
    // }

    public virtual void SetItem(RewardMeta reward, Color32[] colors = null)
    {
        if (reward == null)
        {
            Clear();
            return;
        }


        if (reward.Type == ConditionMeta.ITEM && value != null)
        {
            value.text = reward.Count > 0 ? $"+{Math.Abs(reward.Count)}" : $"-{Math.Abs(reward.Count)}";
            value.color = reward.Count > 0 ? (colors == null || colors.Length == 0 ? Color.green : colors[0]) : (colors == null || colors.Length == 0 ? Color.red : colors[1]);
        }

        isEmpty = false;

        //if (this.data != null && this.data.Id == reward.Id && this.data.Type == reward.Type)
        //    return;

        this.data = reward;
        this.gameObject.SetActive(true);

        //if (tooltip != null)
        //    showTooltipBtn.interactable = true;
        if (reward.Type == ConditionMeta.ITEM)
        {
            icon.LoadItemIcon(reward.Id);

            randomIcon.gameObject.SetActive(reward.Chance > 0);
            //            count.gameObject.SetActive(false);

            // if (Services.Player.Profile.Items.TryGetValue(reward.Id, out ItemData i))
            // {
            //     count.gameObject.SetActive(true);
            //     count.text = i.Count.ToString();
            // }
            // else
            // {
            //     count.gameObject.SetActive(false);
            // }

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

    protected virtual void OnClick()
    {
        //tooltip.ShowTooltip (data);
    }

    public void SetTooltip(UIInventoryTooltip tooltip)
    {
        //this.tooltip = tooltip;
    }
}