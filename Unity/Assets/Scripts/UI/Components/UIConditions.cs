using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleLocalization;
using Core;
using Meta;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    public class UIConditions : MonoBehaviour, ISetData<List<ConditionMeta>>
    {
        [SerializeField] private LocalizedText header;
        [SerializeField] private UIConditionItem[] items;

        public List<ConditionMeta> Data { get; private set; }


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
            if (Data == null || Data.Count == 0)
                return false;

            gameObject.SetActive(true);
            return true;
        }

        public void SetItem(List<ConditionMeta> data)
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
                if (i < data.Count)
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