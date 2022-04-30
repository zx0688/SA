using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class UI_SkillItem : MonoBehaviour, ITick
{

    public int id;
    public int type;
    public Button showTooltipBtn;

    public Image icon;

    protected SkillData data;
    protected bool isEmpty;
    protected UI_SkillTooltip tooltip;

    protected Text timer;

    protected GameObject timerPanel;
    protected Image[] stars;

    public virtual void SetItem(SkillVO item)
    {

        if (item == null)
        {
            Clear();
            return;
        }

        if (id > 0 && id != item.id)
            return;

        if (this.data != null && this.data.Id == item.id)
            return;

        this.data = Services.Data.SkillInfo(item.id);

        if (type > 0 && this.data.Type != type)
            return;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(i < item.count && item.count > 1);
        }

        isEmpty = false;
        icon.enabled = true;

        if (tooltip != null)
            showTooltipBtn.interactable = true;

        Services.Assets.SetSpriteIntoImage(icon, "Skills/" + item.id + "/icon", true).Forget();
    }

    /*public void UpdateItem () {
        SkillVO skillVO = Services.Player.skillHandler.GetVO (id, type);
        SetItem (skillVO == null || skillVO.count == 0 ? null : skillVO);
    }*/

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

        foreach (Image i in stars)
            i.gameObject.SetActive(false);

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

        //count = transform.Find ("Count").GetComponent<Text> ();
        stars = transform.Find("Stars").GetComponentsInChildren<Image>();
        //isEmpty = true;
        Clear();
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

    public void SetTooltip(UI_SkillTooltip tooltip)
    {
        this.tooltip = tooltip;
    }
}