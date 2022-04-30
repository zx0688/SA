using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class UI_AchivementItem : UI_InventoryItem, ITick
{


    public override void SetItem(ItemVO item)
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

        data = Services.Data.ItemInfo(item.id);
        icon.enabled = true;
        count.enabled = true;

        Tick(GameTime.Current);

        if (tooltip != null)
            showTooltipBtn.interactable = true;

        Services.Assets.SetSpriteIntoImage(icon, "Items/" + item.id + "/icon", true).Forget();
    }

}