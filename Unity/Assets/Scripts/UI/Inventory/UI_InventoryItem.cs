using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using UI;
using UnityEngine;
using UnityEngine.UI;


public class UI_InventoryItem : MonoBehaviour, ITick, ISetData<ItemData>
{
    [SerializeField] protected Text count;
    [SerializeField] protected GameObject counter;

    [SerializeField] private Button _showTooltipBtn;
    public Image Icon;

    protected ItemMeta data;
    protected string id;
    protected bool isEmpty;
    protected UI_InventoryTooltip tooltip;

    public ItemData Data => throw new NotImplementedException();

    public void SetItem(ItemData item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        count.text = item.Count.ToString();
        isEmpty = false;

        if (id == item.Id)
            return;

        id = item.Id;
        data = null;///Services.Meta.Game.Items[item.Id.ToString()];
        counter.SetActive(true);
        Icon.enabled = true;
        count.enabled = true;

        _showTooltipBtn.interactable = true;

        Icon.LoadItemIcon(item.Id);
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public int GetId()
    {
        return 0;//data != null ? data. : 0;
    }

    public virtual void Clear()
    {
        isEmpty = true;
        Icon.enabled = false;
        Icon.sprite = null;
        counter.SetActive(false);

        count.enabled = false;

        if (_showTooltipBtn)
            _showTooltipBtn.interactable = false;
    }

    void Start()
    {

    }

    void Awake()
    {
        isEmpty = true;
        Clear();
    }

    protected void UpdateView(int timestamp)
    {

    }

    protected void OnClick()
    {
        if (data != null)
            tooltip.ShowTooltip(data.Id, data);
    }

    public void Tick(int timestamp)
    {

    }

    public bool IsTickble()
    {
        return false;
    }

    public void SetTooltip(UI_InventoryTooltip tooltip)
    {
        this.tooltip = tooltip;
        _showTooltipBtn.onClick.AddListener(OnClick);
    }

    public void Hide()
    {
        Clear();
    }
}