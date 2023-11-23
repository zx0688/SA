using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using DG.Tweening;
using Meta;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_GetRewardAnimationList : ServiceBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    public List<GameObject> items;
    [SerializeField]
    public float duration = 2f;

    [SerializeField]
    public float distance = 130f;

    private List<RewardMeta> waiting;

    protected override void OnServicesInited()
    {
        base.OnServicesInited();
        Services.Player.OnItemReceived += OnItemReceived;
    }

    private void OnItemReceived(List<RewardMeta> items)
    {
        foreach (RewardMeta r in items)
        {
            Add(r);
        }
    }

    void Start()
    {

        waiting = new List<RewardMeta>();

        foreach (GameObject i in items)
        {
            i.SetActive(false);
            i.GetComponent<UI_GetRewardAnimationItem>().duration = duration;
            //i.GetComponent<UI_GetRewardAnimationItem>().OnComplete += CheckWaiting;
        }
    }

    private void CheckWaiting()
    {
        if (waiting.Count == 0)
            return;

        RewardMeta i = waiting[0];
        waiting.RemoveAt(0);

        Add(i);
    }

    public void Add(RewardMeta reward)
    {

        GameObject item = items[items.Count - 1];

        if (item.activeSelf)
        {
            waiting.Add(reward);
            return;
        }

        items.RemoveAt(items.Count - 1);
        items.Insert(0, item);

        item.SetActive(true);
        item.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        // item.GetComponent<UI_GetRewardAnimationItem>().Show(reward);

        for (int index = 0; index < items.Count; index++)
        {
            GameObject i = items[index];
            if (i == item || i.activeSelf == false)
                continue;
            i.GetComponent<RectTransform>().DOAnchorPosY(-distance * index, 0.5f).SetEase(Ease.OutCirc).SetAutoKill();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}