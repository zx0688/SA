using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using haxe.lang;
using UnityEngine;
using UnityEngine.UI;

public class UICurrentItem : MonoBehaviour
{
    [SerializeField] private Text value;
    [SerializeField] private Text difText;
    [SerializeField] private Image icon;

    private ItemMeta data;
    private bool isEmpty;

    public void SetItem(Null<ItemData> item, Null<ItemData> current, ItemMeta itemMeta, bool animate)
    {
        int cur = current.hasValue ? current.value.Count : 0;


        if (animate)
        {
            int prev = Math.Max(cur - (item.hasValue ? item.value.Count : 0), 0);
            int dif = cur - prev;
            difText.text = (dif < 0 ? "-" : "+") + Math.Abs(dif);
            difText.color = dif < 0 ? Color.red : Color.green;
            difText.gameObject.SetActive(true);

            CanvasGroup cg = difText.GetComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.DOKill();
            cg.DOFade(1f, 0.2f).OnComplete(() =>
            {
                cg.DOFade(0f, 0.2f).SetDelay(1f);
            }).SetDelay(0.2f);
            value.text = prev.ToString();
            DOTween.To(() => prev, x => prev = x, cur, 0.25f).OnUpdate(() => value.text = prev.ToString()).SetDelay(0.2f);

        }
        else
        {
            difText.gameObject.SetActive(false);
            value.text = cur.ToString();
        }
        //         difText.color = dif < 0 ? Color.red : Color.green;
        //         Color32 color32 = difText.color;
        //         color32.a = 0;
        //         difText.color = color32;
        //         color32.a = 255;
        // 
        //         difText.DOKill();
        //         difText.DOColor(color32, 0.3f).OnComplete(() =>
        //         {
        //             color32.a = 0;
        //             difText.DOColor(color32, 0.15f).SetDelay(0.3f);
        //         });



        isEmpty = false;

        this.data = itemMeta;
        this.gameObject.SetActive(true);

        icon.LoadItemIcon(itemMeta.Id);
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public virtual void Clear()
    {
        data = null;
        isEmpty = true;

        icon.sprite = null;
        this.gameObject.SetActive(false);
    }

    protected virtual void Awake()
    {

    }

    protected virtual void OnClick()
    {
        //tooltip.ShowTooltip (data);
    }

    public void SetTooltip(UIInventoryTooltip tooltip)
    {
        //this.tooltip = tooltip;
    }
}