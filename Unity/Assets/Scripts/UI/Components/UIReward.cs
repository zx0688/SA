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
    //[SerializeField] private UIRewardItem[] subItems;
    [SerializeField] private Color32[] colors;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private int count = 3;
    //[SerializeField] private GameObject[] texts;


    public void Hide() => SetItems(null, null);

    public void SetItems(RewardMeta[] rewards, RewardMeta[] costs, bool animate = false)
    {
        if ((rewards == null || rewards.Length == 0) && (costs == null || costs.Length == 0))
        {
            gameObject.SetActive(false);
            return;
        }

        List<RewardMeta> reward = rewards != null ? rewards.Where(r => r.Type == ConditionMeta.ITEM).ToList() : new List<RewardMeta>();
        List<RewardMeta> cost = costs != null ? costs.Where(r => r.Type == ConditionMeta.ITEM).ToList() : new List<RewardMeta>();

        Dictionary<string, ItemData> map = new Dictionary<string, ItemData>();
        reward.ForEach(i =>
        {
            if (!map.TryGetValue(i.Id, out ItemData value))
            {
                value = new ItemData(i.Id, 0);
                map[i.Id] = value;
            }
            value.Count += i.Count;
        });
        cost.ForEach(i =>
        {
            if (!map.TryGetValue(i.Id, out ItemData value))
            {
                value = new ItemData(i.Id, 0);
                map[i.Id] = value;
            }
            value.Count -= i.Count;
        });
        //List<RewardMeta> sub = rewards != null ? rewards.Where(r => r.Count < 0 && r.Type == ConditionMeta.ITEM).ToList() : new List<RewardMeta>();

        //if (costs != null)
        //    sub.AddRange(costs);

        gameObject.SetActive(true);
        panels[0].SetActive(map.Keys.Count > 0);
        if (panels[1] != null)
            panels[1].SetActive(map.Keys.Count > count);
        //        panels[1].SetActive(sub.Count > 0);

        //Set(addItems, add, colors[0], animate);
        //Set(subItems, sub, colors[1], animate);

        for (int i = 0; i < addItems.Length; i++)
        {
            UIRewardItem item = addItems[i];
            if (i < map.Count)
            {
                item.gameObject.SetActive(true);
                //List<RewardMeta> data = map.ElementAt(i).Value;
                //                 List<RewardMeta> data = map.ElementAt(i).Value;
                // 
                //                 int min = 0;
                //                 data.Where(r => r.Chance == 0).ToList().ForEach(r => min += Math.Abs(r.Count));
                //                 int max = min;
                //                 data.Where(r => r.Chance > 0).ToList().ForEach(r => max += Math.Abs(r.Count));
                // 
                //                 ItemData itemData = new ItemData(data[0].Id, max);
                ItemData itemData = map.ElementAt(i).Value;
                item.SetItem(itemData, itemData.Count, itemData.Count >= 0 ? colors[0] : colors[1], animate);
            }
            else
            {
                item.Clear();
                item.gameObject.SetActive(false);
            }
        }

    }

    //     private void Set(UIRewardItem[] uiItems, List<RewardMeta> items, Color32 color, bool animate)
    //     {
    //         Dictionary<string, List<RewardMeta>> map = new Dictionary<string, List<RewardMeta>>();
    //         items.ForEach(r =>
    //         {
    //             if (!map.TryGetValue(r.Id, out List<RewardMeta> value))
    //             {
    //                 value = new List<RewardMeta>();
    //                 map[r.Id] = value;
    //             }
    //             value.Add(r);
    //         });
    // 
    // 
    //         for (int i = 0; i < uiItems.Length; i++)
    //         {
    //             UIRewardItem item = uiItems[i];
    //             if (i < map.Count)
    //             {
    //                 item.gameObject.SetActive(true);
    //                 List<RewardMeta> data = map.ElementAt(i).Value;
    // 
    //                 int min = 0;
    //                 data.Where(r => r.Chance == 0).ToList().ForEach(r => min += Math.Abs(r.Count));
    //                 int max = min;
    //                 data.Where(r => r.Chance > 0).ToList().ForEach(r => max += Math.Abs(r.Count));
    // 
    //                 ItemData itemData = new ItemData(data[0].Id, min);
    //                 item.SetItem(itemData, max, color, animate);
    //             }
    //             else
    //             {
    //                 item.Clear();
    //                 item.gameObject.SetActive(false);
    //             }
    //         }
    //     }

}