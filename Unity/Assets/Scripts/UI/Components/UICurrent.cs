using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UICurrent : MonoBehaviour
{
    [SerializeField] private UICurrentItem[] items;
    [SerializeField] private Image backpack;

    void Start()
    {
        UIDropReward.OnDropArraved += OnDropArraved;
    }

    private void OnDropArraved()
    {
        Sequence pulseSequence = DOTween.Sequence();
        pulseSequence.Append(backpack.transform.DOScale(1.1f, 0.1f));
        pulseSequence.Append(backpack.transform.DOScale(1f, 0.1f));
        pulseSequence.SetLoops(1, LoopType.Yoyo);
    }

    public void TakeCard()
    {
        //next.GetComponent<RectTransform>().DOKill();
        //next.GetComponent<RectTransform>().DOScale(1.1f, 0.15f);
    }

    public void DropCard()
    {
        //next.GetComponent<RectTransform>().DOKill();
        //next.GetComponent<RectTransform>().DOScale(1.0f, 0.15f);
    }

    public void Hide() => SetItems(null, null, null);

    public void SetItems(List<ItemData> change, ProfileData profile, GameMeta meta)
    {
        if (change == null || change.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        //next.gameObject.SetActive(false);
        //next.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

        gameObject.SetActive(true);

        for (int i = 0; i < items.Length; i++)
        {
            UICurrentItem item = items[i];
            if (i < change.Count)
            {
                item.gameObject.SetActive(true);
                profile.Items.TryGetValue(change[i].Id, out ItemData data);
                meta.Items.TryGetValue(change[i].Id, out ItemMeta m);

                item.SetItem(change[i], data, m);
            }
            else
            {
                item.Clear();
                item.gameObject.SetActive(false);
            }
        }

    }

}