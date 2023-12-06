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
    public class UIActionPanel : MonoBehaviour
    {
        [SerializeField] private UI_Reward reward;
        [SerializeField] private UIChoice choice;
        //[SerializeField] private Text skipText;
        private int ch = -10;

        private SwipeData data;

        void Awake()
        {
            Swipe.OnChangeDeviation += OnChangeDeviation;
            Swipe.OnDrop += OnDrop;
            Swipe.OnTakeCard += OnTakeCard;
            Swipe.OnEndSwipe += Hide;
        }

        void Start()
        {
            Hide();
            //Services.OnInited += () => { skipText.text = "Продолжить".Localize(); };
        }

        void OnTakeCard()
        {

            if (data.Left != null && data.Left.Id == data.Right.Id)
            {
                ch = -1;
                choice.Show(data.Left);
                reward.SetItems(data.Left.Reward);
            }
            else if (data.Right == null && data.Left == null)
            {
                choice.Hide();
                ch = -1;
                //action.gameObject.SetActive(false);
                //icon.gameObject.SetActive(false);
                // skipText.gameObject.SetActive(true);
            }

            //if (data == null || Swipe.State != Swipe.States.DRAG) return;

            //OnChangeDirection(CardMeta.LEFT);

            //gameObject.SetActive(true);
            //canvasGroup.DOFade(1f, 0.1f);
        }

        void OnDrop()
        {
            Hide();
            ch = -10;
            // canvasGroup.DOFade(0.4f, 0.05f).OnComplete(() =>
            // {
            //     reward.SetItems(null);
            //     //action.gameObject.SetActive(false);
            //     //gameObject.SetActive(false);
            // });
        }

        void Hide()
        {
            reward.SetItems(null);
            choice.Hide();
            ch = -10;
            //skipText.gameObject.SetActive(false);
            //gameObject.SetActive(false);
            //canvasGroup.DOKill();
        }

        void OnChangeDeviation(float dev)
        {
            if (data == null || Swipe.State != Swipe.States.DRAG) return;

            //only one choice
            if (ch == -1)
                return;
            else if (Math.Abs(dev) < 0.9)
            {
                choice.Hide();
                reward.SetItems(null);
                ch = -10;
                return;
            }
            else if (dev < -0.9f)
            {
                if (ch == CardMeta.LEFT)
                    return;
                ch = CardMeta.LEFT;
            }
            else if (dev > 0.9f)
            {
                if (ch == CardMeta.RIGHT)
                    return;
                ch = CardMeta.RIGHT;
            }

            if (data.Left != null && ch == CardMeta.LEFT)
            {
                choice.Show(data.Left);
                reward.SetItems(data.Left.Reward);
            }
            else if (data.Right != null && ch == CardMeta.RIGHT)
            {
                choice.Show(data.Right);
                reward.SetItems(data.Right.Reward);
            }

        }

        public void UpdateData(SwipeData data)
        {
            this.data = data;
            ch = -10;
            Hide();
        }

    }
}
