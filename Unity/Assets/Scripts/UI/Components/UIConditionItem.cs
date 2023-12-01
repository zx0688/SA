﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class UIConditionItem : MonoBehaviour, ISetData<ConditionMeta>
    {
        [SerializeField] private Text count;
        [SerializeField] private Image icon;

        public ConditionMeta Data { get; private set; }

        public void SetItem(ConditionMeta cond)
        {

            if (cond.Type == ConditionMeta.ITEM)
            {
                count.text = cond.Sign == ">" ? Math.Abs(cond.Count + 1).ToString() : Math.Abs(cond.Count).ToString();
                count.gameObject.SetActive(true);
            }
            else if (cond.Type == ConditionMeta.ITEM)
            {
                count.gameObject.SetActive(false);
            }

            if (Data.Id == cond.Id && Data.Type == cond.Type)
                return;

            Data = cond;
            this.gameObject.SetActive(true);

            //if (cond.Tp == ConditionMeta.ITEM)
            {
                //    Services.Assets.SetSpriteIntoImage(icon, "UI/randomItem", true).Forget();
            }
            //else
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