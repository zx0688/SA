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
    [SerializeField] private UIRewardItem[] subItems;
    [SerializeField] private Color32[] colors;
    [SerializeField] private GameObject[] panels;
    //[SerializeField] private GameObject[] texts;


    public void Hide() => SetItems(null);

    public void SetItems(RewardMeta[] rewards)
    {
        if (rewards == null || rewards.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        List<RewardMeta> prepared = rewards.Where(r => r.Type == ConditionMeta.ITEM).ToList();
        List<RewardMeta> add = prepared.Where(r => r.Count > 0).ToList();
        List<RewardMeta> sub = prepared.Where(r => r.Count < 0).ToList();

        gameObject.SetActive(true);
        panels[0].SetActive(add.Count > 0);
        // texts[0].SetActive(add.Count > 0);
        panels[1].SetActive(sub.Count > 0);
        // texts[1].SetActive(sub.Count > 0);

        Dictionary<string, List<RewardMeta>> addmap = new Dictionary<string, List<RewardMeta>>();
        add.ForEach(r =>
        {
            if (!addmap.TryGetValue(r.Id, out List<RewardMeta> value))
            {
                value = new List<RewardMeta>();
                addmap[r.Id] = value;
            }
            value.Add(r);
        });

        Dictionary<string, List<RewardMeta>> submap = new Dictionary<string, List<RewardMeta>>();
        sub.ForEach(r =>
        {
            if (!submap.TryGetValue(r.Id, out List<RewardMeta> value))
            {
                value = new List<RewardMeta>();
                submap[r.Id] = value;
            }
            value.Add(r);
        });


        for (int i = 0; i < addItems.Length; i++)
        {
            UIRewardItem item = addItems[i];
            if (i < addmap.Count)
            {

                item.gameObject.SetActive(true);
                item.SetItem(addmap.ElementAt(i).Value, colors);
            }
            else
            {
                item.Clear();
                item.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < subItems.Length; i++)
        {
            UIRewardItem item = subItems[i];
            if (i < submap.Count)
            {
                RewardMeta r = sub[i];
                item.gameObject.SetActive(true);
                item.SetItem(submap.ElementAt(i).Value, colors);
            }
            else
            {
                item.Clear();
                item.gameObject.SetActive(false);
            }
        }

    }

}