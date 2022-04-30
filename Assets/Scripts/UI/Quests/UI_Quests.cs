using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class UI_Quests : MonoBehaviour
{
    public UI_QuestsTooltip tooltip;
    public UI_QuestsItem[] items;

    public UI_TickChildren tick;

    void Awake()
    {

        items = GetComponentsInChildren<UI_QuestsItem>();

        foreach (UI_QuestsItem item in items)
        {
            item.SetTooltip(tooltip);
        }
    }
    void Start()
    {
        tooltip.HideTooltip();
    }

    void OnEnable()
    {
        UpdateList();
    }
    void UpdateList()
    {

        if (!Services.isInited)
            return;

        List<QuestVO> quests = Services.Player.playerVO.quests;
        int time = GameTime.Current;

        foreach (QuestVO q in quests)
        {

            UI_QuestsItem slot = Array.Find(items, i => i.GetId() == q.id);
            // QuestData questData = Services.Data.QuestInfo (q.id);

            if (q.state == QuestVO.STATE_ACTIVE)
            {
                if (slot != null)
                    continue;
                slot = Array.Find(items, i => i.IsEmpty());
                if (slot == null)
                    continue;
                slot.SetItem(q);
            }
            else if (slot != null)
            {
                slot?.Clear();
            }
        }

        tick.UpdateTickList();

    }
}