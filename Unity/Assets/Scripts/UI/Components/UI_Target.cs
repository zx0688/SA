using System;
using System.Collections;
using System.Collections.Generic;

using haxe.root;
using UnityEngine;
using UnityEngine.UI;

public class UI_Target : MonoBehaviour
{
    [HideInInspector] public UI_TargetItem[] items;
    [SerializeField] private UI_TargetItem _one;

    public void SetItems(ConditionMeta[] condition)
    {
        if (condition == null || condition.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (condition.Length == 1)
        {
            foreach (UI_TargetItem item in items)
            {
                item.Clear();
                item.gameObject.SetActive(false);
            }

            _one.gameObject.SetActive(true);
            _one.SetItem(condition[0]);
            return;
        }

        _one.Clear();
        _one.gameObject.SetActive(false);

        for (int i = 0; i < items.Length; i++)
        {
            UI_TargetItem item = items[i];
            if (i < condition.Length)
            {
                ConditionMeta r = condition[i];
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
        //_one.gameObject.SetActive(false);
    }

    void Awake()
    {
        items = GetComponentsInChildren<UI_TargetItem>();

        //_linePanel = transform.Find("UI_RewardLine").gameObject;
        //items = _linePanel.transform.GetComponentsInChildren<UI_RewardItem>();
        //costItem = buyPanel.transform.Find ("CostPanel").GetComponentInChildren<UI_RewardItem> ();
        //header = transform.Find ("Header").GetComponent<Text> ();
    }

}