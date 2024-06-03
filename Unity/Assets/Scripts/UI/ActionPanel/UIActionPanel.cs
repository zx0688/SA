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

        [SerializeField] private UIReward randomReward;

        [SerializeField] private Text description;
        [SerializeField] private GameObject descPanel;

        [SerializeField] private GameObject delem;
        [SerializeField] private UICurrent backpack;
        [SerializeField] private Text next;

        [SerializeField] private List<Color32> colors;

        private bool choiceble = false;
        private int choice = -10;
        private float threshold = 0.1f;
        //private bool hasNextText = false;
        private ProfileData profile => Services.Player.Profile;


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

            next.transform.localScale = new Vector3(1f, 1f, 1f);
            next.gameObject.SetActive(true);
            next.text = "Дальше";

            int currentCardState = SL.GetCurrentState(profile);
            if (currentCardState == CardData.NOTHING)
            {
                conditions.SetItem(data.Conditions);
                if (data.Card.IfNothing == null)
                    throw new Exception("Description is needed if choice unavailable");

                SetDecription(data.Card.IfNothing[data.Card.IfNothing.Length - profile.CardStates.Where(c => c == CardData.NOTHING).ToList().Count]);
            }
            else if (currentCardState == CardData.ONLY_ONCE)
            {
                SetDecription(data.Card.OnlyOnce[data.Card.OnlyOnce.Length - profile.CardStates.Where(c => c == CardData.ONLY_ONCE).ToList().Count]);
            }
            else if (currentCardState == CardData.DESCRIPTION)
            {
                SetDecription(data.Card.Descs[data.Card.Descs.Length - profile.CardStates.Where(c => c == CardData.DESCRIPTION).ToList().Count]);
            }
            else if (currentCardState == CardData.REWARD)
            {
                randomReward.SetItems(data.Card.Reward != null ? data.Card.Reward[0] : null, data.Card.Cost != null ? data.Card.Cost[0] : null, false);

                if (Services.Player.RewardCollected.Count > 0)
                {
                    backpack.gameObject.SetActive(true);
                    backpack.SetItems(Services.Player.RewardCollected, profile, Services.Meta.Game);

                    if (data.Card.RewardText == null)
                        throw new Exception($"Description is needed for reward {data.Card.Id}");
                    SetDecription(data.Card.RewardText);
                }
                else
                    SetDecription("noint1");

            }
            else if (currentCardState == CardData.CHOICE)
            {
                conditions.SetItem(data.Conditions);

                if (data.Left == null && data.Right == null)
                {
                    throw new Exception($"State Choice should have any next card {data.Card.Id}");
                }
                else if (data.Left.Id == data.Right.Id)
                {
                    left.ShowChoice(data.Left, profile.Left, data.FollowPrompt == CardMeta.LEFT);
                }
                else
                {
                    choiceble = true;
                    left.ShowChoice(data.Left, profile.Left, data.FollowPrompt == CardMeta.LEFT);
                    right.ShowChoice(data.Right, profile.Right, data.FollowPrompt == CardMeta.RIGHT);
                    delem.SetActive(true);
                }
            }
            else
            {
                throw new Exception($"Card {data.Card.Id} has no state");
            }
        }


        void OnTakeCard()
        {
            if (data.Card == null || data.Card.Type != CardMeta.TYPE_CARD)
                return;

            if (next.gameObject.activeInHierarchy)
                next.GetComponent<RectTransform>().DOScale(1.2f, 0.15f);

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

            if (next.gameObject.activeInHierarchy)
                next.GetComponent<RectTransform>().DOScale(1.0f, 0.15f);

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
        }

        public void Hide()
        {
            //conditions.Hide();
            //rewardLeft.Hide();
            //description.gameObject.SetActive(false);
            //followPrompt.gameObject.SetActive(false);
            HideAll();

            gameObject.SetActive(false);
        }

        private void SetDecription(string desc)
        {
            descPanel.gameObject.SetActive(true);

            //description.gameObject.SetActive(true);
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
            conditions.Hide();
            randomReward.Hide();
            backpack.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
            next.GetComponent<RectTransform>().DOKill();

            left.HideAll();
            right.HideAll();
            delem.SetActive(false);

            descPanel.gameObject.SetActive(false);
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
