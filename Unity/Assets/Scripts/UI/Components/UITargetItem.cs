using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using haxe.root;
using UnityEngine;
using UnityEngine.UI;

public class UITargetItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text target;
    [SerializeField] private Text targetName;
    [SerializeField] private Image check;
    [SerializeField] private GameObject border;


    public void SetItem(ConditionMeta condition, TriggerMeta trigger, Color32[] colors = null)
    {
        if (condition == null && trigger == null)
        {
            Hide();
            return;
        }
        // if (this.data != null && this.data.Id == condition.Id && this.data.Type == condition.Type)
        //     return;
        icon.enabled = true;
        target.enabled = true;

        if (trigger != null && trigger.Type == TriggerMeta.ITEM)
        {
            target.Localize(trigger.Count < 0 ? "Trigger.Spend" : "Trigger.Add", LocalizePartEnum.GUI);
            icon.LoadItemIcon(trigger.Id);
            border.SetActive(false);
        }
        else if (trigger != null && trigger.Type == TriggerMeta.CARD)
        {
            CardMeta cardMeta = Services.Meta.Game.Cards[trigger.Id];
            target.text = "Trigger.Locate".Localize(LocalizePartEnum.GUI);
            targetName.text = cardMeta.Name;
            icon.LoadCardImage(cardMeta.Image);
            border.SetActive(true);
        }
        else if (condition != null && condition.Type == ConditionMeta.ITEM)
        {
            if (!Services.Player.Profile.Items.TryGetValue(condition.Id, out ItemData current))
                current = new ItemData(condition.Id, 0);
            targetName.text = ZString.Format("{0}/{1}", current.Count, condition.Sign == ">=" ? condition.Count : condition.Count + 1);

            target.Localize("Condition.Have", LocalizePartEnum.GUI);
            icon.LoadItemIcon(condition.Id);
            border.SetActive(false);
        }
        else
        {
            throw new Exception("unexpected target");
        }
        check.gameObject.SetActive(condition != null && condition.Check());

        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        icon.enabled = false;
        icon.sprite = null;

        target.enabled = false;
        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        //tooltip.ShowTooltip (data);
    }


    public void SetTooltip(UIInventoryTooltip tooltip)
    {
        //this.tooltip = tooltip;
    }
}