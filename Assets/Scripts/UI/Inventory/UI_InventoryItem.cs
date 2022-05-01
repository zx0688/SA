using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Meta;
using UnityEngine;
using UnityEngine.UI;


public class UI_InventoryItem : MonoBehaviour, ITick
{

    [HideInInspector]
    protected Text count;

    public Button showTooltipBtn;

    public Image icon;

    protected ItemData data;
    protected bool isEmpty;
    protected UI_InventoryTooltip tooltip;





    public virtual void SetItem(ItemVO item)
    {

        if (item == null)
        {
            Clear();
            return;
        }

        count.text = item.count.ToString();
        isEmpty = false;

        if (this.data != null && this.data.Id == item.id)
            return;

        this.data = Services.Data.ItemInfo(item.id);

        icon.enabled = true;
        count.enabled = true;

        if (tooltip != null)
            showTooltipBtn.interactable = true;

        Services.Assets.SetSpriteIntoImage(icon, "Items/" + item.id + "/icon", true).Forget();
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
        data = null;
        isEmpty = true;
        icon.enabled = false;
        icon.sprite = null;

        count.enabled = false;

        if (tooltip != null)
            showTooltipBtn.interactable = false;
    }

    void Start()
    {

        if (tooltip != null)
            showTooltipBtn.onClick.AddListener(OnClick);
    }
    protected virtual void Awake()
    {

        count = transform.Find("Value").GetComponent<Text>();

        //isEmpty = true;
        // Clear ();
    }

    protected virtual void UpdateView(int timestamp)
    {

    }

    protected virtual void OnClick()
    {
        tooltip.ShowTooltip(data);
    }

    public virtual void Tick(int timestamp)
    {

    }

    public virtual bool IsTickble()
    {
        return false;
    }

    public void SetTooltip(UI_InventoryTooltip tooltip)
    {
        this.tooltip = tooltip;
    }

}