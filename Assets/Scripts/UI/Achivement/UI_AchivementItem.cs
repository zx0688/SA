using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class UI_AchivementItem : UI_InventoryItem, ITick
{


    /*public override void SetItem(ItemVO item)
    {

        if (item == null)
        {
            Clear();
            return;
        }

        //count.text = item.Count.ToString();
        isEmpty = false;

        if (this.data != null && this.data.Id == item.Id)
            return;

        data = Services.Data.ItemInfo(item.Id);
        Icon.enabled = true;
        //count.enabled = true;

        Tick(GameTime.Current);

        if (tooltip != null)
            ShowTooltipBtn.interactable = true;

        Services.Assets.SetSpriteIntoImage(Icon, "Items/" + item.Id + "/icon", true).Forget();
    }*/

}