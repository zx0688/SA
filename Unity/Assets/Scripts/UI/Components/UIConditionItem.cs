﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class UIConditionItem : MonoBehaviour, ISetData<ItemTypeData>
    {
        [SerializeField] private Text count;
        [SerializeField] private Image icon;

        public ItemTypeData Data { get; private set; }

        public void SetItem(ItemTypeData cond)
        {
            if (cond == null)
            {
                Hide();
                return;
            }

            this.gameObject.SetActive(true);

            if (cond.Type == ConditionMeta.ITEM)
            {
                count.text = Math.Abs(cond.Count).ToString();
                count.gameObject.SetActive(cond.Count != 0);
            }
            else if (cond.Type == ConditionMeta.CARD)
            {
                count.gameObject.SetActive(false);
            }

            count.gameObject.SetActive(false);
            Data = cond;

            if (cond.Type == ConditionMeta.ITEM)
            {
                icon.LoadItemIcon(cond.Id);
            }
            else
            {
                icon.LoadItemIcon(cond.Id);
            }
        }
        public virtual void Hide()
        {
            icon.sprite = null;
            this.gameObject.SetActive(false);
        }

        protected virtual void Awake()
        {
            Hide();
        }
    }
}