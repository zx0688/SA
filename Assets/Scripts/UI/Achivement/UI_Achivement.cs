using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

public class UI_Achivement : UI_Inventory
{

    protected override void UpdateList()
    {

        if (!Services.isInited)
            return;

        List<ItemVO> items = new List<ItemVO>(Services.Player.playerVO.items);
        items = items.Where(i => Services.Data.ItemInfo(i.id).Type > 0).ToList();

        PageSwiper p = GetComponentInChildren<PageSwiper>();
        p.UpdateData(items);
    }
}