using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Reward : MonoBehaviour
{
    [SerializeField] private UI_RewardItem[] _items;

    public void SetItems(List<RewardData> reward)
    {
        if (reward == null || reward.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        for (int i = 0; i < _items.Length; i++)
        {
            UI_RewardItem item = _items[i];
            if (i < reward.Count)
            {
                RewardData r = reward[i];
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

    void Start()
    {
        //gameObject.SetActive(false);
    }

    void Awake()
    {
        //_linePanel = transform.Find("UI_RewardLine").gameObject;
        //items = _linePanel.transform.GetComponentsInChildren<UI_RewardItem>();
        //costItem = buyPanel.transform.Find ("CostPanel").GetComponentInChildren<UI_RewardItem> ();
        //header = transform.Find ("Header").GetComponent<Text> ();
    }

}