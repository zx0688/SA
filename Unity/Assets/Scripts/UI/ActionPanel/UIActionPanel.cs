using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Core;
using System.Data;
using UI.Components;
using System.Linq;
using System.Drawing;
using haxe.root;

namespace UI.ActionPanel
{
    public class UIActionPanel : MonoBehaviour
    {

        [SerializeField] private UIChoicePanel left;
        [SerializeField] private UIChoicePanel right;

        [SerializeField] private UIConditions conditions;
        [SerializeField] private Text description;
        [SerializeField] private GameObject delem;
        [SerializeField] private UICurrent backpack;


        [SerializeField] private List<Color32> colors;

        private bool choiceble = false;
        private int choice = -10;
        private float threshold = 0.1f;
        //private bool hasNextText = false;


        private SwipeData data;

        void Awake()
        {
            Swipe.OnChangeDeviation += OnChangeDeviation;
            Swipe.OnDrop += OnDrop;
            Swipe.OnTakeCard += OnTakeCard;
            Swipe.OnEndSwipe += Hide;

            Swipe.OnEndSwipe -= Hide;
            Swipe.OnEndSwipe -= Hide;
            Swipe.OnEndSwipe -= Hide;

            //followPrompt.gameObject.SetActive(false);
        }

        void OnSet()
        {
            choiceble = false;
            choice = -10;

            HideAll();

            //Services.Player.Profile.Cards.TryGetValue(data.Card.Id, out CardData cardData);

            // if (data.Card.Type == CardMeta.TYPE_QUEST && Services.Player.Profile.Cards.TryGetValue(data.Card.Id, out CardData cardData))
            // {
            //     description.gameObject.SetActive(true);
            //     description.text = data.Card.Name.Localize(LocalizePartEnum.CardName);
            //     description.color = colors[0];
            // }
            // else if (data.Card.Next.HasTriggers() && data.Card.Descs.HasTexts() && Services.Player.Profile.DialogIndex < data.Card.Descs.Length &&
            // (!Services.Player.Profile.Cards.TryGetValue(data.Card.Id, out cardData) || cardData.CT == 0))
            // {
            //     string desc = data.Card.Descs.GetCurrentDescription();
            //     if (desc.HasText()) SetDecription(desc);
            // }

            int currentCardState = SL.GetCurrentState(Services.Player.Profile);
            if (currentCardState == CardData.DESCRIPTION)
            {
                string desc = data.Card.Descs[data.Card.Descs.Length - Services.Player.Profile.CardStates.Where(c => c == 0).ToList().Count];
                SetDecription(desc);
            }
            else if (currentCardState == CardData.REWARD)
            {
                if (Services.Player.RewardCollected.Count > 0)
                {
                    backpack.gameObject.SetActive(true);
                    backpack.SetItems(Services.Player.RewardCollected, Services.Player.Profile, Services.Meta.Game);
                }
                else
                {
                    SetDecription("noint1");
                }
            }
            else if (currentCardState == CardData.CHOICE)
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
            else
            {
                Services.Meta.Game.Cards.TryGetValue("28393500", out CardMeta nextDefaultCard);
                left.ShowChoice(nextDefaultCard, data.FollowPrompt == CardMeta.LEFT);
            }
        }


        // if (data.Card.Next.HasTriggers() && data.Card.Descs.HasTexts() && Services.Player.Profile.DialogIndex < data.Card.Descs.Length &&
        // // (!Services.Player.Profile.Cards.TryGetValue(data.Card.Id, out cardData) || cardData.CT == 0))
        // // {
        // //     string desc = data.Card.Descs.GetCurrentDescription();
        // //     if (desc.HasText()) SetDecription(desc);
        // // }
        // if ((data.Left == null && data.Right == null) || (data.Left.Id == Services.Player.Profile.Deck.Last()))
        // {
        //     //string desc = data.Card.Descs.GetCurrentDescription();
        //     //if (desc.HasText()) SetDecription(desc);
        // }


        void OnTakeCard()
        {
            if (data.Card == null || data.Card.Type != CardMeta.TYPE_CARD)
                return;

            //if (backpack.gameObject.activeInHierarchy)
            //    backpack.TakeCard();

            if (data.Left != null && data.Left.Id == data.Right.Id)
            {
                left.FadeIn();
            }
            else if (data.Left == null && data.Right == null)
            {
                left.FadeIn();
            }

            choice = -10;
        }


        void OnDrop()
        {
            if (data.Card == null || data.Card.Type != CardMeta.TYPE_CARD)
                return;

            //if (backpack.gameObject.activeInHierarchy)
            //    backpack.DropCard();

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

        private void SetDecription(string desc)
        {
            description.gameObject.SetActive(true);
            description.text = desc.Localize(LocalizePartEnum.CardDescription);

            if (desc.Contains("ask"))
                description.color = colors[1];
            else if (desc.Contains("tell"))
                description.color = colors[2];
            else
                description.color = colors[0];
        }

        private void HideAll()
        {
            backpack.gameObject.SetActive(false);

            left.HideAll();
            right.HideAll();
            delem.SetActive(false);
            description.gameObject.SetActive(false);
        }

        void OnChangeDeviation(float dev)
        {
            if (data == null || Swipe.State != Swipe.States.DRAG || choiceble == false) return;

            else if (System.Math.Abs(dev) < threshold)
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
