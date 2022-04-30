using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class UI_RewardItem : MonoBehaviour, ITick
{

    [HideInInspector]
    protected Text count;

    public Button showTooltipBtn;

    public Image icon;

    protected RewardData data;
    protected bool isEmpty;
    protected IShowTooltip<ItemData> tooltip;

    [HideInInspector]
    protected Text timer;
    [HideInInspector]
    protected GameObject coin;
    [HideInInspector]
    protected GameObject timerPanel;

    protected Image back;

    public virtual void SetItem(RewardData reward)
    {

        if (reward == null)
        {
            Clear();
            return;
        }

        count.text = reward.Count.ToString();
        isEmpty = false;

        if (this.data != null && this.data.Id == reward.Id && this.data.Tp == reward.Tp)
            return;

        this.data = reward; //Services.Data.ItemInfo (reward.id);

        icon.enabled = true;
        count.enabled = true;

        if (tooltip != null)
            showTooltipBtn.interactable = true;

        count.gameObject.SetActive(true);
        switch (reward.Tp)
        {
            case DataService.CARD_ID:
                break;
            case DataService.SKILL_ID:
                Services.Assets.SetSpriteIntoImage(back, "Skills/back", true).Forget();
                Services.Assets.SetSpriteIntoImage(icon, "skills/" + reward.Id + "/icon", true).Forget();
                break;
            case DataService.ITEM_ID:
                Services.Assets.SetSpriteIntoImage(back, "Actions/back", true).Forget();
                //Services.Assets.SetSpriteIntoImage (back, "Items/back", true).Forget ();
                Services.Assets.SetSpriteIntoImage(icon, "Items/" + reward.Id + "/icon", true).Forget();
                break;
            case DataService.BUILDING_ID:
                Services.Assets.SetSpriteIntoImage(icon, "Buildings/" + reward.Id + "/icon", true).Forget();
                break;
            case DataService.ACTION_ID:

                Services.Assets.SetSpriteIntoImage(back, "Actions/back", true).Forget();
                Services.Assets.SetSpriteIntoImage(icon, "Actions/" + reward.Id + "/icon", true).Forget();
                count.gameObject.SetActive(false);
                break;

            default:
                // icon.gameObject.SetActive(false);
                break;
        }

    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public int GetId()
    {
        return data != null ? data.Id : 0;
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

        count = transform.Find("Count").GetComponent<Text>();
        back = transform.Find("Back").GetComponent<Image>();

        //isEmpty = true;
        // Clear ();
    }

    protected virtual void UpdateView(int timestamp)
    {

    }

    protected virtual void OnClick()
    {
        //tooltip.ShowTooltip (data);
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
        //this.tooltip = tooltip;
    }
}