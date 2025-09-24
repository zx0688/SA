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
using Unity.Android.Gradle.Manifest;

namespace UI.ActionPanel
{
    public class UIActionPanel : MonoBehaviour
    {
        //[SerializeField] private UIReward needed;

        [SerializeField] private UIReward randomReward;
        [SerializeField] private GameObject randomPanel;

        [SerializeField] private Text description;
        [SerializeField] private Text descChoice;
        //[SerializeField] private UICurrent backpack;
        [SerializeField] private List<Color32> colors;

        [SerializeField] private UIReward reward;

        //[SerializeField] private UIChoicePanel choicePanel;

        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private Image nextCardImage;
        [SerializeField] private Text title;


        private Swipe swipe;

        private ProfileData profile => Services.Player.Profile;


        private SwipeData data;

        void Awake()
        {
            HideAll();
        }


        void OnSet()
        {
            HideAll();

            DeckItem deckItem = data.Item;

            if (deckItem.State == CardData.CHOICE)
            {
                title.Localize("MadeChocie.UI", LocalizePartEnum.GUI);
            }
            else if (deckItem.State == CardData.NOTHING)
            {
                if (data.Card.Next.HasTriggers())
                {
                    var cards = new List<CardMeta>();
                    //Services.Meta.GetAllRecursiveCardsFromGroup(data.Card.Next, cards);
                    //RewardMeta[] cost = cards.SelectMany(c => c.Cost[0]).ToArray();
                    //needed.SetItems(null, cost.ToList());
                }
            }
            else if (deckItem.State == CardData.REWARD && Services.Player.RewardCollected.Count > 0)
            {
                rewardPanel.SetActive(true);
                var add = Services.Player.RewardCollected.Where(r => r.Count > 0).ToReward().ToList();
                var sub = Services.Player.RewardCollected.Where(r => r.Count < 0).ToReward()
                    .Select(r => { r.Count = -r.Count; return r; }).ToList();
                reward.SetItems(add, sub);

                var addr = data.Card.Reward.HasReward() ? data.Card.Reward[0].Where(r => r.Chance > 0).ToList() : null;
                var subr = data.Card.Cost.HasReward() ? data.Card.Cost[0].Where(r => r.Chance > 0).ToList() : null;
                randomPanel.SetActive(addr != null || subr != null);
                randomReward.SetItems(addr, subr);
            }

            if (deckItem.State != CardData.CHOICE && Services.Player.TryGetCardDescription(data.Card, out string desc))
                SetDecription(desc, description);
        }


        public void Show(SwipeData data, Swipe swipe)
        {
            this.data = data;
            this.swipe = swipe;

#if UNITY_EDITOR
            MetaService.ShowUnvalidateCard(data.Card);
#endif

            OnSet();

            if (data.Item.State == CardData.CHOICE)
                OnSwipeListenerEnable();

            gameObject.SetActive(true);
        }

        public void Hide()
        {

            HideAll();

            if (swipe)
                OnSwipeListenerDisable();

            gameObject.SetActive(false);
        }

        private void SetDecription(string desc, Text field, bool localize = true)
        {
            field.gameObject.SetActive(true);
            field.text = localize ? desc.Localize(LocalizePartEnum.CardDescription) : desc;

            if (desc.EndsWith("ask"))
                field.color = colors[1];
            else if (desc.EndsWith("tell"))
                field.color = colors[2];
            else
                field.color = colors[0];
        }

        private void HideAll()
        {
            //choicePanel.Hide();
            nextCardImage.gameObject.SetActive(false);
            randomReward.Hide();
            descChoice.gameObject.SetActive(false);
            //needed.Hide();
            //backpack.Hide();


            randomPanel.SetActive(false);

            // next.gameObject.SetActive(false);
            // next.GetComponent<RectTransform>().DOKill();
            description.gameObject.SetActive(false);
            rewardPanel.SetActive(false);

            //left.HideAll();
            //right.HideAll();
            //delem.SetActive(false);

        }
        private void OnSwipeListenerDisable()
        {
            swipe.OnTakeCard -= OnTakeCard;
            swipe.OnDrop -= OnDropCard;
            swipe.OnChangeDirection -= OnChangeDirection;
            swipe.OnEndSwipe -= OnEndSwipe;
        }

        private void OnSwipeListenerEnable()
        {
            swipe.OnTakeCard += OnTakeCard;
            swipe.OnDrop += OnDropCard;
            swipe.OnChangeDirection += OnChangeDirection;
            swipe.OnEndSwipe += OnEndSwipe;
        }

        private void OnEndSwipe()
        {
            HideAll();
        }

        private void OnChangeDirection(int obj)
        {
            List<string> allReward = new List<string>();
            var cardMeta = data.Choices[obj];
            ShowChoice(cardMeta, data.Item.Choices[obj], false, allReward);
            //backpack.gameObject.SetActive(allReward.Count > 0);

            //if (allReward.Count > 0)
            //   backpack.SetItems(allReward.Distinct().Select(s => new ItemData(s, 0)).ToList(), profile, Services.Meta.Game);
        }

        private void OnDropCard()
        {
            OnSet();
        }

        private void OnTakeCard()
        {
            //needed.Hide();
            //backpack.Hide();
            description.gameObject.SetActive(false);

            if (data.Item.State == CardData.CHOICE)
            {
                //madeChoicePanel.gameObject.SetActive(false);
                OnChangeDirection(0);
            }


        }

        private void ShowChoice(CardMeta cardMeta, ChoiceInfo info, bool showFollowPrompt, List<string> allRewards)
        {
            bool hasRewardOrCost = info.RI != -1 || info.CI != -1;
            rewardPanel.SetActive(false);

            nextCardImage.gameObject.SetActive(true);
            nextCardImage.LoadCardImage(cardMeta.Image);
            title.Localize(cardMeta.Name, LocalizePartEnum.CardName);

            string text = cardMeta.Act != null ? cardMeta.Act : Services.Assets.Localize("learn1", LocalizePartEnum.CardAction);
            SetDecription(text, descChoice, cardMeta.Act != null);
        }

    }
}
