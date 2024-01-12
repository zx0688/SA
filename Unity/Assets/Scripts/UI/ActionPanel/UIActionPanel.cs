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

        [SerializeField] private UIChoicePanel left;
        [SerializeField] private UIChoicePanel right;

        [SerializeField] private UIConditions conditions;
        [SerializeField] private Text description;
        [SerializeField] private GameObject delem;

        // [SerializeField] private GameObject choicePanel;


        [SerializeField] private List<Color32> colors;

        private bool choiceble = false;
        private int choice = -10;
        private float threshold = 0.1f;


        private SwipeData data;

        void Awake()
        {
            Swipe.OnChangeDeviation += OnChangeDeviation;
            Swipe.OnDrop += OnDrop;
            Swipe.OnTakeCard += OnTakeCard;
            Swipe.OnEndSwipe += Hide;

            //followPrompt.gameObject.SetActive(false);
        }

        void OnSet()
        {
            choiceble = false;
            choice = -10;

            HideAll();

            if (data.Card.Type == CardMeta.TYPE_QUEST && Services.Player.Profile.Cards.TryGetValue(data.Card.Id, out CardData cardData))
            {


                //                 action.gameObject.SetActive(true);
                //                 action.text = (cardData.Value == CardMeta.QUEST_SUCCESS ? "Quest.CompletedQuest" : "Quest.NewQuest").Localize().ToUpper();
                // 
                //                 action.color = colors[1];
                //                 choicePanel.gameObject.SetActive(true);

                description.gameObject.SetActive(true);
                description.text = data.Card.Name.Localize(LocalizePartEnum.CardName);
            }
            else if (data.Left == null && data.Right == null || (data.Left.Id == Services.Player.Profile.Deck.Last()))
            {
                if (data.Card.Desc.HasText())
                {
                    description.gameObject.SetActive(true);
                    description.text = data.Card.Desc.Localize(LocalizePartEnum.CardDescription);
                }
            }
            else
            {
                if (data.Left.Id == data.Right.Id)
                {

                    left.ShowChoice(data.Left, data.FollowPrompt == CardMeta.LEFT);

                }
                else
                {
                    choiceble = true;
                    left.ShowChoice(data.Left, data.FollowPrompt == CardMeta.LEFT);
                    right.ShowChoice(data.Right, data.FollowPrompt == CardMeta.RIGHT);
                    delem.SetActive(true);
                }


            }
        }

        void OnTakeCard()
        {
            if (data.Card == null || data.Card.Type != CardMeta.TYPE_CARD)
                return;

            if (data.Left != null && data.Left.Id == data.Right.Id)
            {
                // if (firstTake == true)
                // {
                //     left.ShowChoice(data.Left, data.FollowPrompt == CardMeta.LEFT);
                // }

                left.FadeIn();

            }
            else if (data.Left != null)
            {
                // choiceble = true;
                // left.ShowChoice(data.Left, data.FollowPrompt == CardMeta.LEFT);
                // right.ShowChoice(data.Right, data.FollowPrompt == CardMeta.RIGHT);
                //delem.SetActive(true);
            }
            else if (data.Left == null)
            {

            }

            choice = -10;
        }


        void OnDrop()
        {
            if (data.Card == null || data.Card.Type != CardMeta.TYPE_CARD)
                return;

            left.FadeOut();
            right.FadeOut();

            choice = -10;
        }



        public void Show(SwipeData data)
        {
            this.data = data;

            OnSet();

            gameObject.SetActive(true);

            if (data.Conditions.Count > 0)
                conditions.SetItem(data.Conditions);
            else
                conditions.Hide();

        }

        public void Hide()
        {
            //conditions.Hide();
            //rewardLeft.Hide();
            description.gameObject.SetActive(false);
            //followPrompt.gameObject.SetActive(false);
            HideAll();

            gameObject.SetActive(false);
        }

        private void HideAll()
        {
            left.HideAll();
            right.HideAll();
            delem.SetActive(false);
            description.gameObject.SetActive(false);
        }

        void OnChangeDeviation(float dev)
        {
            if (data == null || Swipe.State != Swipe.States.DRAG || choiceble == false) return;

            else if (Math.Abs(dev) < threshold)
            {
                if (choice == -10)
                    return;
                choice = -10;
            }
            else if (dev < -threshold)
            {
                if (choice == CardMeta.LEFT)
                    return;
                choice = CardMeta.LEFT;
            }
            else if (dev > threshold)
            {
                if (choice == CardMeta.RIGHT)
                    return;
                choice = CardMeta.RIGHT;
            }

            if (choice == -10)
            {
                left.FadeOut();
                right.FadeOut();
            }
            else if (choice == CardMeta.LEFT)
            {
                left.FadeIn();
                right.FadeOut();
            }
            else if (choice == CardMeta.RIGHT)
            {
                left.FadeOut();
                right.FadeIn();
            }


        }

    }
}
