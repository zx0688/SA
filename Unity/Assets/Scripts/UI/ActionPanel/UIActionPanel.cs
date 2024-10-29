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
using Random = UnityEngine.Random;
using UnityEditor;
using UnityJSON;
using UnityEngine.Assertions.Must;

namespace UI.ActionPanel
{
    public class UIActionPanel : MonoBehaviour
    {
        [SerializeField] private Text name;
        [SerializeField] private UIReward needed;
        //[SerializeField] private UIChoicePanel left;
        //[SerializeField] private UIChoicePanel right;

        [SerializeField] private UIReward randomReward;

        [SerializeField] private Text description;
        [SerializeField] private GameObject descPanel;

        //[SerializeField] private GameObject delem;
        [SerializeField] private UICurrent backpack;
        //[SerializeField] private Text next;

        [SerializeField] private List<Color32> colors;

        private Swipe swipe;

        private ProfileData profile => Services.Player.Profile;


        private SwipeData data;

        void Awake()
        {


            //followPrompt.gameObject.SetActive(false);
        }

        //         void AddListeners()
        //         {
        //             swipe.OnChangeDeviation += OnChangeDeviation;
        //             swipe.OnDrop += OnDrop;
        //             swipe.OnTakeCard += OnTakeCard;
        //             swipe.OnEndSwipe += Hide;
        //         }
        // 
        //         void RemoveListeners()
        //         {
        //             swipe.OnChangeDeviation -= OnChangeDeviation;
        //             swipe.OnDrop -= OnDrop;
        //             swipe.OnTakeCard -= OnTakeCard;
        //             swipe.OnEndSwipe -= Hide;
        //         }

        void OnSet()
        {
            HideAll();

            name.Localize(data.Card.Name, LocalizePartEnum.CardName);

            DeckItem deckItem = SL.GetCurrentCard(profile);
            if (deckItem.State == CardData.NOTHING)
            {
                if (data.Card.Next.HasTriggers())
                {
                    var cards = new List<CardMeta>();
                    Services.Meta.GetAllRecursiveCardsFromGroup(data.Card.Next, cards);
                    RewardMeta[] cost = cards.SelectMany(c => c.Cost[0]).ToArray();
                    needed.SetItems(null, cost, false);
                }
                SetDecription(data.Card.IfNothing[(data.Card.IfNothing.Length - 1) - deckItem.DescIndex]);
            }
            else if (deckItem.State == CardData.DESCRIPTION)
            {
                if (data.Card.OnlyOnce != null && (!profile.Cards.TryGetValue(data.Card.Id, out CardData _card) || _card.CT == 0))
                    SetDecription(data.Card.OnlyOnce[(data.Card.OnlyOnce.Length - 1) - deckItem.DescIndex]);
                else if (data.Card.RStory)
                    SetDecription(data.Card.Descs[Random.Range(0, data.Card.Descs.Length)]);
                else
                    SetDecription(data.Card.Descs[(data.Card.Descs.Length - 1) - deckItem.DescIndex]);
            }
            else if (deckItem.State == CardData.REWARD)
            {
                if (data.Card.Reward != null)
                    randomReward.SetItems(data.Card.Reward[0].Where(r => r.Chance > 0).ToArray(), null, false);

                if (Services.Player.RewardCollected.Count > 0)
                {
                    backpack.gameObject.SetActive(true);
                    backpack.SetItems(Services.Player.RewardCollected, profile, Services.Meta.Game);

                    if (data.Card.RewardText == null)
                        throw new Exception($"Description is needed for reward {data.Card.Id}");

                    if (Services.Player.RewardCollected.Exists(r => r.Count > 0) || !data.Card.IfNothing.HasTexts())
                        SetDecription(data.Card.RewardText);
                    else
                        SetDecription(data.Card.IfNothing[0]);
                }
                else if (data.Card.Reward == null && data.Card.Cost == null && data.Card.Over != null)
                    SetDecription(data.Card.RewardText);
                else if (data.Card.IfNothing != null && data.Card.IfNothing.Length > 0)
                    SetDecription(data.Card.IfNothing[0]);
                else
                    throw new Exception($"card {data.Card.Id} must have no reward message");
            }
            else
            {
                throw new Exception($"Card {data.Card.Id} has no state");
            }
        }


        //         void OnTakeCard()
        //         {
        //             if (data.Card == null || data.Card.Type != CardMeta.TYPE_CARD)
        //                 return;
        // 
        //             // if (next.gameObject.activeInHierarchy)
        //             //     next.GetComponent<RectTransform>().DOScale(1.2f, 0.15f);
        // 
        //             //if (backpack.gameObject.activeInHierarchy)
        //             //    backpack.TakeCard();
        // 
        // 
        // 
        //             choice = -10;
        //         }


        //         void OnDrop()
        //         {
        //             if (data.Card == null || data.Card.Type != CardMeta.TYPE_CARD)
        //                 return;
        //             // 
        //             //             if (next.gameObject.activeInHierarchy)
        //             //                 next.GetComponent<RectTransform>().DOScale(1.0f, 0.15f);
        // 
        //             //if (backpack.gameObject.activeInHierarchy)
        //             //    backpack.DropCard();
        // 
        //             //left.FadeOut();
        //             //right.FadeOut();
        // 
        //             choice = -10;
        //         }



        public void Show(SwipeData data, Swipe swipe)
        {
            this.data = data;
            this.swipe = swipe;

            MetaService.ShowUnvalidateCard(data.Card);


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
            randomReward.Hide();
            needed.Hide();
            backpack.gameObject.SetActive(false);
            // next.gameObject.SetActive(false);
            // next.GetComponent<RectTransform>().DOKill();
            description.gameObject.SetActive(false);

            //left.HideAll();
            //right.HideAll();
            //delem.SetActive(false);

            descPanel.gameObject.SetActive(false);
        }

        /*
        void OnChangeDeviation(float dev)
        {
            //if (data == null || this.State != Swipe.States.DRAG || choiceble == false) return;

            if (System.Math.Abs(dev) < threshold)
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
                // left.FadeOut();
                // right.FadeOut();
            }
            else if (choice == CardMeta.LEFT)
            {
                // left.FadeIn();
                // right.FadeOut();
            }
            else if (choice == CardMeta.RIGHT)
            {
                // left.FadeOut();
                // right.FadeIn();
            }


        } */

    }
}
