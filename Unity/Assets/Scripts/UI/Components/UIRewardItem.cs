using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardItem : MonoBehaviour
{
    [SerializeField] private Text value;
    [SerializeField] private Image icon;
    [SerializeField] private Text chanceValue;
    [SerializeField] private Color32[] colors;

    private bool isEmpty;

    public void SetItem(RewardMeta item, bool asCost)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        // if (animate)
        // {
        //     int cur = Services.Player.Profile.Items.TryGetValue(item.Id, out ItemData i) ? i.Count : 0;
        //     int prev = Math.Max(cur - item.Count, 0);
        //     value.text = prev.ToString();
        //     DOTween.To(() => prev, x => prev = x, cur, 0.3f).SetEase(Ease.InExpo).OnUpdate(() => value.text = prev.ToString());
        // }
        // else
        // {
        //     value.text = max == item.Count ? $"{item.Count}" : $"{item.Count}-{max}";
        // }
        value.text = $"{Math.Abs(item.Count)}";
        value.color = asCost || item.Count < 0 ? colors[1] : colors[0];

        chanceValue.gameObject.SetActive(item.Chance > 0);
        if (item.Chance != 0)
            chanceValue.text = $"{item.Chance}%";

        isEmpty = false;

        this.gameObject.SetActive(true);
        icon.LoadItemIcon(item.Id);
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public virtual void Clear()
    {
        isEmpty = true;

        icon.sprite = null;
        this.gameObject.SetActive(false);
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