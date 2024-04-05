using System;
using System.Collections;
using System.Collections.Generic;

using haxe.root;
using UnityEngine;
using UnityEngine.UI;

public class UITarget : MonoBehaviour
{
    [SerializeField] public UITargetItem[] items;
    [SerializeField] public Color32[] colors;

    public void SetItems(ConditionMeta[][] conditions, TriggerMeta[] triggers)
    {
        if ((conditions == null || conditions.Length == 0) && (triggers == null || triggers.Length == 0))
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        for (int i = 0; i < items.Length; i++)
        {
            UITargetItem item = items[i];
            if (triggers != null && i < triggers.Length)
            {
                TriggerMeta r = triggers[i];
                item.SetItem(null, r, colors);
            }
            else if (conditions != null && ((triggers == null && i < conditions.Length) || (triggers != null && i - triggers.Length < conditions.Length)))
            {
                int ii = triggers == null ? i : i - triggers.Length;
                ConditionMeta r = conditions[ii][0];
                item.SetItem(r, null, colors);
            }
            else
            {
                item.Hide();
            }
        }

    }

}