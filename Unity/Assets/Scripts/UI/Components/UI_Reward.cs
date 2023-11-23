using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleLocalization;
using Meta;
using UnityEngine;
using UnityEngine.UI;

public class UI_Reward : MonoBehaviour
{
    [SerializeField] private UI_RewardItem[] _items;

    public void SetItems(List<RewardMeta> rewards)
    {
        if (rewards == null || rewards.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        List<RewardMeta> prepared = rewards.Where(r => r.Tp == GameMeta.ITEM).ToList();
        RewardMeta cardItem = rewards.Find(r => r.Tp == GameMeta.CARD);
        if (cardItem != null)
            prepared.Add(cardItem);

        gameObject.SetActive(true);

        for (int i = 0; i < _items.Length; i++)
        {
            UI_RewardItem item = _items[i];
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