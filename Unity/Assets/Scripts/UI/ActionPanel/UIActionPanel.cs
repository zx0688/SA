using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Core;
using System.Data;

namespace UI.ActionPanel
{
    public class UIActionPanel : MonoBehaviour
    {
        [SerializeField] private UI_Reward reward;
        [SerializeField] private Text action;
        [SerializeField] private Image icon;
        [SerializeField] private Text skipText;

        private CanvasGroup canvasGroup;
        private SwipeData data;

        void Awake()
        {
            Swipe.OnChangeDirection += OnChangeDirection;
            Swipe.OnDrop += OnDrop;
            Swipe.OnTakeCard += OnTakeCard;
            Swipe.OnEndSwipe += Hide;

            canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            Hide();
            Services.OnInited += () => { skipText.text = "Продолжить".Localize(); };
            canvasGroup.alpha = 0f;
        }

        void OnTakeCard()
        {
            if (data == null || Swipe.State != Swipe.States.DRAG) return;

            OnChangeDirection(CardMeta.LEFT);

            gameObject.SetActive(true);
            canvasGroup.DOFade(1f, 0.1f);
        }

        void OnDrop()
        {
            canvasGroup.DOFade(0.4f, 0.05f).OnComplete(() =>
            {
                reward.SetItems(null);
                //action.gameObject.SetActive(false);
                gameObject.SetActive(false);
            });
        }

        void Hide()
        {
            reward.SetItems(null);
            skipText.gameObject.SetActive(false);
            gameObject.SetActive(false);
            canvasGroup.DOKill();
        }

        void OnChangeDirection(int direction)
        {
            if (data == null || Swipe.State != Swipe.States.DRAG) return;

            action.gameObject.SetActive(true);
            icon.gameObject.SetActive(true);
            skipText.gameObject.SetActive(false);

            if (data.LastCard)
            {
                action.text = "Привал".Localize();
                icon.LoadCardImage("endturn");
            }
            else if (data.Right == null && data.Left == null)
            {
                action.gameObject.SetActive(false);
                icon.gameObject.SetActive(false);
                skipText.gameObject.SetActive(true);
            }
            else if (direction == CardMeta.LEFT || data.Right == null)
            {
                action.text = data.Left.Name.Localize();
                icon.LoadCardImage(data.Left.Image);
                reward.SetItems(data.Left.Reward);
            }
            else
            {
                action.text = data.Right.Name.Localize();
                icon.LoadCardImage(data.Right.Image);
                reward.SetItems(data.Right.Reward);
            }



        }

        public void UpdateData(SwipeData data)
        {
            this.data = data;
            Hide();
        }

    }
}
