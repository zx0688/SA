using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Core;

using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class UIConditions : MonoBehaviour, ISetData<ConditionMeta[]>
    {
        [SerializeField] private UIConditionItem[] items;

        public ConditionMeta[] Data { get; private set; }

        void Awake()
        {
            Swipe.OnDrop += () => ShowIfAvailable();
            Swipe.OnTakeCard += Hide;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool ShowIfAvailable()
        {
            if (Data == null || Data.Length == 0)
                return false;

            gameObject.SetActive(true);
            return true;
        }

        public void SetItem(ConditionMeta[] data)
        {
            Data = data;

            if (!ShowIfAvailable())
            {
                Hide();
                return;
            }

            for (int i = 0; i < items.Length; i++)
            {
                UIConditionItem item = items[i];
                if (i < data.Length)
                {
                    item.SetItem(data[i]);
                }
                else
                {
                    item.Hide();
                    item.gameObject.SetActive(false);
                }
            }

        }

    }
}