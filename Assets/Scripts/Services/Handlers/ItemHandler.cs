using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta;
using UnityEngine;

public class ItemHandler
{

    private PlayerVO player;
    private DataService dataManager;

    //delegate bool ActionTrigger(TriggerVO trigger, ActionData data, int startTime, int timestamp);

    //private ActionTrigger actionTrigger;
    public ItemHandler(PlayerVO playerVO)
    {
        this.player = playerVO;
        dataManager = Services.Data;
        //  actionTrigger = Services.Data.ActionTrigger;
    }

    public int AvailableItem(int id, int type = -1)
    {
        InventoryVO b = player.items.Find(_r => _r.id == id);
        return b != null ? b.count : 0;
    }

    public InventoryVO Add(ItemData data, int count, int time)
    {

        ItemData d = dataManager.ItemInfo(data.Id);

        InventoryVO current = null;
        current = player.items.Find(_r => _r.id == data.Id);
        if (current == null)
        {
            current = new InventoryVO(data.Id, 0);
            player.items.Add(current);
        }
        InventoryVO r = new InventoryVO(data.Id, current.id);
        int max = 9999999;
        if (count + current.count < 0)
            r.count = -current.count;
        else if (count + current.count > max)
            r.count = max - current.count;
        else
            r.count = count;
        current.count += count;
        if (current.count < 0)
            current.count = 0;

        return r;
    }

    /*public void Trigger(List<CardData> queue, TriggerVO trigger, List<RewardData> reward, int time)
    {

        if (trigger.tp == TriggerData.ITEM)
        {
            foreach (RewardData r in trigger.reward)
            {
                if (r.Tp == DataService.ITEM_ID && r.Id == ItemData.EXP_ID)
                {
                    //InventoryVO level = GetSkill (2);
                    /*ExpData expd = Array.Find (Services.Data.game.exp, e => e.id == level.count + 1);
                    InventoryVO expVO = GetSkill (1);

                    if (expd.count <= expVO.count) {
                        RewardData rd = new RewardData ();
                        rd.tp = DataManager.ITEM_ID;
                        rd.id = ItemData.LEVEL_ID;
                        rd.count = 1;
                        Services.Data.ApplyReward (reward, new List<RewardData> () { rd }, 1f);
                        queue.Add (CardCreator.NewLevel (expd));
                    }

                }

            }
        }

    }*/

    public InventoryVO GetVO(int id, int type)
    {
        for (int i = 0; i < player.items.Count; i++)
        {
            if (player.items[i].id == id)
                return player.items[i];
        }
        return null;
    }

    public InventoryVO Change(ItemData data, int time)
    {
        throw new System.NotImplementedException();
    }
}