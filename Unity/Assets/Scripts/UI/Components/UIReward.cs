using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIReward : MonoBehaviour
{
    [SerializeField] private UIRewardItem[] items;

    public void Hide() => SetItems(null);

    public void SetItems(RewardMeta[] rewards)
    {
        if (rewards == null || rewards.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        List<RewardMeta> prepared = rewards.Where(r => r.Type == ConditionMeta.ITEM).ToList();
        RewardMeta cardItem = rewards.Find(r => r.Type == ConditionMeta.CARD);
        if (cardItem != null)
            prepared.Add(cardItem);

        gameObject.SetActive(true);

        for (int i = 0; i < items.Length; i++)
        {
            UIRewardItem item = items[i];
            if (i < prepared.Count)
            {
                RewardMeta r = rewards[i];
                item.gameObject.SetActive(true);
                item.SetItem(r);
            }
            else
            {
                item.Clear();
                item.gameObject.SetActive(false);
            }
        }

    }

}