using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Core;
using System.Data;
using Cysharp.Text;
using UI.Components;
using System.Linq;

namespace UI.ActionPanel
{
    public class UIActionPanel : MonoBehaviour
    {

        [SerializeField] private UIReward reward;
        [SerializeField] private UIChoice uiChoice;
        [SerializeField] private UIConditions conditions;
        [SerializeField] private Text description;

        private int choice = -10;

        private SwipeData data;

        void Awake()
        {
            Swipe.OnChangeDeviation += OnChangeDeviation;
            Swipe.OnDrop += OnDrop;
            Swipe.OnTakeCard += OnTakeCard;
            Swipe.OnEndSwipe += Hide;
        }

        void OnTakeCard()
        {
            conditions.Hide();

            if (data.Left == null && data.Right == null || (data.Left.Id == Services.Player.Profile.Deck.Last()))
            {
                choice = -1;
                reward.Hide();
                uiChoice.Hide();
            }
            else if (data.Left.Id == data.Right.Id)
            {
                choice = -1;
                reward.SetItems(data.Left.Reward);

                if (data.Card.Hero != null && data.Card.Hero == data.Left.Hero)
                {
                    description.gameObject.SetActive(true);
                    description.text = data.Left.Name.Localize(LocalizePartEnum.CardName);
                }
                else
                {
                    uiChoice.Show(data.Left);
                    description.gameObject.SetActive(false);
                }
            }
            else
            {
                reward.Hide();
                uiChoice.Hide();
                description.gameObject.SetActive(false);
            }

        }

        void OnDrop()
        {
            choice = -10;
            conditions.Hide();
            uiChoice.Hide();
            reward.Hide();

            description.gameObject.SetActive(true);
            description.text = data.Card.Desc.Localize(LocalizePartEnum.CardDescription);

            if (data.Conditions.Count > 0)
            {
                conditions.SetItem(data.Conditions);
            }
        }

        void OnChangeDeviation(float dev)
        {
            if (data == null || Swipe.State != Swipe.States.DRAG || choice == -1) return;

            else if (Math.Abs(dev) < 0.9)
            {
                uiChoice.Hide();
                reward.SetItems(null);
                choice = -10;
                return;
            }
            else if (dev < -0.9f)
            {
                if (choice == CardMeta.LEFT)
                    return;
                choice = CardMeta.LEFT;
            }
            else if (dev > 0.9f)
            {
                if (choice == CardMeta.RIGHT)
                    return;
                choice = CardMeta.RIGHT;
            }

            if (data.Left != null && choice == CardMeta.LEFT)
            {
                if (data.Left.Image == null)
                {
                    description.gameObject.SetActive(true);
                    description.text = data.Left.Desc.Localize(LocalizePartEnum.CardDescription);
                    uiChoice.Hide();
                }
                else
                {
                    description.gameObject.SetActive(false);
                    uiChoice.Show(data.Left);
                }
                reward.SetItems(data.Left.Reward);
            }
            else if (data.Right != null && choice == CardMeta.RIGHT)
            {
                if (data.Right.Image == null)
                {
                    description.gameObject.SetActive(true);
                    description.text = data.Right.Desc.Localize(LocalizePartEnum.CardDescription);
                    uiChoice.Hide();
                }
                else
                {
                    description.gameObject.SetActive(false);
                    uiChoice.Show(data.Right);
                }
                reward.SetItems(data.Right.Reward);
            }

        }

        public void Show(SwipeData data)
        {
            this.data = data;

            OnDrop();
        }

        public void Hide()
        {
            conditions.Hide();
            reward.Hide();
            description.gameObject.SetActive(false);
            uiChoice.Hide();
        }

    }
}
