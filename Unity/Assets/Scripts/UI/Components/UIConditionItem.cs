using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Meta;
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
            if (cond == null)
            {
                Hide();
                return;
            }

            if (cond.Tp == GameMeta.ITEM)
            {
                count.text = cond.Sign == ">" ? Math.Abs(cond.Count + 1).ToString() : Math.Abs(cond.Count).ToString();
                count.gameObject.SetActive(true);
            }
            else if (cond.Tp == GameMeta.SKILL)
            {
                count.gameObject.SetActive(false);
            }

            if (Data != null && Data.Id == cond.Id && Data.Tp == cond.Tp)
                return;

            Data = cond;
            this.gameObject.SetActive(true);

            //if (cond.Tp == GameMeta.ITEM)
            {
                //    Services.Assets.SetSpriteIntoImage(icon, "UI/randomItem", true).Forget();
            }
            //else
            {
                icon.SetImage(AssetsService.ITEM_ADDRESS(cond.Id));
            }
        }
        public virtual void Hide()
        {
            Data = null;
            icon.sprite = null;
            this.gameObject.SetActive(false);
        }

        protected virtual void Awake()
        {
            Hide();
        }
    }
}