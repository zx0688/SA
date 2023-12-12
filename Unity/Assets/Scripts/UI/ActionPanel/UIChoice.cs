using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Core;
using System.Data;
using Cysharp.Text;

namespace UI.ActionPanel
{
    public class UIChoice : MonoBehaviour
    {
        [SerializeField] private Text action;
        [SerializeField] private Image icon;
        [SerializeField] private Image hero;

        [SerializeField] private CanvasGroup canvasGroup;

        public void Hide()
        {
            //canvasGroup.DOKill();

            //canvasGroup.DOFade(0, 0.2f);
            gameObject.SetActive(false);
        }

        public void Show(CardMeta meta)
        {
            action.text = meta.Name.Localize(LocalizePartEnum.CardName);
            icon.LoadCardImage(meta.Image);

            if (meta.Hero != null)
            {
                hero.LoadHeroImage(meta.Hero);
                hero.gameObject.SetActive(true);
            }
            else
            {
                hero.gameObject.SetActive(false);
            }

            //canvasGroup.DOKill();
            //canvasGroup.alpha = 0f;
            //canvasGroup.DOFade(1f, 0.2f);
            gameObject.SetActive(true);
        }
    }
}
