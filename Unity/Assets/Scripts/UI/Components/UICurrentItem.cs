using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using haxe.lang;
using UnityEngine;
using UnityEngine.UI;

public class UICurrentItem : MonoBehaviour
{
    [SerializeField] private Text value;
    [SerializeField] private Image icon;

    private ItemMeta data;
    private bool isEmpty;


    public void SetItem(Null<ItemData> item, Null<ItemData> current, ItemMeta itemMeta)
    {
        int cur = current.hasValue ? current.value.Count : 0;
        int prev = Math.Max(cur - (item.hasValue ? item.value.Count : 0), 0);
        value.text = prev.ToString();
        DOTween.To(() => prev, x => prev = x, cur, 0.2f).SetEase(Ease.OutCirc).OnUpdate(() => value.text = prev.ToString());

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
        Clear();
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