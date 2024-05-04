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

    private RewardMeta data;
    private bool isEmpty;


    public void SetItem(List<RewardMeta> rewards, Color32[] colors = null, bool animate = false)
    {
        if (rewards == null || rewards.Count == 0)
        {
            Clear();
            return;
        }

        int min = 0;
        rewards.Where(r => r.Chance == 0).ToList().ForEach(r => min += Math.Abs(r.Count));
        int max = min;
        rewards.Where(r => r.Chance > 0).ToList().ForEach(r => max += Math.Abs(r.Count));
        RewardMeta reward = rewards[0];

        if (reward.Type == ConditionMeta.ITEM && value != null)
        {
            if (animate)
            {
                int cur = Services.Player.Profile.Items.TryGetValue(reward.Id, out ItemData i) ? i.Count : 0;
                int prev = Math.Max(cur - reward.Count, 0);
                value.text = prev.ToString();
                DOTween.To(() => prev, x => prev = x, cur, 0.3f).SetEase(Ease.InExpo).OnUpdate(() => value.text = prev.ToString());
            }
            else
            {
                value.text = max == min ? $"{min}" : $"{min}-{max}";
            }
            value.color = reward.Count > 0 ? (colors == null || colors.Length == 0 ? Color.green : colors[0]) : (colors == null || colors.Length == 0 ? Color.red : colors[1]);
        }

        isEmpty = false;

        this.data = reward;
        this.gameObject.SetActive(true);

        if (reward.Type == ConditionMeta.ITEM)
            icon.LoadItemIcon(reward.Id);
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