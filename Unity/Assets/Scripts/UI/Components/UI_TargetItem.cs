using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Meta;
using UnityEngine;
using UnityEngine.UI;

public class UI_TargetItem : MonoBehaviour
{

    [SerializeField] private Text _value;
    [SerializeField] private Image _icon;
    [SerializeField] private Text _target;
    [SerializeField] private Image _check;

    [MaybeNull] private ConditionMeta _data;

    public void SetItem(ConditionMeta condition)
    {
        if (condition == null)
        {
            Clear();
            return;
        }

        _value.text = condition.Count.ToString();

        if (this._data != null && this._data.Id == condition.Id && this._data.Tp == condition.Tp)
            return;

        this._data = condition;
        ItemVO current = Services.Player.GetItemVOByID(_data.Id);

        _icon.enabled = true;
        _value.enabled = true;
        _target.enabled = true;

        _target.text = "Накопи";
        _value.text = $"{current.Count}/{_data.Count}";
        _value.color = current.Count >= _data.Count ? Color.green : Color.white;
        _check.gameObject.SetActive(current.Count >= _data.Count);

        Services.Assets.SetSpriteIntoImage(_icon, "Items/" + condition.Id + "/icon", true).Forget();
    }

    public virtual void Clear()
    {
        _data = null;

        _icon.enabled = false;
        _icon.sprite = null;

        _value.enabled = false;
        _target.enabled = false;
    }

    void Start()
    {
    }

    void Awake()
    {
        Clear();
    }

    public void OnClick()
    {
        //tooltip.ShowTooltip (data);
    }


    public void SetTooltip(UI_InventoryTooltip tooltip)
    {
        //this.tooltip = tooltip;
    }
}