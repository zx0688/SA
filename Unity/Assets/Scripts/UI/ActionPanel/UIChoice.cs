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

        private CanvasGroup canvasGroup;

        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            Hide();
        }

        public void Hide()
        {
            canvasGroup.DOKill(false);
            canvasGroup.DOFade(0, 0.2f);
            gameObject.SetActive(false);
        }

        public void Show(CardMeta meta)
        {
            action.text = meta.Name.Localize(LocalizePartEnum.CardName);
            icon.LoadCardImage(meta.Image);

            canvasGroup.DOKill(false);
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.OutExpo);
            gameObject.SetActive(true);
        }
    }
}
