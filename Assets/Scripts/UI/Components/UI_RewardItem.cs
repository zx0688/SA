using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardItem : MonoBehaviour, ITick
{

    [SerializeField] private Text _count;
    [SerializeField] private Image _icon;

    private RewardData _data;
    private bool _isEmpty;

    //protected IShowTooltip<ItemMeta> tooltip;
    //public Button showTooltipBtn;

    // [HideInInspector]
    // protected Text timer;
    // [HideInInspector]
    // protected GameObject coin;
    // [HideInInspector]
    // protected GameObject timerPanel;

    //protected Image back;

    public virtual void SetItem(RewardData reward)
    {

        if (reward == null)
        {
            Clear();
            return;
        }

        _count.text = reward.Count > 0 ? $"+{reward.Count}" : $"-{reward.Count}";
        _count.color = reward.Count > 0 ? Color.green : Color.red;
        _isEmpty = false;

        if (this._data != null && this._data.Id == reward.Id && this._data.Tp == reward.Tp)
            return;

        this._data = reward; //Services.Data.ItemInfo (reward.id);
        this.gameObject.SetActive(true);

        //if (tooltip != null)
        //    showTooltipBtn.interactable = true;

        switch (reward.Tp)
        {
            case MetaData.CARD:
                break;
            //case DataService.SKILL_ID:
            //Services.Assets.SetSpriteIntoImage(back, "Skills/back", true).Forget();
            //Services.Assets.SetSpriteIntoImage(_icon, "skills/" + reward.Id + "/icon", true).Forget();
            //    break;
            case MetaData.ITEM:
                //Services.Assets.SetSpriteIntoImage(back, "Actions/back", true).Forget();
                //Services.Assets.SetSpriteIntoImage (back, "Items/back", true).Forget ();
                Services.Assets.SetSpriteIntoImage(_icon, "Items/" + reward.Id + "/icon", true).Forget();
                break;
            // case DataService.BUILDING_ID:
            //     Services.Assets.SetSpriteIntoImage(_icon, "Buildings/" + reward.Id + "/icon", true).Forget();
            //     break;
            // case DataService.ACTION_ID:

            //     Services.Assets.SetSpriteIntoImage(back, "Actions/back", true).Forget();
            //     Services.Assets.SetSpriteIntoImage(_icon, "Actions/" + reward.Id + "/icon", true).Forget();
            //     _count.gameObject.SetActive(false);
            //     break;

            default:
                this.gameObject.SetActive(false);
                break;
        }

    }

    public bool IsEmpty()
    {
        return _isEmpty;
    }

    public virtual void Clear()
    {
        _data = null;
        _isEmpty = true;

        _icon.sprite = null;

        this.gameObject.SetActive(false);

        //if (tooltip != null)
        //    showTooltipBtn.interactable = false;
    }

    void Start()
    {
        // if (tooltip != null)
        //     showTooltipBtn.onClick.AddListener(OnClick);
    }
    protected virtual void Awake()
    {
        Clear();
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