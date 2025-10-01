using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIReward : MonoBehaviour
{
    [SerializeField] private UIRewardItem[] addItems;
    [SerializeField] private GameObject title;

    public bool HasReward = false;


    public void Hide()
    {
        HasReward = false;
        gameObject.SetActive(false);
        if (title != null)
            title.SetActive(false);
        foreach (UIRewardItem r in addItems)
        {
            r.Clear();
            r.gameObject.SetActive(false);
        }
    }

    public void SetItems(List<RewardMeta> rewards, List<RewardMeta> cost, bool animate = false)
    {
        if ((rewards == null || rewards.Count == 0) && (cost == null || cost.Count == 0))
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);

        if (title != null)
            title.SetActive(true);

        var reward = GroupById(rewards, cost);
        reward = reward
            .OrderBy(item => item.Count >= 0)
            .ThenBy(item => int.Parse(item.Id))
            .ToList();

        HasReward = true;

        for (int i = 0; i < addItems.Length; i++)
        {
            UIRewardItem item = addItems[i];
            if (i < reward.Count)
            {
                item.gameObject.SetActive(true);
                item.SetItem(reward[i]);
            }
            else
            {
                item.Clear();
                item.gameObject.SetActive(false);
            }
        }

    }

    private List<RewardMeta> GroupById(List<RewardMeta> rewards, List<RewardMeta> cost)
    {
        List<RewardMeta> result = new List<RewardMeta>();
        List<RewardMeta> _result = rewards != null ? rewards : new List<RewardMeta>();
        if (cost != null)
        {
            cost.ForEach(c =>
            {
                var r = new RewardMeta();
                r.Id = c.Id;
                r.Chance = c.Chance;
                r.Count = -c.Count;
                _result.Add(r);
            });
        }
        var rewardIds = _result.GroupBy(r => r.Id);

        foreach (var group in rewardIds)
        {
            if (group.Count() > 1)
            {
                double chance = 1f;
                int count = 0;
                foreach (var reward in group)
                {
                    chance *= reward.Chance / 100;
                    count += reward.Count;
                }
                RewardMeta r = new RewardMeta();
                r.Id = group.First().Id;
                r.Count = count;
                r.Chance = Math.Round(chance, 2) * 100;
                result.Add(r);
            }
            else
                result.Add(group.First());
        }
        return result;
    }


}