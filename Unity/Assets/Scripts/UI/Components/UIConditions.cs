using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Core;

using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class UIConditions : MonoBehaviour, ISetData<List<ItemTypeData>>
    {
        [SerializeField] private UIConditionItem[] items;

        public List<ItemTypeData> Data { get; private set; }

        void Awake()
        {
            //Swipe.OnDrop += () => Show();
            //Swipe.OnTakeCard += Hide;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool Show()
        {
            if (Data == null || Data.Count == 0)
                return false;

            gameObject.SetActive(true);
            return true;
        }

        public void SetItem(List<ItemTypeData> data)
        {
            Data = data;

            if (!Show())
            {
                Hide();
                return;
            }

            for (int i = 0; i < items.Length; i++)
            {
                UIConditionItem item = items[i];
                if (i < data.Count)
                {
                    ItemTypeData c = data[i];
                    item.SetItem(c);
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