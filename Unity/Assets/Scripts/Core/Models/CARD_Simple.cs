using System;
using System.Collections;
using System.Collections.Generic;

using Core;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;
using UI.Components;

namespace Core
{
    public class CARD_Simple : MonoBehaviour, ICard
    {
        private SwipeData data = default!;

        [SerializeField] private Image art;
        [SerializeField] private Text name;
        [SerializeField] private GameObject eventIcon;

        private CardMeta card => data.Card;

        public void SetActive(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public void UpdateData(SwipeData data)
        {
            this.data = data;
            UpdateHUD();
        }

        protected void UpdateHUD()
        {
            // description.gameObject.SetActive(false);
            art.gameObject.SetActive(false);

            name.Localize(card.Name);

            if (card.Desc != null)
            {
                //description.Localize(card.Desc);
                //description.gameObject.SetActive(true);
            }

            if (card.Image != null)
            {
                art.LoadCardImage(card.Image);
                art.gameObject.SetActive(true);
            }

            //eventIcon.SetActive(card.Event == true);
        }
    }

}