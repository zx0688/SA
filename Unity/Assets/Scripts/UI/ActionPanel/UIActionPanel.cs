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
using System.Drawing;

namespace UI.ActionPanel
{
    public class UIActionPanel : MonoBehaviour
    {

        [SerializeField] private UIReward reward;
        [SerializeField] private Image image;
        [SerializeField] private Image hero;
        [SerializeField] private UIConditions conditions;
        [SerializeField] private Text description;
        [SerializeField] private Text action;
        [SerializeField] private GameObject choicePanel;

        [SerializeField] private GameObject leftArrow;
        [SerializeField] private GameObject rightArrow;



        [SerializeField] private List<Color32> colors;

        private int choice = -10;

        private SwipeData data;

        void Awake()
        {
            Swipe.OnChangeDeviation += OnChangeDeviation;
            Swipe.OnDrop += OnDrop;
            Swipe.OnTakeCard += OnTakeCard;
            Swipe.OnEndSwipe += Hide;

            leftArrow.gameObject.SetActive(false);
            rightArrow.gameObject.SetActive(false);
        }

        void OnTakeCard()
        {
            conditions.Hide();



            if (data.Card.Type == CardMeta.TYPE_QUEST)
            {
                choice = -1;
                description.gameObject.SetActive(false);
            }
            else if (data.Left == null && data.Right == null || (data.Left.Id == Services.Player.Profile.Deck.Last()))
            {
                choice = -1;
                HideChoice();
            }
            else if (data.Left.Id == data.Right.Id)
            {
                choice = -1;
                if (data.Card.Hero != null && data.Card.Hero == data.Left.Hero)
                {
                    description.gameObject.SetActive(true);
                    description.text = data.Left.Name.Localize(LocalizePartEnum.CardName);
                }
                else
                {
                    ShowChoice(data.Left);
                    description.gameObject.SetActive(false);
                }
            }
            else
            {
                HideChoice();
                description.gameObject.SetActive(false);
            }

        }

        void OnDrop()
        {
            choice = -10;
            conditions.Hide();
            reward.Hide();

            HideChoice();

            if (data.Card.Type == CardMeta.TYPE_QUEST)
            {
                action.gameObject.SetActive(true);
                action.Localize("Quest.NewQuest", LocalizePartEnum.GUI);
                action.color = colors[1];
                choicePanel.gameObject.SetActive(true);
            }
            else
            {
                description.gameObject.SetActive(true);
                description.text = data.Card.Desc.Localize(LocalizePartEnum.CardDescription);
            }

            if (data.Conditions.Count > 0)
                conditions.SetItem(data.Conditions);
        }



        public void Show(SwipeData data)
        {
            this.data = data;

            OnDrop();

            leftArrow.gameObject.SetActive(true);
            rightArrow.gameObject.SetActive(false);

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            conditions.Hide();
            reward.Hide();
            description.gameObject.SetActive(false);
            leftArrow.gameObject.SetActive(false);
            rightArrow.gameObject.SetActive(false);

            HideChoice();

            gameObject.SetActive(false);
        }

        private void ShowChoice(CardMeta ch)
        {
            choicePanel.gameObject.SetActive(true);
            if (ch.Reward != null && ch.Reward.Length > 0)
                reward.SetItems(ch.Reward);
            else
                reward.Hide();


            image.LoadCardImage(ch.Image);
            image.gameObject.SetActive(true);

            if (ch.Hero != null)
            {
                hero.LoadHeroImage(ch.Hero);
                hero.gameObject.SetActive(true);
            }
            else
            {
                hero.gameObject.SetActive(false);
            }


            action.Localize(ch.Name, LocalizePartEnum.CardName);
            action.gameObject.SetActive(true);
            action.color = colors[0];
        }

        private void HideChoice()
        {
            action.gameObject.SetActive(false);
            reward.Hide();
            choicePanel.gameObject.SetActive(false);
            image.gameObject.SetActive(false);
            hero.gameObject.SetActive(false);
        }

        void OnChangeDeviation(float dev)
        {
            if (data == null || Swipe.State != Swipe.States.DRAG || choice == -1) return;

            else if (Math.Abs(dev) < 0.9)
            {
                HideChoice();
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
                    HideChoice();
                }
                else
                {
                    description.gameObject.SetActive(false);
                    ShowChoice(data.Left);
                }
            }
            else if (data.Right != null && choice == CardMeta.RIGHT)
            {
                if (data.Right.Image == null)
                {
                    description.gameObject.SetActive(true);
                    description.text = data.Right.Desc.Localize(LocalizePartEnum.CardDescription);
                    HideChoice();
                }
                else
                {
                    description.gameObject.SetActive(false);
                    ShowChoice(data.Right);
                }
            }

        }

    }
}
