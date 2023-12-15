using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using haxe.root;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;

public class UITargetItem : MonoBehaviour
{

    [SerializeField] private Text value;
    [SerializeField] private Image icon;
    [SerializeField] private Text target;
    [SerializeField] private Text targetName;
    [SerializeField] private Image check;
    [SerializeField] private GameObject back;

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
        value.enabled = true;
        target.enabled = true;
        targetName.gameObject.SetActive(false);

        if (trigger != null && trigger.Type == TriggerMeta.ITEM)
        {
            target.Localize(trigger.Count < 0 ? "Trigger.Spend" : "Trigger.Add", LocalizePartEnum.GUI);
            back.gameObject.SetActive(false);
            icon.LoadItemIcon(trigger.Id);
        }
        else if (trigger != null && trigger.Type == TriggerMeta.CARD)
        {
            CardMeta cardMeta = Services.Meta.Game.Cards[trigger.Id];
            target.text = "Trigger.Move".Localize(LocalizePartEnum.GUI);
            targetName.text = cardMeta.Name;
            targetName.gameObject.SetActive(true);
            back.gameObject.SetActive(false);
            icon.LoadCardImage(cardMeta.Image);
        }
        else if (condition != null && condition.Type == ConditionMeta.ITEM)
        {
            if (!Services.Player.Profile.Items.TryGetValue(condition.Id, out ItemData current))
                current = new ItemData(condition.Id, 0);

            back.gameObject.SetActive(true);
            target.Localize("Condition.Collect", LocalizePartEnum.GUI);
            value.text = ZString.Format("{0}/{1}", current.Count, condition.Sign == ">=" ? condition.Count : condition.Count + 1);
            icon.LoadItemIcon(condition.Id);

        }
        else
        {
            throw new Exception("unexpected target");
        }
        value.color = condition != null && condition.Check() ? (colors == null || colors.Length == 0 ? Color.green : colors[0]) : (colors == null || colors.Length == 0 ? Color.red : colors[1]);
        check.gameObject.SetActive(condition != null && condition.Check());

        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        icon.enabled = false;
        icon.sprite = null;

        value.enabled = false;
        target.enabled = false;
        gameObject.SetActive(false);
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