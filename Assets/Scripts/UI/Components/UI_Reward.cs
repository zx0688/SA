using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Meta;
using UnityEngine;
using UnityEngine.UI;

public class UI_Reward : MonoBehaviour
{
    // Start is called before the first frame update

    [HideInInspector]
    public UI_RewardItem[] items;
    private Text header;
    private GameObject buyPanel;
    private GameObject linePanel;
    private UI_RewardItem costItem;
    public void SetItems(List<RewardData> reward)
    {

        if (reward == null || reward.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);


        linePanel.SetActive(true);


        for (int i = 0; i < items.Length; i++)
        {
            UI_RewardItem item = items[i];
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
        gameObject.SetActive(false);
    }
    void Awake()
    {

        linePanel = transform.Find("UI_RewardLine").gameObject;
        items = linePanel.transform.GetComponentsInChildren<UI_RewardItem>();

        //costItem = buyPanel.transform.Find ("CostPanel").GetComponentInChildren<UI_RewardItem> ();

        //header = transform.Find ("Header").GetComponent<Text> ();
    }

}