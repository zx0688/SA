using System;
using System.Collections;
using System.Collections.Generic;
using Meta;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Core;

namespace UI.ActionPanel
{
    public class UIActionPanel : MonoBehaviour
    {
        [SerializeField] private UI_Reward reward;
        [SerializeField] private Text action;
        [SerializeField] private Image icon;

        private CanvasGroup canvasGroup;
        private SwipeData data;

        void Awake()
        {
            Swipe.OnChangeDirection += OnChangeDirection;
            Swipe.OnDrop += OnDrop;
            Swipe.OnTakeCard += OnTakeCard;
            Swipe.OnEndSwipe += OnForceHide;

            canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            OnDrop();

            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        void OnTakeCard()
        {
            OnChangeDirection(Swipe.LEFT_CHOICE);

            gameObject.SetActive(true);
            canvasGroup.DOFade(1f, 0.15f);
        }

        void OnDrop()
        {
            canvasGroup.DOFade(0f, 0.15f).OnComplete(() =>
            {
                reward.SetItems(null);
                //action.gameObject.SetActive(false);
                gameObject.SetActive(false);
            });
        }

        void OnForceHide()
        {
            reward.SetItems(null);
            gameObject.SetActive(false);
        }

        void OnChangeDirection(int direction)
        {
            if (data == null || Swipe.State != Swipe.States.DRAG) return;

            action.text = data.GetChoiceData(direction).Text;

            string card = data.GetChoiceData(direction).Icon;
            icon.enabled = card != null;
            icon.SetImage(AssetsService.CARD_ADDRESS(card));
            reward.SetItems(data.GetChoiceData(direction).Reward);
        }

        public void UpdateData(SwipeData data)
        {
            this.data = data;
            OnDrop();
        }

    }
}
